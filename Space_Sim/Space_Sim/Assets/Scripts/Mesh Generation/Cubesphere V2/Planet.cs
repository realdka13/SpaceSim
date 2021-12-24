using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Save this data to a scriptable object
public class Planet : MonoBehaviour
{
    //Meshes
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    //LODs
    public int startResolution = 9; // *** TODO Play with this and see what it does **
    public float radius = 1000; //Must be set to the size of the planet defined in the inspector *** TODO impliment this better/reflect changes in the editor***
    public float cullingMinAngle = 1.91986218f; //90 degrees for now *** TODO reduce this as the player gets closer to the planet surface ***

    //Player Info
    public Transform player;
    public float distanceToPlayer;

   //Hardcoded Detail Levels *** TODO add possibility to add more levels (have chunk auto get max)
    public float[] detailLevelDistances = new float[] {
        Mathf.Infinity,
        6000f,
        2500f,
        1000f,
        400f,
        150f,
        70f,
        30f,
        10f
    };
    
//-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;// Slow, but that doesn't really matter in this case *** TODO Improve this ***

        Initialize();
        GenerateMesh();

        StartCoroutine(PlanetGenerationLoop());
    }

    private void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
    }


    /* Only update the planet once per second
    Other possible improvements include:
    1: Only updating once the player has moved far enough to be able to cause a noticable change in the LOD */
    private IEnumerator PlanetGenerationLoop()
    {
        GenerateMesh();

        while(true)
        {
            yield return new WaitForSeconds(.1f);
            UpdateMesh();
        }
    }


    //This function just creates the necessary objects and assigns them, it does not actually calculate anything
    void Initialize()
    {
        if(meshFilters == null || meshFilters.Length == 0)                                                             //Only creates new mesh filters if none exist
        {
            meshFilters = new MeshFilter[6];                                                                           //Storage Array for Mesh Filters
        }
        terrainFaces = new TerrainFace[6];                                                                             //Storage array for all terrain faces

        Vector3[] cardinalDirections = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

        for (int i = 0; i < 6; i++)                                                                                    //Creating 6 Quads as Gameobjects
        {
            if(meshFilters[i] == null)                                                                                 //Only sets up new mesh filters if none exist
            {
                GameObject meshObj = new GameObject("mesh");                                                           //Creating an actual gameobject on screen
                meshObj.transform.parent = transform;                                                                  //Attach to a parent object

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));           //Add mesh renderer component, and assign a defailt material for now
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();                                                   //Add mesh filter component to meshObj, and save a reference to them in this array for easy access
                meshFilters[i].sharedMesh = new Mesh();                                                                //Initialize a mesh on the mesh filter components
            }

            //Calculate Meshes
            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, startResolution, cardinalDirections[i], radius, this);
        }
    }
    
    
    
    //Generates a new mesh
    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructTree();
        }
    }

    //Updates the current mesh
    void UpdateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.UpdateTree();
        }
    }
}