using System.Collections;
using System.Collections.Generic;
using System.Numerics;
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
        if (partners.Count > 0) {
            foreach (GameObject obj in partners)
            {
                if (obj.GetComponent<WaypointPatrol>().enabled)
                {
                    obj.GetComponent<WaypointPatrol>().enabled = false;
                }


                obj.GetComponent<ChasePlayer>().enabled = true;
            }
        }
    }

    public void StopChasingPlayer()
    {
        if (partners.Count > 0)
        {
            foreach (GameObject obj in partners)
            {
                if (obj.GetComponent<ChasePlayer>().enabled)
                    obj.GetComponent<ChasePlayer>().enabled = false;

                if (!obj.GetComponent<WaypointPatrol>().enabled)
                    obj.GetComponent<WaypointPatrol>().enabled = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GHOST")
        {
            bool encontrado = false;

            foreach (GameObject obj in partners)
            {
                if (other.name == obj.name)
                    encontrado = true;
            }

            if (!encontrado)
            {
                partners.Add(other.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "GHOST")
        {
            partners.Remove(other.gameObject);
        }
    }

}
