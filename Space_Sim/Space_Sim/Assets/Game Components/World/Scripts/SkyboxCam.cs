using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxCam : MonoBehaviour
{
    //Universe Manaegr
    private UniverseManager universeManager;

    //Skybox
    [SerializeField]
    private float skyboxScale = 1f; //This number indicates how much bigger the skybox appears than its actual size

    private void Awake()
    {
        universeManager = GameObject.FindGameObjectWithTag("Universe Manager").GetComponent<UniverseManager>();
    }

    private void Update()
    {
        //Rotation
        transform.rotation = universeManager.GetPlayerRotation();

        //Position
        transform.localPosition = universeManager.GetPlayerCoords() / skyboxScale;
    }
}
