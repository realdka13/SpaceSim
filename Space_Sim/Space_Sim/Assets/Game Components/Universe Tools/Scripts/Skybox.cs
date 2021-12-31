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
    [SerializeField]
    private GameObject[] railBodyMeshes;    // TODO Sync this with Universe Manager

//******************************************************************************************************************************
//                                                     Private Functions
//******************************************************************************************************************************

    private void Awake()
    {
        universeManager = GameObject.FindGameObjectWithTag("Universe Manager").GetComponent<UniverseManager>();
    }

    private void Start()
    {
        //railedBodies = new GameObject[BodiesIndex];   // TODO this will be done automatically later, when prefabs of the planets exist
    }

    private void Update()
    {
        //Update Camera
        skyboxCamera.transform.rotation = universeManager.GetPlayerRotation();
        skyboxCamera.transform.localPosition = universeManager.GetPlayerCoords() / skyboxScale;

        //Update bodies
        for (int i = 0; i < railBodyMeshes.Length; i++)
        {
            Vector3 railBodyCoords = universeManager.GetBodyCoords(i);
            railBodyMeshes[i].transform.localPosition = railBodyCoords / skyboxScale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 1100f);
    }


//******************************************************************************************************************************
//                                                     Public Functions
//******************************************************************************************************************************

    public void EnableObject(bool enable, int bodyIndex)
    {
        if(enable)
        {
            railBodyMeshes[bodyIndex].SetActive(true);
        }
        else
        {
            railBodyMeshes[bodyIndex].SetActive(false);
        }
    }
}
