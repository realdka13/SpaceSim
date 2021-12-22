using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//TODO: Make sure player can only move when grounded
public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed = 5f;

    //Grounding
    public float groundDistance = .25f;
    private bool grounded = false;
    private int layerMask;

    //Moving
    private Vector2 moveVal;
    private Vector3 moveDirection;

    private Rigidbody playerBody;

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

    private void Update()
    {
        
        grounded = Physics.CheckSphere(transform.position - transform.up ,groundDistance, layerMask);
    }

    private void FixedUpdate()
    {
        if (grounded)
        {
            //Move Player
            playerBody.AddForce(moveDirection.normalized * (playerSpeed * 10f), ForceMode.Acceleration);
        }

        //Gravity
        gravityForce = 9.8f;//(Universe.gravitationalConstant * attractorMass) / Vector3.Distance(attractorBody.transform.position, transform.position);
        playerBody.AddForce((attractorBody.transform.position - transform.position).normalized * gravityForce);
        playerBody.rotation = Quaternion.FromToRotation(transform.up, (transform.position - attractorBody.transform.position).normalized) * playerBody.rotation;
    }

    void OnMovement(InputValue value)
    {
        moveVal = value.Get<Vector2>();
        moveDirection = transform.forward * moveVal.y + transform.right * moveVal.x;
    }

/*     void OnDrawGizmos()
    {
        // Draw a black sphere at the transform's position
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position - transform.up, groundDistance);
    } */
}
