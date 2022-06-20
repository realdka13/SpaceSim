using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO have all planet control be possible on this script
//TODO Better object management

public class UniverseManager : MonoBehaviour
{    
    //**********
    //trueOrigin is the "backend double origin",screenOrigin is the float origin in the editor
    //playerTrueCoords is the "backend double origin", playerScreenCoords is the float coordinates in the editor
    //**********

    //Origin
    private Vector3d trueOrigin;

    //Player
    private Transform playerTransform; //Also playerScreenCoords
    private Vector3d playerTrueCoords;

    //Origin Tracking
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
                                            //Distance to switch between real(on screen) and fake(scaled space) bodies

    //Skybox
    private Skybox skybox;

//******************************************************************************************************************************
//                                                     Private Functions
//******************************************************************************************************************************

    private void Awake()
    {
        //Set default variables
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        skybox = GameObject.FindGameObjectWithTag("Skybox").GetComponent<Skybox>();

        //Tell railbodies to calculate their constants to save processing later
        for (int i = 0; i < railBodies.Length; i++)
        {
            railBodies[i].CalculateSemiConstants();
        }
    }


    private void Update()
    {
        //Update playerTrueCoords every frame **This is not updating the origin**
        playerTrueCoords.Set((double)playerTransform.position.x + trueOrigin.x, (double)playerTransform.position.y + trueOrigin.y, (double)playerTransform.position.z + trueOrigin.z);
        
        //Update railBodies positions
        for (int i = 0; i < railBodies.Length; i++)
        {
            //Pass railBodies the current time
            railBodies[i].CalculateCoordinates(Time.time); //TODO Will be upgraded to a universal timer

            //If player is within distance threshold to planet, turn on real body and remove fake body
            if(Vector3d.Distance(playerTrueCoords, railBodies[i].GetCoordinates()) <= renderDistance)   //TODO remove distance calculation for optimization
            {                                                                                           //TODO separate UpdateRealBodyPosition
                railBodies[i].EnableObject(true);
                skybox.EnableObject(false, i);
                UpdateRealBodyPosition(i);
            }
            else//else, turn off real body and turn on fake body 
            {   //TODO dont do this every frame, check if body is already hidden
                skybox.EnableObject(true, i);
                railBodies[i].EnableObject(false);
            }
        }
    }

    private void OnValidate()   // TODO Later move this to in game debug?
    {
        //For Teleporting
        trueOrigin.Set(playerXCoord,playerYCoord,playerZCoord);
        for (int i = 0; i < railBodies.Length; i++)
        {
            railBodies[i].CalculateSemiConstants(); // TODO change mass
        }
    }

    //
    private void UpdateRealBodyPosition(int planetIndex)
    {
        Vector3d objectCoords = railBodies[planetIndex].GetCoordinates();
        Vector3 localPosition = new Vector3((float)(objectCoords.x - trueOrigin.x), (float)(objectCoords.y - trueOrigin.y), (float)(objectCoords.z - trueOrigin.z));
        railBodies[planetIndex].SetObjectLocalPosition(localPosition);
    }


//******************************************************************************************************************************
//                                                     Public Functions
//******************************************************************************************************************************

    //This function will update the trueOrigin position to use in calculating the players universe coords
    public void UpdateOrigin(Vector3 originOffset)
    {
        trueOrigin.Set(trueOrigin.x + (double)originOffset.x, trueOrigin.y + (double)originOffset.y, trueOrigin.z + (double)originOffset.z);
    }

    public Vector3 GetPlayerCoords()
    {
        return new Vector3((float)playerTrueCoords.x, (float)playerTrueCoords.y, (float)playerTrueCoords.z);
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
