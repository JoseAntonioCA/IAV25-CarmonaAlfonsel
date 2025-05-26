using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : BaseState
{
    public NavMeshAgent navMeshAgent;
    public float chaseTime;
    float stopChaseCoolDown;
    float originalStoppingDistance;

    bool playerVisible;
    bool partnerAlert;

    public void SeePlayer(bool see)
    {
        playerVisible = see;
        partnerAlert = see;
    }

    private void OnEnable()
    {
        Debug.Log("JUGADOR AVISTADO, PROCEDO A PERSEGUIRLO, QUE ME AYUDEN MIS COMPAÑEROS");
        originalStoppingDistance = navMeshAgent.stoppingDistance;
        playerVisible = true;
        partnerAlert = true;
        navMeshAgent.SetDestination(player.position);
        
    }

    private void OnDisable()
    {
        navMeshAgent.stoppingDistance = originalStoppingDistance;
        stopChaseCoolDown = chaseTime;
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

    public override void DetectSmth()
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

        transform.rotation = navMeshAgent.transform.rotation;

        if (player.gameObject.GetComponent<PlayerMovement>().IsInvisible())
        {
            navMeshAgent.stoppingDistance = originalStoppingDistance * 10;
        }
        else
        {
            navMeshAgent.stoppingDistance = originalStoppingDistance;
        }

        navMeshAgent.SetDestination(player.position);
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && player.gameObject.GetComponent<PlayerMovement>().IsInvisible())
        {
            int rndNumber = Random.Range(0, 100);
            ScanLookablePoints();
            enemyComunicator.PartnersSpreadAroundTheArea(lookablePoints);
            
            if (rndNumber <= 50)
            {
                Debug.Log("MIRO AQUÍ MISMO SI ENCUENTRO AL JUGADOR");
                GetComponent<Investigate>().enabled = true;
                GetComponent<Investigate>().GoToPointToInvestigate(transform);
            }
            else
            {
                Debug.Log("PERDÍ AL JUGADOR, VOY A UN PUNTO CERCANO");
                GetComponent<Investigate>().enabled = true;
                GetComponent<Investigate>().GoToPointToInvestigate(RandomDestination());
            }
            gameManager.PlayerIsMissing();
            this.enabled = false;
        }
        if (playerVisible)
        {
            stopChaseCoolDown = chaseTime;
        }
        else if (!playerVisible)
        {
            stopChaseCoolDown -= Time.deltaTime;
            if (stopChaseCoolDown <= 0.0f)
            {
                Debug.Log("LO HE PERDIDO, CONTINUO LA PATRULLA");
                gameManager.PlayerIsMissing();
                enemyComunicator.StopChasingPlayer();
                GetComponent<WaypointPatrol>().enabled = true;
                this.enabled = false;
            }
        }

        partnerAlert = false;

    }
}
