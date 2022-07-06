using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO LOD - Octree Marching cubes - Transvoxel
    //LOD
    //Transvoxel/Transvoxel-like algorithm
//TODO Culling - Law of cosines thing
    //Move Marching cube mesh creation (MarchCube function) to Octree node? Use as optimization of octree children??

//Need Chunks???

//TODO Collision Mesh
//TODO Modifiable Terrain - move to its own script?

//TODO Save terrain when loading/unloading

//TODO Move rendering to GPU

//TODO Shows (whole) body in editor/Actively changes
//TODO Decorate!

public class MCOOctreeBody : MonoBehaviour
{
    //Octrees
    [Header("Octrees")]
    [Range(1,6)]
    public int octreeSubdivions;
    private MCOctree mcOctree;

    //Body Settings
    [Header("Shape")]
    [Tooltip("Length of marching cube area. This is the radius of the body if the terrain scaler is set to 1")]
	public float radius;
    [Range(0f,1f)][Tooltip("Scale of terrain inside of marching cube area")]
    public float terrainScaler = .999f;

    [Header("Smoothness")]
	public bool smoothTerrain;
	public bool flatShaded;	//***WARNING, INCREASES VERTEX COUNT AS VERTICES GET DUPLICATED***

    //Terrain
    private float[,,] terrainMap;

    //Rendering
    [Header("Render - INACTIVE")]
    public bool culling;
    public float cullingAngle = 90f;

    //Gizmos
    [Header("Gizmos")]
    public bool drawAllCubes;
    public bool drawMainCube;

//******************************************************************************************************************************
//                                                     Public Functions
//******************************************************************************************************************************
public void PlaceTerrain(Vector3 pos)
{
    Vector3Int v3Int = new Vector3Int(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y), Mathf.CeilToInt(pos.z));
    //terrainMap[v3Int.x, v3Int.y, v3Int.z] = 0f;
    //mcOctree.GenerateMesh();
    Debug.Log("Regenerating Meshes");
}


//******************************************************************************************************************************
//                                                     Private Functions
//******************************************************************************************************************************
    private void Awake() 
    {
        //Create Terrain Map for WHOLE body, the size of terrain map depends on the LOD Level
        //SD:1 = 2^1; SD:2 = 2^2; SD:3 = 2^3;
        int cubeSegments = (int)(Mathf.Pow(2, octreeSubdivions));
        terrainMap = new float[cubeSegments + 1, cubeSegments + 1, cubeSegments + 1];
        PopulateTerrainMap(cubeSegments);

        //Create new marching Cubes Octree
        mcOctree = new MCOctree(radius, terrainScaler, smoothTerrain, flatShaded, terrainMap, this.transform, octreeSubdivions);
        
        //Generate Mesh
        mcOctree.GenerateMesh();
    }
    
    private void Start()
    {
        //Error handling (Checks if Size or Terrain Scaler is invalid and sends a warning)
        if(radius == 0){Debug.LogWarning("Radius = 0");}
        if(terrainScaler == 0){Debug.LogWarning("Terrain Scaler = 0");}
    }

    private void Update()
    {
        //Update Culling - TODO put in IEnumerator to not check every single frame
        //If culling is true, calculate
        //If culling false, and changeed last frame, calculate full mesh
        //Else do nothing
    }

    private void PopulateTerrainMap(int cubeSegments)
    {
        for (int x = 0; x < cubeSegments + 1; x++)
        {
            for (int z = 0; z < cubeSegments + 1; z++)
            {
                for (int y = 0; y < cubeSegments + 1; y++)
                {
                    //Get the percentage through the large mesh
                    float xPer = (float)x / (float)cubeSegments;
                    float yPer = (float)y / (float)cubeSegments;
                    float zPer = (float)z / (float)cubeSegments;

                    //Remap them to center the sphere correctly
                    xPer = Remap(xPer, 0f, 1f, -1f, 1f);
                    yPer = Remap(yPer, 0f, 1f, -1f, 1f);
                    zPer = Remap(zPer, 0f, 1f, -1f, 1f);

                    //Equation for a sphere
                    terrainMap[x, y, z] = xPer*xPer + yPer*yPer + zPer*zPer;
                    //Debug.Log("Remapped (" + x + ", " + y + ", " + z + ")\n" + "xPer: " + xPer + " yPer: " + yPer + " zPer: " + zPer + "\nFinal Value: " + terrainMap[x, y, z]);
                }
            }
        }
    }
//******************************************************************************************************************************
//                                                     Helper Functions
//******************************************************************************************************************************
    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    
//******************************************************************************************************************************
//                                                     Editor Functions
//******************************************************************************************************************************
    private void OnValidate()
    {
        //Clamps radius to positive numbers
        radius = Mathf.Clamp(radius, 0, float.MaxValue);
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying && drawAllCubes)
        {
            mcOctree.DrawBounds(this.transform);
        }
        Gizmos.color = Color.green;
        if(drawMainCube)
        {
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(radius * 2, radius * 2, radius * 2));
        }
    }
}