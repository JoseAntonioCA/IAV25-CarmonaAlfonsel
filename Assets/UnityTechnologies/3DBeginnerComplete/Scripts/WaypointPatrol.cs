﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : BaseState
{
    public NavMeshAgent navMeshAgent;
    public Transform[] waypoints;

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
        enemyComunicator = comunicator.GetComponent<EnemyComunicator>();
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

                    Debug.Log("ALIADO CAÍDO, VOY A AYUDARLO, INVESTIGAD LA ZONA");
                    GetComponent<Investigate>().enabled = true;
                    GetComponent<Investigate>().GoToPointToInvestigate(hit.transform);

                    GetComponent<Investigate>().PartnerToRevive(hit.collider.gameObject);
                    this.enabled = false;
                }
                else if (hit.collider.gameObject.CompareTag("PLAYER_ITEM"))
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
        transform.rotation = navMeshAgent.transform.rotation;
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            GetComponent<Investigate>().enabled = true;
            GetComponent<Investigate>().GoToPointToInvestigate(waypoints[m_CurrentWaypointIndex]);
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
            this.enabled = false;
            //navMeshAgent.SetDestination (waypoints[m_CurrentWaypointIndex].position);
        }
    }
}
