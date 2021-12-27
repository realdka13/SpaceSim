using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugCam : MonoBehaviour
{
    //Objects
    private Transform cameraTransform;

    //Movement
    private Vector2 moveVal;
    private float xRot = 0f;    //Used for looking up and down
    private float yRot = 0f;    //used for spinning
    private float zRot = 0f;    //Used for tilting
    private Vector3 moveDirection;

    //Speeds
    public float horizantalSpeed = .1f;
    public float verticalSpeed = .1f;
    public float xyRotationSpeed = .1f;
    public float zRotationSpeed = 1f;

    private void Start()
    {
        Cursor.visible = false;
        cameraTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        //Rotation
        float mouseX = Input.GetAxis("Mouse X") * xyRotationSpeed * 10f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * xyRotationSpeed * 10f * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        yRot += mouseX;

        if(Input.GetKey("q"))
        {
            zRot += zRotationSpeed;
        }
        else if(Input.GetKey("e"))
        {
            zRot -= zRotationSpeed;
        }

        cameraTransform.localRotation = Quaternion.Euler(xRot, yRot, zRot);

        //WASD Movement
        moveDirection = transform.forward * moveVal.y + transform.right * moveVal.x;
        cameraTransform.position = Vector3.Lerp(transform.position, transform.position + moveDirection, horizantalSpeed);

        //Speed adjust
        horizantalSpeed += Input.mouseScrollDelta.y * .1f;

        //Up_Down Movement
        if (Input.GetKey("space"))
        {
            cameraTransform.position = Vector3.Lerp(transform.position, transform.position + transform.up, verticalSpeed);
        }
        else if(Input.GetKey("left shift"))
        {
            cameraTransform.position = Vector3.Lerp(transform.position, transform.position - transform.up, verticalSpeed);
        }
    }

    void OnMovement(InputValue value)
    {
        moveVal = value.Get<Vector2>();
    }
}
