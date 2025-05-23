using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : MonoBehaviour
{
    Transform player;
    GameManager gameManager;
    EnemyComunicator enemyComunicator;
    public GameObject comunicator;
    public NavMeshAgent navMeshAgent;
    public float chaseTime;
    float stopChaseCoolDown;

    bool playerVisible;
    bool partnerAlert;

    public float visionRange = 10f;         // Cuánto alcance tiene el cono
    public float visionAngle = 30f;         // Ángulo total del cono
    public int rayCount = 20;               // Cuántos rayos lanzar
    //public LayerMask obstacleMask;          // Capas a detectar

    public void SeePlayer(bool see)
    {
        playerVisible = see;
        partnerAlert = see;
    }

    private void OnEnable()
    {
        Debug.Log("JUGADOR AVISTADO, PROCEDO A PERSEGUIRLO, QUE ME AYUDEN MIS COMPAÑEROS");
        enemyComunicator.GoAndChasePlayer();
        playerVisible = true;
        partnerAlert = true;
    }

    private void OnDisable()
    {
        Debug.Log("LO HE PERDIDO, CONTINUO LA PATRULLA");
        enemyComunicator.StopChasingPlayer();
        stopChaseCoolDown = chaseTime;
        GetComponent<WaypointPatrol>().enabled = true;
        playerVisible = false;
        partnerAlert = false;
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
        playerVisible = false;
        partnerAlert = false;
        enemyComunicator = comunicator.GetComponent<EnemyComunicator>();
        stopChaseCoolDown = chaseTime;
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

        bool jugadorEncontrado = false;

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
                    jugadorEncontrado = true;
                    enemyComunicator.IseePlayer();
                    hit.collider.gameObject.GetComponent<PlayerMovement>().CanHide(false);
                    playerVisible = true;
                    //Debug.Log("HE VISTO AL JUGADOR");
                }
            }
        }

        if (!jugadorEncontrado && !partnerAlert)
        {
            playerVisible = false;
            player.gameObject.GetComponent<PlayerMovement>().CanHide(true);
        }
    }

    void Update()
    {
        DetectSmth();

        //Debug.Log($"Yo soy: {this.name}, Puedo ver al jugador: {playerVisible}");
        navMeshAgent.SetDestination(player.position);
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            int rndNumber = Random.Range(0, 100);

            if (rndNumber <= 50)
            {
                GetComponent<Investigate>().enabled = true;
            }
            else
            {

            }

            this.enabled = false;
        }
        else if (playerVisible)
        {
            stopChaseCoolDown = chaseTime;
        }
        else if (!playerVisible)
        {
            stopChaseCoolDown -= Time.deltaTime;
            //Debug.Log(stopChaseCoolDown);
            if (stopChaseCoolDown <= 0.0f)
            {
                this.enabled = false;
            }
        }

        partnerAlert = false;

    }

    void OnTriggerEnter(Collider other)
    {
        //if (other.transform == player)
        //{
        //    enemyComunicator.IseePlayer();
        //    playerVisible = true;
        //    Debug.Log("HE VISTO AL JUGADOR");
        //}
    }

    void OnTriggerStay(Collider other)
    {
        //if (other.transform == player)
        //{
        //    //enemyComunicator.IseePlayer();
        //    other.GetComponent<PlayerMovement>().CanHide(false);
        //    playerVisible = true;
        //}
    }
    void OnTriggerExit(Collider other)
    {
        //if (other.transform == player)
        //{
        //    playerVisible = false;
        //    Debug.Log("ESTOY PERDIENDO AL JUGADOR");
        //}
    }
}
