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

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //Get player position
        playerUniverseCoords.Set((double)playerTransform.position.x + currentOrigin.x, (double)playerTransform.position.y + currentOrigin.y, (double)playerTransform.position.z + currentOrigin.z);
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
}
