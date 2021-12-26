using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModifiedFloatingOrigin : MonoBehaviour
{
    public float distanceThreshold = 100f;
    private Scene currentScene;


    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
    }

    private void FixedUpdate()
    {
        Vector3 playerPosition = gameObject.transform.position;
        playerPosition.y = 0; // *** TEMP, find solution ***

        if (playerPosition.magnitude > distanceThreshold)
        {
            foreach (GameObject objects in currentScene.GetRootGameObjects())
            {
                objects.transform.position -= playerPosition;
            }
            Debug.Log("Origin Reset");
        }
    }
}