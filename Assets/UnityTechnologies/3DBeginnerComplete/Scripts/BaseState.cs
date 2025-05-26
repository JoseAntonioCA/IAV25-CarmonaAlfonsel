using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseState : MonoBehaviour
{

    public Transform player;
    public GameManager gameManager;
    public EnemyComunicator enemyComunicator;
    public GameObject comunicator;

    public List<Transform> lookablePoints = new List<Transform>();

    public float visionRange = 10f;         // Cuánto alcance tiene el cono
    public float visionAngle = 30f;         // Ángulo total del cono
    public int rayCount = 20;               // Cuántos rayos lanzar

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
            if (Physics.Raycast(ray, out RaycastHit hit, visionRange) &&
                (hit.transform == player || (hit.collider.CompareTag("GHOST_CORE") && !hit.collider.gameObject.GetComponent<EnemyHealth>().IsAlive())))
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

    public virtual void DetectSmth()
    {
        Debug.Log("DetectStmh CLASE PADRE");
    }

    public void ScanLookablePoints()
    {
        lookablePoints.Clear();
        Collider[] colisiones = Physics.OverlapSphere(transform.position, 20.0f);

        foreach (Collider col in colisiones)
        {
            if (col.CompareTag("LOOK_POINT")) // puedes omitir esto si solo usas la capa
            {
                lookablePoints.Add(col.transform);
            }
        }
    }

    public Transform RandomDestination()
    {
        if (lookablePoints.Count == 0) return transform;

        int index = Random.Range(0, lookablePoints.Count);

        return lookablePoints[index];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}