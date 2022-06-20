using System;
using System.Collections;
using UnityEngine;

//TODO Doesnt build at body location
//TODO Fix not sphere not being created when chunks arnt whole numbers                                                                                                                                              
//TODO Optimize
//TODO Fix Verts when flat shading?

//TODO LOD?
//TODO Culling
//TODO Upgrade Interpolation code
//TODO Shows in editor/Actively changes
//TODO Remove terrainScaler/smooth terrain variables(needed)?

//TODO Modifiable Terrain
//TODO Save terrain when loading/unloading

//TODO Decorate!

public class MarchingBody : MonoBehaviour
{
	//Settings
    [Header("Shape")]
    [Tooltip("Length of marching cube area. This is the diameter of the body if the terrain scaler is set to 1")]
	public int diameter;
    [Range(0f,1f)][Tooltip("Scale of terrain inside of marching cube area")]
    public float terrainScaler;

    [Header("Smoothness")]
	public bool smoothTerrain;
	public bool flatShaded;	//***WARNING, INCREASES VERTEX COUNT AS VERTICES GET DUPLICATED***

    [Header("Chunks")]
    [Range(0,5)][Tooltip("Chunk Subdivisions + 1 must be  multiple of diameter to render the full sphere")]
    public int chunkSubdivisions;
    public float marchingDelay;
    private float[,,] terrainMap;
    private float chunkSize;
    private int[] terrainStart;
    private int[] terrainEnd;
    

    //Chunk Objects
    private MarchingChunk[,,] chunks;

    //TEMP
    private MarchingChunk chunk;


//******************************************************************************************************************************
//                                                     Private Functions
//******************************************************************************************************************************
    //IEnumerator Start() //*****Marching Cube debug cube*****
    private void Awake()
    {
        //Create terrain map for body
        terrainMap = new float[diameter + 1, diameter + 1, diameter + 1];
        PopulateTerrainMap();

        //Calculate size of each chunck based of the selected number of subdivisions
        chunkSize = (diameter / (chunkSubdivisions + 1));

        //Telling the chunk where to look on the terrain map
        terrainStart = new int[3];
        terrainEnd = new int[3];
        for (int i = 0; i < terrainStart.Length; i++)
        {
            terrainStart[i] = 0;
            terrainEnd[i] = terrainStart[i] + (int)chunkSize;
        }

        //Set size of chunks array
        chunks = new MarchingChunk[chunkSubdivisions + 1, chunkSubdivisions + 1, chunkSubdivisions + 1];

        
        //Create Chunks
        for (int x = 0; x < chunkSubdivisions + 1; x++)
        {
            for (int z = 0; z < chunkSubdivisions + 1; z++)
            {
                for (int y = 0; y < chunkSubdivisions + 1; y++)
                {
                    //*****Marching Cube debug*****
                    //yield return new WaitForSeconds(marchingDelay);
                    //*****Marching Cube debug*****

                    chunk = new MarchingChunk(transform, diameter, terrainScaler, smoothTerrain, flatShaded, terrainMap, terrainStart, terrainEnd);

                    //*****Marching Cube debug*****
                    //yield return new WaitForSeconds(marchingDelay);
                    //*****Marching Cube debug*****
                    
                    //Chunk Y Offset
                    terrainStart[1] = terrainStart[1] + (int)chunkSize; // TODO fix this int cast
                    terrainEnd[1] = terrainStart[1] + (int)chunkSize;
                }
                //Chunk Z Offset
                terrainStart[1] = 0;
                terrainEnd[1] = terrainStart[1] + (int)chunkSize;
                terrainStart[2] = terrainStart[2] + (int)chunkSize; // TODO fix this int cast
                terrainEnd[2] = terrainStart[2] + (int)chunkSize;
            }
            //Chunk X Offset
            terrainStart[1] = 0;
            terrainEnd[1] = terrainStart[1] + (int)chunkSize;
            terrainStart[2] = 0;
            terrainEnd[2] = terrainStart[2] + (int)chunkSize;
            terrainStart[0] = terrainStart[0] + (int)chunkSize; // TODO fix this int cast
            terrainEnd[0] = terrainStart[0] + (int)chunkSize;
        }
    }

    private void PopulateTerrainMap()
    {
        for (int x = 0; x < diameter + 1; x++)
        {
            for (int z = 0; z < diameter + 1; z++)
            {
                for (int y = 0; y < diameter + 1; y++)
                {
                    //Get the percentage through the large mesh
                    float xPer = (float)x / (float)diameter;
                    float yPer = (float)y / (float)diameter;
                    float zPer = (float)z / (float)diameter;

                    //Remap them to center the sphere correctly
                    xPer = Remap(xPer, 0f, 1f, -1f, 1f);
                    yPer = Remap(yPer, 0f, 1f, -1f, 1f);
                    zPer = Remap(zPer, 0f, 1f, -1f, 1f);

                    //Equation for a sphere
                    terrainMap[x, y, z] = xPer*xPer + yPer*yPer + zPer*zPer;
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
    void OnDrawGizmosSelected()
    {
        // Draw a Black cube at the transform position
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, new Vector3(diameter, diameter, diameter));
    }
}