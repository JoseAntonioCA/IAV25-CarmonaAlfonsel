using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    bool m_IsAlive;
    // Start is called before the first frame update
    void Start()
    {
        m_IsAlive = true;
    }

    public bool IsAlive()
    {
        return m_IsAlive;
    }

    public void SetAlive(bool isAlive)
    {
        m_IsAlive = isAlive;
    }

    public void Revive()
    {
        SetAlive(true);
        GetComponent<NavMeshAgent>().isStopped = false;
        GetComponentInChildren<WaypointPatrol>().enabled = true;
        GetComponentInChildren<Observer>().enabled = true;
    }

    public void Kill()
    {
        SetAlive(false);
        if (GetComponentInChildren<WaypointPatrol>().enabled)
        {
            GetComponentInChildren<WaypointPatrol>().enabled = false;
        }
        else if (GetComponentInChildren<Investigate>().enabled)
        {
            GetComponentInChildren<Investigate>().enabled = false;
        }
        GetComponentInChildren<Observer>().enabled = false;
        GetComponent<NavMeshAgent>().isStopped = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
