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

    //Railed Bodies
    [SerializeField]
    private Vector3d[] railedBodyCoords;

//*****************************************************************************************************************************************************

    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        railedBodyCoords = new Vector3d[3]; //Improve this
    }

    void Update()
    {
        //Get player position
        playerUniverseCoords.Set((double)playerTransform.position.x + currentOrigin.x, (double)playerTransform.position.y + currentOrigin.y, (double)playerTransform.position.z + currentOrigin.z);
        for (int i = 0; i < railedBodyCoords.Length; i++)
        {
            railedBodyCoords[i] = new Vector3d(249.21d, 234d, 23.134d);
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
        return railedBodyCoords.Length;
    }

    public Vector3 GetBodyCoords(int bodyIndex)
    {
        return new Vector3((float)railedBodyCoords[bodyIndex].x, (float)railedBodyCoords[bodyIndex].y, (float)railedBodyCoords[bodyIndex].z);
    }
}
