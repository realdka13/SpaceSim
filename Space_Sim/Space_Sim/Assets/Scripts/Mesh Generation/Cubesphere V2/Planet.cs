using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Show Mesh in the Editor

public class Planet : MonoBehaviour
{
    //Meshes
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    QuadTree[] tree;

    //Player Info
    [HideInInspector]
    public Transform player;
    [HideInInspector]
    public float distancePlayerFromCenter;      //Used for culling math
    private Vector3 previousPlayerPosition;     //Used to decide if the player has moved, if not, dont bother redrawing mesh
    private static float playerMoveTolerance = .5f;//Used to change the distance the player has to move before the mesh updates

    //Settings
    [HideInInspector]
    public bool planetSettingsFoldout;
    public PlanetSettings planetSettings;

    [HideInInspector]
    public bool lodSettingsFoldout;
    public LODSettings lodSettings;

    //Planet Parameters
    [HideInInspector]
    public float radius;                        //Size of the planet

    //LODs
    private float updateFrequency = .1f;         //Update frequency of LOD chunks
    [HideInInspector]
    public int LOD0Resolution = 9;              //Resolution of LOD0, reccomended odd, 11 is the max for up to LOD8
    [HideInInspector]
    public float cullingMinAngle = 90f;         //Angle in Degrees from player to vertex to cull

    //LOD Levels
    [Space,HideInInspector]
    public float[] detailLevelDistances;
    public float[,] cullingAngle;               //This currently cannot be edited per planet, and only in LODSettings.cs
    
//-----------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Awake() 
    {
        //Initialize Settings
        radius = planetSettings.radius;
        detailLevelDistances = lodSettings.detailLevelDistances;
        cullingAngle = lodSettings.cullingAngle;
    }

    private void Start()
    {
        //Find Player
        player = GameObject.FindGameObjectWithTag("Player").transform;

        //Create Mesh
        Initialize();
        GenerateMesh();

        //Start Checking for mesh updates
        StartCoroutine(PlanetGenerationLoop());
    }

    private IEnumerator PlanetGenerationLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(updateFrequency);

            //Check if player has even moved, if not, dont bother updating the mesh
            if (Vector3.Distance(previousPlayerPosition, player.position) >= playerMoveTolerance)
            {
                distancePlayerFromCenter = Vector3.Distance(transform.position, player.position);
                float distancePlayerFromSurface = distancePlayerFromCenter - radius;
                for(int i = cullingAngle.GetLength(0) - 1; i >= 0; i--)
                {
                    if(distancePlayerFromSurface < cullingAngle[i,0])
                    {
                        cullingMinAngle = cullingAngle[i,1];
                        break;
                    }
                }
                UpdateMesh();
                previousPlayerPosition = player.position;
            }
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

    public void GeneratePlanet()
    {
        radius = planetSettings.radius;
        detailLevelDistances = lodSettings.detailLevelDistances;
        cullingAngle = lodSettings.cullingAngle;
        Initialize();
        GenerateMesh();
    }
}