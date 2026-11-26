using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AudioNavigatorComponent : MonoBehaviour
{
    public Transform player;
    public GameManager gameManager;
    private StudioEventEmitter audioSource;
    private float occlusionFactor;

    public float grosorMaximo = 1f;
    public float distanciaMax = 20f;

    private NavMeshPath camino;
    void Start()
    {
        GameObject gameManagerObject = GameObject.FindWithTag("GameManager");
        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
            if (gameManager == null)
            {
                Debug.LogWarning("No se encontró un componente 'GameManager' en el objeto 'GameManager' correspondiente.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con la etiqueta 'GameManager'.");
        }

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con la etiqueta 'Player'.");
        }
        audioSource = GetComponent<StudioEventEmitter>();

        camino = new NavMeshPath();
    }

    private float MedirGrosor()
    {
        Vector3 origen = transform.position;
        Vector3 destino = player.position;
        Vector3 direccion = (destino - origen).normalized;
        float distancia = Vector3.Distance(origen, destino);
        float grosorTotal = 0f;

        RaycastHit[] paredes = Physics.RaycastAll(
            origen, direccion, distancia,
            LayerMask.GetMask("PARED"),
            QueryTriggerInteraction.Ignore
        );

        foreach (RaycastHit entrada in paredes)
        {
            Ray rayoInverso = new Ray(destino, -direccion);

            if (entrada.collider.Raycast(
                rayoInverso, out RaycastHit salida, distancia))
            {
                grosorTotal += Mathf.Max(
                    0f, distancia - entrada.distance - salida.distance);
            }
        }

        return grosorTotal;
    }

    private float MedirDistancia()
    {
        bool encontrado = NavMesh.CalculatePath(
            transform.position, player.position,
            NavMesh.AllAreas, camino
        );

        if (!encontrado || camino.status != NavMeshPathStatus.PathComplete)
            return distanciaMax;

        float distancia = 0f;
        Vector3[] puntos = camino.corners;

        for (int i = 1; i < puntos.Length; i++)
        {
            distancia += Vector3.Distance(puntos[i - 1], puntos[i]);
        }

        return distancia;
    }

    // Update is called once per frame
    void Update()
    {

        if (player == null || audioSource == null)
            return;

        bool hayPared = Physics.Linecast(
                        transform.position,
                        player.position,
                        LayerMask.GetMask("PARED"),
                        QueryTriggerInteraction.Ignore
);
        if(hayPared){
            float grosor = MedirGrosor();
            float distanciaCamino = MedirDistancia();

            float grosorFactor = Mathf.Clamp01(grosor / grosorMaximo);
            float distanciaFactor = Mathf.Clamp01(distanciaCamino / distanciaMax);

            occlusionFactor = grosorFactor * 0.4f + distanciaFactor * 0.6f;
        }
        else
        {
            occlusionFactor = 0;
        }
        audioSource.SetParameter("Oclusion", occlusionFactor);
    }
}
