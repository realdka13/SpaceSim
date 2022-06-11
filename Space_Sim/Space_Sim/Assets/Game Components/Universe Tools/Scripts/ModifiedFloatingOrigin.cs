using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
Description:
    This scripts keeps track of the player position in the scene view, after crossing a certain threshold
    it moves the player back to 0,0,0 and moves every other object by the players displacement from the real
    origin when the move was was triggered
*/

public class ModifiedFloatingOrigin : MonoBehaviour
{
    //Universe
    private UniverseManager universeManager;    //Universe Manager Object

    [SerializeField]
    private float distanceThreshold = 100f;     //Distance from origin before reset
    private Scene currentScene;                 //Current Scene object -> used to get all objects in the scene

    private void Awake()
    {
        //Set Objects
        currentScene = SceneManager.GetActiveScene();
        universeManager = GameObject.FindGameObjectWithTag("Universe Manager").GetComponent<UniverseManager>();
    }

    private void FixedUpdate()
    {
        //Get players float position
        Vector3 playerPosition = gameObject.transform.position;

        //If players position is farther than the threshold
        if (playerPosition.magnitude > distanceThreshold)
        {
            //Get every object in the scene
            foreach (GameObject objects in currentScene.GetRootGameObjects())
            {
                //Check what layer the objects on and dont move objects on these layers
                //7 is the 3D skybox layer, 8 is the universe manager, 9 is lighting
                if(objects.layer != 7 && objects.layer != 8 && objects.layer != 9)
                {
                    //Move the rest of the objects
                    objects.transform.position -= playerPosition;
                }
            }
            //Update the true origin (true origin is the "backend double origin", screen origin is the float origin in the editor)
            universeManager.UpdateOrigin(playerPosition);
        }
    }
}