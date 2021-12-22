using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public GameObject playerCamera;
    public float mouseSensitivity = 5f;

    private Transform playerTransform;
    private Transform cameraTransform;

    private float xRot = 0f;    //Used for looking up and down

    private void Start()
    {
        Cursor.visible = false;
        playerTransform = GetComponent<Transform>();
        cameraTransform = playerCamera.GetComponent<Transform>();
    }

    void OnMouseLook(InputValue value)
    {
        Vector2 moveVal = value.Get<Vector2>();
        float mouseX = moveVal.x * (mouseSensitivity * 10f) * Time.deltaTime;
        float mouseY = moveVal.y * (mouseSensitivity * 10f) * Time.deltaTime;

        //Get proper rotation value
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        playerTransform.Rotate(Vector3.up * mouseX);
        cameraTransform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }
}
