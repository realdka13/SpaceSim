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
    [SerializeField]
    private RailBody[] railBodies;

//*****************************************************************************************************************************************************

    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        for (int i = 0; i < railBodies.Length; i++)
        {
            railBodies[i].CalculateSemiConstants(10000000000000d); // TODO Will be upgraded to a universal timer, TODO change mass
        }
    }

    void Update()
    {
        //Get player position
        playerUniverseCoords.Set((double)playerTransform.position.x + currentOrigin.x, (double)playerTransform.position.y + currentOrigin.y, (double)playerTransform.position.z + currentOrigin.z);
        for (int i = 0; i < railBodies.Length; i++)
        {
            railBodies[i].CalculateCoordinates(Time.time); // TODO Will be upgraded to a universal timer
        }
    }

    private void OnValidate()   // TODO Later move this to in game debug?
    {
        //For Teleporting
        currentOrigin.Set(playerXCoord,playerYCoord,playerZCoord);
        for (int i = 0; i < railBodies.Length; i++)
        {
            railBodies[i].CalculateSemiConstants(10000000000000d); // TODO Will be upgraded to a universal timer, TODO change mass
        }
    }


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
