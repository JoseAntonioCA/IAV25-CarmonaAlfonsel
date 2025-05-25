using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction AttackAction;

    public GameObject john;
    public GameObject dropableObject;
    public CapsuleCollider collider;

    public float turnSpeed;
    public float movementSpeed;
    public float attackRange;

    public int maxNumberDropableObjects;
    int remainingDropableObjects;

    bool canHide;

    bool ImInvisible;

    bool hideButtonPressed;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start ()
    {
        hideButtonPressed = false;
        ImInvisible = false;
        canHide = true;
        remainingDropableObjects = maxNumberDropableObjects;
        m_Animator = GetComponent<Animator> ();
        m_Rigidbody = GetComponent<Rigidbody> ();
        m_AudioSource = GetComponent<AudioSource> ();
        
        MoveAction.Enable();
    }

    public void CanHide(bool hide)
    {
        canHide = hide;
    }

    public bool IsInvisible()
    {
        return ImInvisible;
    }

    private void DropObject()
    {
        Instantiate (dropableObject, transform.position, transform.rotation);
        remainingDropableObjects--;
    }

    private void OnDrawGizmos()
    //Metodo para ver el cono de visi�n, dibujando el �ngulo
    //no se hace nada si el angulo es menor que cero
    {
        float halfAngle = 15;
        Vector3 forward = transform.forward;

        for (int i = 0; i <= 10; i++)
        {
            float t = i / 10.0f;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * forward;

            Ray ray = new Ray(transform.position, dir);

            if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, hit.point);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, dir * attackRange);
            }
        }

        //Vector3 forward = transform.forward;

        //Ray ray = new Ray(transform.position, forward);

        //if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawLine(transform.position, hit.point);
        //}
        //else
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawRay(transform.position, forward * attackRange);
        //}
    }

    private void Attack()
    {

        float halfAngle = 15;
        Vector3 forward = transform.forward;

        for (int i = 0; i <= 5; i++)
        {
            float t = i / (float)5.0f;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * forward;

            Ray ray = new Ray(transform.position, dir);

            if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
            {
                if (hit.collider.gameObject.GetComponent<EnemyHealth>() != null)
                {
                    if (hit.collider.gameObject.GetComponent<EnemyHealth>().IsAlive() && !hit.collider.gameObject.GetComponentInChildren<ChasePlayer>().enabled)
                    {
                        Debug.Log("ENEMIGO DERRIBADO");
                        hit.collider.gameObject.GetComponent<EnemyHealth>().Kill();
                    }
                }
            }
        }

        //Vector3 forward = transform.forward;

        //Ray ray = new Ray(transform.position, forward);

        //if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
        //{
        //    if (hit.collider.gameObject.GetComponent<EnemyHealth>().IsAlive() && !hit.collider.gameObject.GetComponentInChildren<ChasePlayer>().enabled)
        //    {
        //        Debug.Log("ENEMIGO DERRIBADO");
        //        hit.collider.gameObject.GetComponent<EnemyHealth>().Kill();
        //    }
        //}
    }

    void Update()
    {
        //Debug.Log($"Puedo esconderme {canHide}");
        if (Input.GetKeyDown(KeyCode.Q) && remainingDropableObjects > 0)
        {
            Debug.Log("SUELTO UN OBJETO");
            DropObject();
        }
        else if (Input.GetKeyDown(KeyCode.Q) && remainingDropableObjects <= 0)
        {
            Debug.Log("NO ME QUEDAN OBJETOS");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }
    void FixedUpdate ()
    {
        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop();
        }

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);
    }

    void OnAnimatorMove ()
    {
        m_Rigidbody.MovePosition (m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude * movementSpeed);
        m_Rigidbody.MoveRotation (m_Rotation);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HIDE_POINT"))
        {
            if (canHide)
            {
                Debug.Log("ME PUEDO ESCONDER");
                ImInvisible = true;
            }
            else if (!canHide)
            {
                Debug.Log("NO ME PUEDO ESCONDER AHORA");
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HIDE_POINT"))
        {
            if (ImInvisible)
            {
                ImInvisible = false;
                this.gameObject.isStatic = true;
            }
        }
    }
}