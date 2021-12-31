using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModifiedFloatingOrigin : MonoBehaviour
{
    //Universe
    private UniverseManager universeManager;

    public float distanceThreshold = 100f;
    private Scene currentScene;

    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
        universeManager = GameObject.FindGameObjectWithTag("Universe Manager").GetComponent<UniverseManager>();
    }

    private void FixedUpdate()
    {
        Vector3 playerPosition = gameObject.transform.position;

        if (playerPosition.magnitude > distanceThreshold)
        {
            foreach (GameObject objects in currentScene.GetRootGameObjects())
            {
                if(objects.layer != 7 && objects.layer != 8 && objects.layer != 9)   //7 is the 3D skybox layer, 8 is the universe manager, 9 is lighting it ignores it so it does not move
                {
                    objects.transform.position -= playerPosition;
                }
            }
            universeManager.UpdateOrigin(playerPosition);
        }
    }
}