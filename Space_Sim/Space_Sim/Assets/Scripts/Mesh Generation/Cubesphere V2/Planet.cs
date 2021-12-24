using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Show Mesh in the Editor
//TODO Make LOD update more efficient? Like if player hasnt moved, dont bother checking every chunk
//TODO reduce culling angle as player gets closer to the planet
//TODO Save this data to a scriptable object
    //TODO Need to make sure there arnt too many meshes on the planet on properties reset


public class Planet : MonoBehaviour
{
    //Meshes
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    QuadTree[] tree;

    //Player Info
    [HideInInspector]
    public Transform player;
    //[HideInInspector]
    public float distancePlayerFromCenter;              //Used for culling math

    [Header("Planet parameters")]
    public float radius = 1000;                 //Size of the planet

    //LODs
    [Space, Header("LOD Parameters")]
    public float updateFrequency = .1f;         //Update frequency of LOD chunks
    public int LOD0Resolution = 9;              //Resolution of LOD0, reccomended odd, 11 is the max for up to LOD8
    public float cullingMinAngle = 90f; //Angle in Degrees from player to vertex to cull

    //LOD Levels
    [Space]
    public float[] detailLevelDistances = new float[] { //1 Square is subdivided into 4 at every LOD level
        Mathf.Infinity, //LOD0
        6000f,          //LOD1
        2500f,          //LOD2
        1000f,          //LOD3
        400f,           //LOD4
        150f,           //LOD5
        70f,            //LOD6
        30f,            //LOD7
        10f             //LOD8
    };

    public float[,] cullingAngles = {
    {Mathf.Infinity, 90f},
    {6000f, 80f}, 
    {2000f, 60f},
    {1500f, 45f},
    {1000, 30f},
    {500, 20f},
    {100f, 10f},
    };
    
//-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        Initialize();
        GenerateMesh();

        StartCoroutine(PlanetGenerationLoop());
    }

    private void Update()
    {

    }

    private IEnumerator PlanetGenerationLoop()
    {
        GenerateMesh();

        while(true)
        {
            yield return new WaitForSeconds(updateFrequency);

            distancePlayerFromCenter = Vector3.Distance(transform.position, player.position);
            float distancePlayerFromSurface = distancePlayerFromCenter - radius;
            for(int i = cullingAngles.GetLength(0) - 1; i >= 0; i--)
            {
                if(distancePlayerFromSurface < cullingAngles[i,0])
                {
                    cullingMinAngle = cullingAngles[i,1];
                    break;
                }
            }
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
        tree = new QuadTree[6];                                                                                        //Storage array for all terrain faces

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
            tree[i] = new QuadTree(meshFilters[i].sharedMesh, cardinalDirections[i], radius, this);
        }
    }
    
    
    //Generates a new mesh
    void GenerateMesh()
    {
        foreach (QuadTree branch in tree)
        {
            branch.ConstructTree();
        }
    }

    //Updates the current mesh
    void UpdateMesh()
    {
        foreach (QuadTree branch in tree)
        {
            branch.UpdateTree();
        }
    }
}