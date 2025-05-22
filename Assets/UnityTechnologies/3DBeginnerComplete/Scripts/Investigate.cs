using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Investigate : MonoBehaviour
{
    Transform player;
    GameManager gameManager;
    public NavMeshAgent navMeshAgent;
    Transform pointToInvestigate;
    public BoxCollider viewPoint;
    public float investigationTime;
    float stopInvestigationCoolDown;

    public void GoToPointToInvestigate(Transform point)
    {
        pointToInvestigate = point;
    }

    private void OnEnable()
    {
        if (pointToInvestigate != null)
            navMeshAgent.SetDestination(pointToInvestigate.position);
    }
    private void OnDisable()
    {
        stopInvestigationCoolDown = investigationTime;
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
        this.enabled = false;
    }

    void Update()
    {
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            stopInvestigationCoolDown -= Time.deltaTime;

            if (stopInvestigationCoolDown <= investigationTime / 2)
            {
                transform.Rotate(new Vector3(0, 1, 0));
            }

            if (stopInvestigationCoolDown <= 0.0f)
            {
                GetComponent<WaypointPatrol>().enabled = true;
                this.enabled = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            GetComponent<ChasePlayer>().enabled = true;
            this.enabled = false;
        }
    }
}
