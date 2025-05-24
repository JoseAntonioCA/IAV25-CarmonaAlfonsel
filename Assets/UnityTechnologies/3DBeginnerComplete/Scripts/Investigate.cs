using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Investigate : MonoBehaviour
{
    Transform player;
    GameManager gameManager;
    EnemyComunicator enemyComunicator;
    public GameObject comunicator;
    GameObject objectToDestroy;

    Quaternion originalRotation;
    public NavMeshAgent navMeshAgent;
    Transform pointToInvestigate;
    public float investigationTime;
    float stopInvestigationCoolDown;

    public float visionRange = 10f;         // Cuánto alcance tiene el cono
    public float visionAngle = 30f;         // Ángulo total del cono
    public int rayCount = 20;               // Cuántos rayos lanzar
    //public LayerMask obstacleMask;          // Capas a detectar

    public void GoToPointToInvestigate(Transform point)
    {
        originalRotation = transform.rotation;
        pointToInvestigate = point;
        navMeshAgent.SetDestination(pointToInvestigate.position);
    }

    public void ObjectToDestroy(GameObject obj)
    {
        objectToDestroy = obj;
    }

    private void OnEnable()
    {
        Debug.Log("VOY A INVESTIGAR");
        //if (pointToInvestigate != null)
        //navMeshAgent.updateRotation = false;
    }
    private void OnDisable()
    {
        transform.rotation = originalRotation;
        Debug.Log("DEJO DE INVESTIGAR");
        if (objectToDestroy != null)
            Destroy(objectToDestroy);
        stopInvestigationCoolDown = investigationTime;
        //navMeshAgent.updateRotation = true;
    }

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
        stopInvestigationCoolDown = investigationTime;
        navMeshAgent.updateRotation = true;
        enemyComunicator = comunicator.GetComponent<EnemyComunicator>();
        this.enabled = false;
    }
    private void OnDrawGizmos()
    //Metodo para ver el cono de visi�n, dibujando el �ngulo
    //no se hace nada si el angulo es menor que cero
    {
        float halfAngle = visionAngle / 2;
        Vector3 forward = transform.forward;

        for (int i = 0; i <= rayCount; i++)
        {
            float t = i / (float)rayCount;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * forward;

            Ray ray = new Ray(transform.position, dir);
            if (Physics.Raycast(ray, out RaycastHit hit, visionRange) && hit.transform == player)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, hit.point);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, dir * visionRange);
            }
        }
    }

    private void DetectSmth()
    {
        float halfAngle = visionAngle / 2;
        Vector3 forward = transform.forward;

        for (int i = 0; i <= rayCount; i++)
        {
            float t = i / (float)rayCount;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * forward;

            Ray ray = new Ray(transform.position, dir);
            if (Physics.Raycast(ray, out RaycastHit hit, visionRange))
            {
                if (hit.transform == player && !player.gameObject.GetComponent<PlayerMovement>().IsInvisible())
                {
                    gameManager.AlertPlayerDetected();
                    GetComponent<ChasePlayer>().enabled = true;
                    enemyComunicator.GoAndChasePlayer();
                    this.enabled = false;
                }
            }
        }
    }

    void Update()
    {
        DetectSmth();

        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance * 3)
        {
            stopInvestigationCoolDown -= Time.deltaTime;

            if (stopInvestigationCoolDown <= investigationTime / 2)
            {
                transform.Rotate(new Vector3(0, 2, 0));
            }

            if (stopInvestigationCoolDown <= 0.0f)
            {
                GetComponent<WaypointPatrol>().enabled = true;
                this.enabled = false;
            }
        }
    }
}
