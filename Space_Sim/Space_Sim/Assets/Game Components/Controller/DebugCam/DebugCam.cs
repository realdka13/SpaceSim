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
    private float zRot = 0f;    //Used for tilting
    private Vector3 moveDirection;

    //Speeds
    public float horizantalSpeed = .1f;
    public float verticalSpeed = .1f;
    public float xyRotationSpeed = 1f;
    public float zRotationSpeed = 1f;

    //Terrain
    public float terraformDistance;

    private void Start()
    {
        Cursor.visible = false;
        cameraTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        //Rotation
        float yRot = Input.GetAxis("Mouse X") * xyRotationSpeed * 10f * Time.deltaTime;
        float xRot = Input.GetAxis("Mouse Y") * xyRotationSpeed * 10f * Time.deltaTime;

        //xRot -= mouseY;
        //xRot = Mathf.Clamp(xRot, -90f, 90f);
       //yRot += mouseX;

        if(Input.GetKey("q"))
        {
            zRot = Time.deltaTime * zRotationSpeed;
        }
        else if(Input.GetKey("e"))
        {
            zRot = -Time.deltaTime * zRotationSpeed;
        }
        else
        {
            zRot = 0f;
        }

        cameraTransform.Rotate(new Vector3(-xRot,yRot,zRot));

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

        //Mouse Click (For terrain)
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = gameObject.GetComponent<Camera>().ViewportPointToRay(new Vector3(.5f,.5f,0));
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, terraformDistance))
            {
                if(hit.transform.tag == "Terrain")
                {
                    hit.transform.parent.GetComponent<MCOOctreeBody>().PlaceTerrain(hit.point);
                }
            }
        }
    }

    void OnMovement(InputValue value)
    {
        moveVal = value.Get<Vector2>();
    }
}
