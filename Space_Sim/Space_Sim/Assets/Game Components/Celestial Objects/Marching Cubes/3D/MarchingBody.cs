using System;
using System.Collections;
using UnityEngine;

//TODO Culling
//TODO Shows (whole) body in editor/Actively changes

//TODO Collision Mesh
//TODO Modifiable Terrain
//TODO Save terrain when loading/unloading

//TODO LOD?

//TODO Decorate!

public class MarchingBody : MonoBehaviour
{
	//Settings
    [Header("Shape")]
    [Tooltip("Length of marching cube area. This is the radius of the body if the terrain scaler is set to 1")]
	public int radius;
    [Range(0f,.999f)][Tooltip("Scale of terrain inside of marching cube area")]
    public float terrainScaler;

    [Header("Smoothness")]
	public bool smoothTerrain;
	public bool flatShaded;	//***WARNING, INCREASES VERTEX COUNT AS VERTICES GET DUPLICATED***

    [Header("Chunks")]
    [Range(0,10)][Tooltip("Chunk Subdivisions + 1 must be a multiple of the DIAMETER to render the full sphere")]
    public int chunkSubdivisions;
    
    public float marchingDelay;
    private MarchingChunk[,,] chunks;

    //Terrain
    private float[,,] terrainMap;


//******************************************************************************************************************************
//                                                     Private Functions
//******************************************************************************************************************************
    private void Awake()
    {
        //Create terrain map for body
        terrainMap = new float[(radius * 2) + 1, (radius * 2) + 1, (radius * 2) + 1]; //(radius * 2) is Diameter
        PopulateTerrainMap(radius * 2);

        //Create Chunks
        //StartCoroutine(GenerateChunks()); //*****Marching Cube debug cube*****
        GenerateChunks();

        
    }

    private void PopulateTerrainMap(int diameter)
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

    //IEnumerator GenerateChunks() //*****Marching Cube debug cube*****
    private void GenerateChunks()
    {
        //Calculate size of each chunck based of the selected number of subdivisions
        float chunkSize = ((float)(radius * 2) / ((float)chunkSubdivisions + 1f));
        if(!Mathf.Approximately(chunkSize, Mathf.Round(chunkSize)))
        {
            Debug.LogWarning("Chunk Subdivisions + 1 must be a multiple of radius*2 (diameter) to render the full body correctly (chunkSize needs to be a whole number)\nChunk Size = " + chunkSize);
        }

        //Telling the chunk where to look on the terrain map
        int[] terrainStart = new int[3];
        int[] terrainEnd = new int[3];
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

                    chunks[x, y, z] = new MarchingChunk(transform, (radius * 2), terrainScaler, smoothTerrain, flatShaded, terrainMap, terrainStart, terrainEnd);

                    //*****Marching Cube debug*****
                    //yield return new WaitForSeconds(marchingDelay);
                    //*****Marching Cube debug*****
                    
                    //Chunk Y Offset
                    terrainStart[1] = terrainStart[1] + (int)chunkSize;
                    terrainEnd[1] = terrainStart[1] + (int)chunkSize;
                }
                //Chunk Z Offset
                terrainStart[1] = 0;
                terrainEnd[1] = terrainStart[1] + (int)chunkSize;
                terrainStart[2] = terrainStart[2] + (int)chunkSize;
                terrainEnd[2] = terrainStart[2] + (int)chunkSize;
            }
            //Chunk X Offset
            terrainStart[1] = 0;
            terrainEnd[1] = terrainStart[1] + (int)chunkSize;
            terrainStart[2] = 0;
            terrainEnd[2] = terrainStart[2] + (int)chunkSize;
            terrainStart[0] = terrainStart[0] + (int)chunkSize;
            terrainEnd[0] = terrainStart[0] + (int)chunkSize;
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
        Gizmos.matrix = this.transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(radius * 2, radius * 2, radius * 2));
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(Vector3.zero, radius);
    }
}