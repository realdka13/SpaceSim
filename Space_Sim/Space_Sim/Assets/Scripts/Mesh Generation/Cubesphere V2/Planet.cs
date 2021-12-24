using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField, HideInInspector]   //Will make sure the mesh filters are saved in the editor
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    //For LOD
    public int startResolution = 9;
    public float size = 1000; // Must be set to the size of the planet defined in the inspector *** TODO impliment this better***
    public float cullingMinAngle = 1.91986218f; //90 degrees for now *** TODO reduce this as the player gets closer to the planet surface ***

    //Player details
    public Transform player;
    public float distanceToPlayer;

   // Hardcoded detail levels. First value is level, second is distance from player. Finding the right values can be a little tricky
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
    
    



    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Slow, but that doesn't really matter in this case

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
    1: Only updating once the player has moved far enough to be able to cause a noticable change in the LOD
    2: Only displaying chunks that are in sight
    3: Not recreating chunks that already exist */
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
        if(meshFilters == null || meshFilters.Length == 0)//Only creates new mesh filters if none exist
        {
            meshFilters = new MeshFilter[6]; //Storage Array for Mesh Filters
        }
        terrainFaces = new TerrainFace[6];   //Storage array for all terrain faces

        Vector3[] cardinalDirections = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

        for (int i = 0; i < 6; i++) //Creating 6 Quads as Gameobjects
        {
            if(meshFilters[i] == null)//Only sets up new mesh filters if none exist
            {
                GameObject meshObj = new GameObject("mesh");    //Creating an actual gameobject on screen
                meshObj.transform.parent = transform;           //Attach to a parent object

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));           //Add mesh renderer component, and assign a defailt material for now
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();    //Add mesh filter component to meshObj, and save a reference to them in this array for easy access
                meshFilters[i].sharedMesh = new Mesh();                 //Initialize a mesh on the mesh filter components
            }

            //Calculate Meshes
            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, startResolution, cardinalDirections[i], size, this);
        }
    }
    
    
    
    
    // Generates the mesh. The generation is done from scratch every time it's called, which could be improved
    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructTree();
        }
    }


    void UpdateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.UpdateTree();
        }
    }
}