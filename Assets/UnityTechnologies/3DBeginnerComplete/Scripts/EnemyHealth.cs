using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;

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
        GetComponent<WaypointPatrol>().enabled = true;
    }

    public void Kill()
    {
        SetAlive(false);
        if (GetComponent<WaypointPatrol>().enabled)
        {
            GetComponent<WaypointPatrol>().enabled = false;
        }
        else if (GetComponent<Investigate>().enabled)
        {
            GetComponent<Investigate>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
