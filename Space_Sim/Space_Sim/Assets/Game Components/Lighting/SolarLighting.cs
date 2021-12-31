using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarLighting : MonoBehaviour
{
    //Universe Manaegr
    private UniverseManager universeManager;

    // Start is called before the first frame update
    private void Awake()
    {
        universeManager = GameObject.FindGameObjectWithTag("Universe Manager").GetComponent<UniverseManager>();
        
    }

    // Update is called once per frame
    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(universeManager.GetPlayerCoords());
    }
}
