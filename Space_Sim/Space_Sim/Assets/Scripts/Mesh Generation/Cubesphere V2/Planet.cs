using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
//Mesh is the data, set of vertices
//Mesh filter stores the mesh data
//Mesh renderer actually draws it on the screen

    [SerializeField, HideInInspector]   //Will make sure the mesh filters are saved in the editor
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    //For LOD
    public static float size = 10; // Must be set to the size of the planet defined in the inspector

    public static Transform player;

    /*
    LOD detail levels, modify this to figure out what works
    {LODLevel, player distance}
    */
    public static Dictionary<int, float> detailLevelDistances = new Dictionary<int, float>()
    {
        {0, Mathf.Infinity},
        {1, 60f},
        {2, 25f},
        {3, 10f},
        {4, 4f},
        {5, 1.5f},
        {6, .7f},
        {7, .3f},
        {8, .1f}
    };
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Slow, but that doesn't really matter in this case

        Initialize();
        GenerateMesh();

        StartCoroutine(PlanetGenerationLoop());
    }


    /* Only update the planet once per second
    Other possible improvements include:
    1: Only updating once the player has moved far enough to be able to cause a noticable change in the LOD
    2: Only displaying chunks that are in sight
    3: Not recreating chunks that already exist */
    private IEnumerator PlanetGenerationLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            GenerateMesh();
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
            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, 4, cardinalDirections[i], size);
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

}
