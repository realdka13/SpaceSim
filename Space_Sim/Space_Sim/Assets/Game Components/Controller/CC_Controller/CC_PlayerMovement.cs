using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CC_PlayerMovement : MonoBehaviour
{
    //Player
    private CharacterController controller;
    public float playerSpeed = 5f;
    public bool useAttractor = false;

    //Moving
    private Vector2 moveVal;
    private Vector3 moveDirection;

    //Gravity
    [Space]
    private Vector3 velocity;
    public GameObject attractorBody;
    public float attractorMass;
    public float gravityForce; //Using g=-(GM)(r^2)

    //Grounding
    public float groundDistance = .25f;
    private bool grounded = false;
    private int layerMask;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        layerMask = ~LayerMask.GetMask("Player");
    }

    private void Update()
    {
        //Movement
        moveDirection = transform.forward * moveVal.y + transform.right * moveVal.x;
        controller.Move(moveDirection * playerSpeed * Time.deltaTime);

        //Gravity
        if(useAttractor)
        {
            gravityForce = 9.81f;//(Universe.gravitationalConstant * attractorMass) / Vector3.Distance(attractorBody.transform.position, transform.position);
            controller.Move((attractorBody.transform.position - transform.position).normalized * gravityForce * Time.deltaTime);
            transform.rotation = Quaternion.FromToRotation (Vector3.up, (transform.position - Vector3.zero).normalized); 
        }
        else
        {
            velocity.y += -9.81f * Time.deltaTime;
            controller.Move(.5f * velocity * Time.deltaTime);
        }

        //Grounded
        grounded = Physics.CheckSphere(transform.position ,groundDistance, layerMask);
        if(grounded && velocity.y < 0)  //Reset v to a small number
        {
            velocity.y = -2f;
        }
    }




    void OnMovement(InputValue value)
    {
        moveVal = value.Get<Vector2>();
    }

    void OnDrawGizmos()
    {
        //Ground Sphere
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position, groundDistance);

        //Gravity
        Gizmos.DrawLine(transform.position, attractorBody.transform.position);
    }

}
