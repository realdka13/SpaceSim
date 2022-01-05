using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniverseManager : MonoBehaviour
{
    //Origin
    private Vector3d currentOrigin;

    //Player
    private Transform playerTransform;
    private Vector3d playerUniverseCoords;

    //For Teleporting
    [Space,Header("Teleport To:")]
    [SerializeField]
    private double playerXCoord;
    [SerializeField]
    private double playerYCoord;
    [SerializeField]
    private double playerZCoord;

    //Railed Bodies
    [Space,SerializeField]
    private RailBody[] railBodies;
    [SerializeField]
    private float renderDistance = 1000f;   // TODO sync with LOD 0/1 distance, since the basic mesh used by the skybox will be the LOD0 version

    //Skybox
    private Skybox skybox;

//******************************************************************************************************************************
//                                                     Private Functions
//******************************************************************************************************************************

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        skybox = GameObject.FindGameObjectWithTag("Skybox").GetComponent<Skybox>();
        for (int i = 0; i < railBodies.Length; i++)
        {
            railBodies[i].CalculateSemiConstants();
        }
    }


    private void Update()
    {
        //Get player position
        playerUniverseCoords.Set((double)playerTransform.position.x + currentOrigin.x, (double)playerTransform.position.y + currentOrigin.y, (double)playerTransform.position.z + currentOrigin.z);
        for (int i = 0; i < railBodies.Length; i++)
        {
            railBodies[i].CalculateCoordinates(Time.time); // TODO Will be upgraded to a universal timer

            if(Vector3d.Distance(playerUniverseCoords, railBodies[i].GetCoordinates()) <= renderDistance)   //TODO remove distance calculation for optimization
            {
                railBodies[i].EnableObject(true);
                skybox.EnableObject(false, i);
                RenderPlanet(i);
            }
            else
            {
                skybox.EnableObject(true, i);
                railBodies[i].EnableObject(false);
            }
        }
    }

    private void OnValidate()   // TODO Later move this to in game debug?
    {
        //For Teleporting
        currentOrigin.Set(playerXCoord,playerYCoord,playerZCoord);
        for (int i = 0; i < railBodies.Length; i++)
        {
            railBodies[i].CalculateSemiConstants(); // TODO change mass
        }
    }

    private void RenderPlanet(int planetIndex)
    {
        Vector3d objectCoords = railBodies[planetIndex].GetCoordinates();
        Vector3 localPosition = new Vector3((float)(objectCoords.x - currentOrigin.x), (float)(objectCoords.y - currentOrigin.y), (float)(objectCoords.z - currentOrigin.z));
        railBodies[planetIndex].SetObjectLocalPosition(localPosition);
    }


//******************************************************************************************************************************
//                                                     Public Functions
//******************************************************************************************************************************

    //This function will update the current origin position to use in calculating the players universe coords
    public void UpdateOrigin(Vector3 originOffset)
    {
        currentOrigin.Set(currentOrigin.x + (double)originOffset.x, currentOrigin.y + (double)originOffset.y, currentOrigin.z + (double)originOffset.z);
    }

    public Vector3 GetPlayerCoords()
    {
        return new Vector3((float)playerUniverseCoords.x, (float)playerUniverseCoords.y, (float)playerUniverseCoords.z);
    }

    public Quaternion GetPlayerRotation()
    {
        return playerTransform.rotation;
    }

    public GameObject GetBodyObjects(int bodyIndex)
    {
        return railBodies[bodyIndex].GetBodyObject();
    }

    public int GetBodyCount()
    {
        return railBodies.Length;
    }

    public Vector3 GetBodyCoords(int bodyIndex)
    {
        Vector3d railBodyCoords = railBodies[bodyIndex].GetCoordinates();
        return new Vector3((float)railBodyCoords.x, (float)railBodyCoords.y, (float)railBodyCoords.z);
    }
}