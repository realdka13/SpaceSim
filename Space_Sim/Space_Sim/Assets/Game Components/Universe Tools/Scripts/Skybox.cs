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
    private GameObject[] railBodyMeshes;

//******************************************************************************************************************************
//                                                     Private Functions
//******************************************************************************************************************************

    private void Awake()
    {
        //Set default variables
        universeManager = GameObject.FindGameObjectWithTag("Universe Manager").GetComponent<UniverseManager>();
        railBodyMeshes = new GameObject[universeManager.GetBodyCount()];
    }

    private void Start()
    {
        for(int i = 0; i < railBodyMeshes.Length; i++)
        {
            //Create scaled bodies
            railBodyMeshes[i] = Instantiate(universeManager.GetBodyObjects(i), transform);
            railBodyMeshes[i].layer = 7; //Set bodies to skybox layer (7) Skybox layer
            foreach(Transform t in railBodyMeshes[i].transform)
            {
                t.gameObject.layer = 7;
            }
            //Set positions of bodies
            railBodyMeshes[i].transform.localScale = Vector3.one / skyboxScale;
        }
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

    //Enable or disable the body
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
