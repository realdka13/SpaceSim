using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//TODO: Look direction not being taken into account when actively moving

public class PlayerMovement_RB : MonoBehaviour
{
    public float playerSpeed = 5f;
    public bool useAttractor = false;
    private Rigidbody playerBody;

    //Grounding
    public float groundDistance = .25f;
    private bool grounded = false;
    private int layerMask;

    //Moving
    private Vector2 moveVal;
    private Vector3 moveDirection;

    //Gravity
    [Space]
    public GameObject attractorBody;
    public float attractorMass;
    public float gravityForce; //Using g=-(GM)(r^2)

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody>();
        //attractorMass = attractorBody.GetComponent<BodyProperties>().mass;
        layerMask = ~LayerMask.GetMask("Player");
    }

    private void Start()
    {
        if(useAttractor)
        {
            playerBody.useGravity = false;
        }
        else
        {
            playerBody.useGravity = true;
        }
    }

    private void Update()
    {
        grounded = Physics.CheckSphere(transform.position ,groundDistance, layerMask);
    }

    private void FixedUpdate()
    {
        //if (grounded)
        //{
            //Move Player
            moveDirection = transform.forward * moveVal.y + transform.right * moveVal.x;
            playerBody.AddForce(moveDirection.normalized * (playerSpeed * 10f), ForceMode.Acceleration);
       //}

        //Gravity
        if(useAttractor && attractorBody != null)
        {
            gravityForce = 9.8f;//(Universe.gravitationalConstant * attractorMass) / Vector3.Distance(attractorBody.transform.position, transform.position);
            playerBody.AddForce((attractorBody.transform.position - transform.position).normalized * gravityForce);
            playerBody.rotation = Quaternion.FromToRotation(transform.up, (transform.position - attractorBody.transform.position).normalized) * playerBody.rotation;
        }
        else if (attractorBody == null && useAttractor)
        {
            useAttractor = false;
            playerBody.useGravity = true;
        }
    }

    void OnMovement(InputValue value)
    {
        moveVal = value.Get<Vector2>();
    }

    void OnDrawGizmos()
    {
        //Draw a black sphere at the transform's position
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position, groundDistance);

        //Gravity
        if(attractorBody != null)
        {
            Gizmos.DrawLine(transform.position, attractorBody.transform.position);
        }
    }
}
