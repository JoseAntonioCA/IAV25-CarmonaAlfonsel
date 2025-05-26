using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Investigate : BaseState
{
    GameObject objectToDestroy;
    GameObject partnerToRevive;

    public NavMeshAgent navMeshAgent;
    Transform pointToInvestigate;
    public float investigationTime;
    float stopInvestigationCoolDown;

    private void OnEnable()
    {
        Debug.Log("VOY A INVESTIGAR");
    }
    private void OnDisable()
    {
        Debug.Log("DEJO DE INVESTIGAR");

        if (objectToDestroy != null)
            Destroy(objectToDestroy);

        stopInvestigationCoolDown = investigationTime;
    }
    public void ResetTime()
    {
        stopInvestigationCoolDown = investigationTime;
    }
    public void GoToPointToInvestigate(Transform point)
    {
        pointToInvestigate = point;
        navMeshAgent.SetDestination(pointToInvestigate.position);
    }

    public void ObjectToDestroy(GameObject obj)
    {
        objectToDestroy = obj;
    }
    public void PartnerToRevive(GameObject obj)
    {
        partnerToRevive = obj;
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
        enemyComunicator = comunicator.GetComponent<EnemyComunicator>();
        this.enabled = false;
    }

    public override void DetectSmth()
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
                else if (hit.collider.CompareTag("GHOST_CORE") && !hit.collider.gameObject.GetComponent<EnemyHealth>().IsAlive())
                {
                    ScanLookablePoints();
                    enemyComunicator.PartnerIsUnconciousSpread(lookablePoints);
                    ResetTime();

                    Debug.Log("ALIADO CAÍDO, VOY A AYUDARLO, INVESTIGAD LA ZONA");
                    GetComponent<Investigate>().enabled = true;
                    GetComponent<Investigate>().GoToPointToInvestigate(hit.transform);

                    GetComponent<Investigate>().PartnerToRevive(hit.collider.gameObject);
                }
                else if (hit.collider.gameObject.CompareTag("PLAYER_ITEM"))
                {
                    ResetTime();
                    GetComponent<Investigate>().enabled = true;
                    GetComponent<Investigate>().ObjectToDestroy(hit.collider.gameObject);
                    GetComponent<Investigate>().GoToPointToInvestigate(hit.transform);
                }
            }
        }
    }

    void Update()
    {
        DetectSmth();

        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance * 3)
        {

            if (partnerToRevive != null)
            {
                if (partnerToRevive.GetComponent<EnemyHealth>() != null && !partnerToRevive.GetComponent<EnemyHealth>().IsAlive())
                {
                    partnerToRevive.GetComponent<EnemyHealth>().Revive();
                }
            }

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
