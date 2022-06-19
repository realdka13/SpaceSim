using System;
using System.Collections;
using UnityEngine;

//TODO Remove Chunk Lines
//TODO Fix not sphere not being created when chunks arnt whole numbers                                                                                                                                              
//TODO Optimize
//TODO Fix Verts when flat shading?

//TODO LOD
//TODO Culling
//TODO Upgrade Interpolation code
//TODO Shows in editor/Actively changes
//TODO Remove terrainScaler/smooth terrain variables(needed)?

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
    private float chunkSize;

    //Chunk Objects
    private MarchingChunk[,,] chunks;

    //TEMP
    private MarchingChunk chunk;


//******************************************************************************************************************************
//                                                     Private Functions
//******************************************************************************************************************************
    IEnumerator Start() //*****Marching Cube debug cube*****
    //private void Start()
    {
        //Calculate size of each chunck based of the selected number of subdivisions
        chunkSize = (diameter / (chunkSubdivisions + 1));

        //Create Array to keep track of which chunk we are looking at
        float[] chunkOffset = new float[3];
        for (int i = 0; i < chunkOffset.Length; i++)
        {
            chunkOffset[i] = 0f;
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
                    yield return new WaitForSeconds(marchingDelay);
                    //*****Marching Cube debug*****

                    chunks[x, y, z] = new MarchingChunk(transform, diameter, terrainScaler, smoothTerrain, flatShaded, chunkSize, chunkOffset); //The actual chunk creation call

                    //*****Marching Cube debug*****
                    yield return new WaitForSeconds(marchingDelay);
                    //*****Marching Cube debug*****
                    
                    //Chunk Y Offset
                    chunkOffset[1] = chunkOffset[1] + chunkSize;
                }
                 //Chunk Z Offset
                 chunkOffset[1] = 0;
                 chunkOffset[2] = chunkOffset[2] + chunkSize;
            }
             //Chunk X Offset
             chunkOffset[1] = 0;
             chunkOffset[2] = 0;
             chunkOffset[0] = chunkOffset[0] + chunkSize;
        }
        
        //chunk = new MarchingChunk(transform, diameter, terrainScaler, smoothTerrain, flatShaded, chunkSize, chunkOffset);
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