using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    Transform player;
    GameManager gameManager;
    public NavMeshAgent navMeshAgent;
    public Transform[] waypoints;

    public float visionRange = 10f;         // Cuánto alcance tiene el cono
    public float visionAngle = 30f;         // Ángulo total del cono
    public int rayCount = 20;               // Cuántos rayos lanzar
    //public LayerMask obstacleMask;          // Capas a detectar

    int m_CurrentWaypointIndex;

    private void OnEnable()
    {
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }

    void Start ()
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
        navMeshAgent.SetDestination (waypoints[0].position);
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
                    GetComponent<ChasePlayer>().enabled = true;
                    this.enabled = false;
                }
                if (hit.collider.gameObject.CompareTag("PLAYER_ITEM"))
                {
                    GetComponent<Investigate>().enabled = true;
                    GetComponent<Investigate>().ObjectToDestroy(hit.collider.gameObject);
                    GetComponent<Investigate>().GoToPointToInvestigate(hit.transform);
                    this.enabled = false;
                }
            }
        }
    }

    void Update ()
    {
        DetectSmth();

        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            GetComponent<Investigate>().enabled = true;
            //GetComponent<Investigate>().GoToPointToInvestigate(waypoints[m_CurrentWaypointIndex]);
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
            this.enabled = false;
            //navMeshAgent.SetDestination (waypoints[m_CurrentWaypointIndex].position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER_ITEM"))
        {
            GetComponent<Investigate>().enabled = true;
            GetComponent<Investigate>().ObjectToDestroy(other.gameObject);
            GetComponent<Investigate>().GoToPointToInvestigate(other.transform);
            this.enabled = false;
        }
    }
}
