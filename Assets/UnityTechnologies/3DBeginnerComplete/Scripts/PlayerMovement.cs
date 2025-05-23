using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction HideAction;
    public InputAction AttackAction;

    public GameObject dropableObject;
    public CapsuleCollider collider;

    public float turnSpeed;
    public float movementSpeed;

    public int maxNumberDropableObjects;
    int remainingDropableObjects;

    bool canHide;

    bool ImInvisible;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start ()
    {
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

    void FixedUpdate ()
    {
        if (!ImInvisible)
        {
            if (Input.GetKeyDown(KeyCode.Q) && remainingDropableObjects > 0)
            {
                Debug.Log("SUELTO UN OBJETO");
                DropObject();
            }
            else if (Input.GetKeyDown(KeyCode.Q) && remainingDropableObjects <= 0)
            {
                Debug.Log("NO ME QUEDAN OBJETOS");
            }

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
    }

    void OnAnimatorMove ()
    {
        m_Rigidbody.MovePosition (m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude * movementSpeed);
        m_Rigidbody.MoveRotation (m_Rotation);
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HIDE_POINT"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (canHide && !ImInvisible)
                {
                    Debug.Log("ME PUEDO ESCONDER");
                    ImInvisible = true;
                    this.gameObject.isStatic = true;
                }
                else if (!canHide && !ImInvisible)
                {
                    Debug.Log("NO ME PUEDO ESCONDER AHORA");
                }
                else if (ImInvisible)
                {
                    ImInvisible = false;
                    this.gameObject.isStatic = true;
                }
            }
        }
    }
}