using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxCam : MonoBehaviour
{
    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private float skyboxScale = 1f;

    private void Update()
    {
        transform.rotation = playerCamera.rotation;
        transform.localPosition = playerCamera.position / skyboxScale;
    }
}
