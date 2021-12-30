using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skybox : MonoBehaviour
{
    //Universe Manaegr
    private UniverseManager universeManager;

    //Children
    [SerializeField]
    private GameObject skyboxCamera;

    //Skybox
    [SerializeField]
    private float skyboxScale = 1f; //This number indicates how much bigger the skybox appears than its actual size

    //Bodies
    private Vector3[] railedBodyCoords;
    [SerializeField]
    private GameObject[] railedBodyMeshes;

//**********************************************************************************************************************

    private void Awake()
    {
        universeManager = GameObject.FindGameObjectWithTag("Universe Manager").GetComponent<UniverseManager>();
    }

    private void Start()
    {
        //Synchronize the body count arrays so that it only has to be changed in 1 location
        int BodiesIndex = universeManager.GetBodyCount();
        railedBodyCoords = new Vector3[BodiesIndex];
        //railedBodies = new GameObject[BodiesIndex];   // TODO this will be done automatically later, when prefabs of the planets exist
    }

    private void Update()
    {
        //Update Camera
        skyboxCamera.transform.rotation = universeManager.GetPlayerRotation();
        skyboxCamera.transform.localPosition = universeManager.GetPlayerCoords() / skyboxScale;

        //Update bodies
        for (int i = 0; i < railedBodyCoords.Length; i++)
        {
            railedBodyCoords[i] = universeManager.GetBodyCoords(i);
            railedBodyMeshes[i].transform.localPosition = railedBodyCoords[i];
        }
    }

    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 1100f);
    }
}
