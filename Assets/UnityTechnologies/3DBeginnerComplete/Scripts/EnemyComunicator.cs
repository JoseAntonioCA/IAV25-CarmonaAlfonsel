using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Jobs;
using UnityEngine;

public class EnemyComunicator : MonoBehaviour
{

    List<GameObject> partners;
    // Start is called before the first frame update
    void Start()
    {
        partners = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoAndChasePlayer()
    {
        partners.RemoveAll(p => p == null);
        if (partners.Count > 0) {
            foreach (GameObject obj in partners)
            {
                //Debug.Log("Objeto en partners: " + obj.name);
                //Debug.Log("Tiene WaypointPatrol: " + (obj.GetComponentInChildren<WaypointPatrol>() != null));

                if (obj.GetComponentInChildren<WaypointPatrol>().enabled)
                {
                    obj.GetComponentInChildren<WaypointPatrol>().enabled = false;
                }


                obj.GetComponentInChildren<ChasePlayer>().enabled = true;
            }
        }
    }

    public void IseePlayer()
    {
        if (partners.Count > 0)
        {
            foreach (GameObject obj in partners)
            {
                if (obj.GetComponentInChildren<WaypointPatrol>().enabled)
                {
                    obj.GetComponentInChildren<WaypointPatrol>().enabled = false;
                }


                if (!obj.GetComponentInChildren<ChasePlayer>().enabled)
                    obj.GetComponentInChildren<ChasePlayer>().enabled = true;
                
                obj.GetComponentInChildren<ChasePlayer>().SeePlayer(true);
            }
        }
    }

    public void StopChasingPlayer()
    {
        if (partners.Count > 0)
        {
            foreach (GameObject obj in partners)
            {
                if (obj.GetComponentInChildren<ChasePlayer>().enabled)
                    obj.GetComponentInChildren<ChasePlayer>().enabled = false;

                if (!obj.GetComponentInChildren<WaypointPatrol>().enabled && !obj.GetComponentInChildren<Investigate>().enabled)
                    obj.GetComponentInChildren<WaypointPatrol>().enabled = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"OnTriggerEnter con {other.name} (tag: {other.tag})");
        if (other.CompareTag("GHOST"))
        {
            EnemyHealth myEnemy = GetComponentInParent<EnemyHealth>();
            EnemyHealth otherEnemy = other.GetComponentInParent<EnemyHealth>();

            if (myEnemy == null || otherEnemy == null) return;

            //Debug.Log($"Yo soy: {myEnemy.name}, Otro es: {otherEnemy.name}");

            if (myEnemy != otherEnemy)
            {
                if (!partners.Contains(otherEnemy.gameObject))
                {
                    Debug.Log("TENGO UN ALIADO MÁS EN EL COMUNICADOR");
                    //Debug.Log("Añadiendo a partners: " + otherEnemy.gameObject.name);
                    partners.Add(otherEnemy.gameObject);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GHOST"))
        {
            EnemyHealth myEnemy = GetComponentInParent<EnemyHealth>();
            EnemyHealth otherEnemy = other.GetComponentInParent<EnemyHealth>();

            if (myEnemy == null || otherEnemy == null) return;

            //Debug.Log($"Yo soy: {myEnemy.name}, Otro es: {otherEnemy.name}");

            if (myEnemy != otherEnemy)
            {
                if (partners.Contains(otherEnemy.gameObject))
                {
                    Debug.Log("SE FUE UN ALIADO DE MI COMUNICADOR");
                    //Debug.Log("Quitando de partners: " + otherEnemy.gameObject.name);
                    partners.Remove(otherEnemy.gameObject);
                }
            }
        }
    }

}
