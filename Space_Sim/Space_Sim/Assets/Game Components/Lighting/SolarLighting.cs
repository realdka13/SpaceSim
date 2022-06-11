using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarLighting : MonoBehaviour
{
    //Universe Manaegr
    private UniverseManager universeManager;

    private void Awake()
    {
        //Set default variables
        universeManager = GameObject.FindGameObjectWithTag("Universe Manager").GetComponent<UniverseManager>();   
    }

    private void Update()
    {
        //Set Directional light to shine in players direction
        transform.rotation = Quaternion.LookRotation(universeManager.GetPlayerCoords());
    }
}
