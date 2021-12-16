using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
//Bodies
int bodiesIndex = 0;
[SerializeField] BodyProperties[] bodies;
public BodyProperties focusedBody;

//Camera Control
[Header("Camera Controls")]
[SerializeField] private Camera cam;
[SerializeField] float cameraOffest;
[SerializeField] float scrollSpeed;
private Vector3 previousPosition;


private void Awake()
{
    focusedBody = bodies[bodiesIndex];
}

void Update() 
{
    //Focus Body
    if(Input.GetKeyDown("space"))
    {
        focusedBody = bodies[bodiesIndex];
    }

    //Set Camera position
    cam.transform.position = focusedBody.transform.position;
    cam.transform.Translate(new Vector3(0,0, -cameraOffest));

    //Changing Planet Focus
    if (Input.GetKeyDown("left"))
    {
        if(bodiesIndex == 0){bodiesIndex = bodies.Length - 1;}
        else{bodiesIndex -= 1;}
        focusedBody = bodies[bodiesIndex];
    }
    if (Input.GetKeyDown("right"))
    {
        if(bodiesIndex == bodies.Length -1){bodiesIndex = 0;}
        else{bodiesIndex += 1;}
        focusedBody = bodies[bodiesIndex];
    }

    //Rotating Around Planets
    if(Input.GetMouseButtonDown(0))
    {
        previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
    }
    if(Input.GetMouseButton(0))
    {
        Vector3 direction = previousPosition - cam.ScreenToViewportPoint(Input.mousePosition);

        cam.transform.position = focusedBody.transform.position;

        cam.transform.Rotate(new Vector3(1,0,0), direction.y * 180);
        cam.transform.Rotate(new Vector3(0,1,0), -direction.x * 180, Space.World);
        cam.transform.Translate(new Vector3(0,0, -cameraOffest));

        previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
    }

    //Allow Zooming in and out
    if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
    {
        cameraOffest -= scrollSpeed;
    }
    else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
    {
        cameraOffest += scrollSpeed;
    }

}
}
