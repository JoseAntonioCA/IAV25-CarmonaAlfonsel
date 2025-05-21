using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : MonoBehaviour
{
    Transform player;
    GameManager gameManager;
    public NavMeshAgent navMeshAgent;
    public float chaseTime;
    float stopChaseCoolDown;

    bool playerVisible;

    private void OnEnable()
    {
        GetComponent<EnemyComunicator>().GoAndChasePlayer();
        playerVisible = true;
    }

    private void OnDisable()
    {
        Debug.Log("LO HE PERDIDO, CONTINUO LA PATRULLA");
        stopChaseCoolDown = chaseTime;
        GetComponent<WaypointPatrol>().enabled = true;
        playerVisible = false;
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
        this.enabled = false;
        playerVisible = false;
    }

    void Update()
    {
        navMeshAgent.SetDestination(player.position);
        if (playerVisible)
        {
            stopChaseCoolDown = chaseTime;
        }
        else
        {
            stopChaseCoolDown -= Time.deltaTime;
            Debug.Log(stopChaseCoolDown);
            if (stopChaseCoolDown <= 0.0f)
            {
                
                this.enabled = false;
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            playerVisible = true;
            Debug.Log("HE VISTO AL JUGADOR");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            playerVisible = false;
            Debug.Log("ESTOY PERDIENDO AL JUGADOR");
        }
    }
}
