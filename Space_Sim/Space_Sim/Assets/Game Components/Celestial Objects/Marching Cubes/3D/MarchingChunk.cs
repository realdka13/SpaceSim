using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingChunk
{
	//Verts and Tris Lists
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

	//Chunk
	private GameObject chunkObj;
    private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;

	//Settings
	private int diameter;
    private float terrainScaler;

	private bool smoothTerrain;
	private bool flatShaded;	//***WARNING, INCREASES VERTEX COUNT AS VERTICES GET DUPLICATED

	private float chunkSize;
	private float[] chunkOffset;

	//Terrain
    private float[,,] terrainMap;

//******************************************************************************************************************************
//                                                     Public Functions
//******************************************************************************************************************************

	//Constructor
	public MarchingChunk(Transform parentTransform, int diameter, float terrainScaler, bool smoothTerrain, bool flatShaded, float chunkSize, float[] chunkOffset)
	{
		//Set object variables to passed in variables
		this.diameter = diameter;
		this.terrainScaler = terrainScaler;
		this.smoothTerrain = smoothTerrain;
		this.flatShaded = flatShaded;
		this.chunkSize = chunkSize;
		this.chunkOffset = chunkOffset;

		//Create chunks
		chunkObj = new GameObject();
		chunkObj.name = "chunk";
		chunkObj.transform.parent = parentTransform;

        meshFilter = chunkObj.AddComponent<MeshFilter>();
		meshRenderer = chunkObj.AddComponent<MeshRenderer>();

		chunkObj.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard")); 
        terrainMap = new float[diameter + 1, diameter + 1, diameter + 1];	//Create terrain map

		//March Cubes
        PopulateTerrainMap();
        CreateMeshData();
	}

//******************************************************************************************************************************
//                                                     Private Functions
//******************************************************************************************************************************

	//Iterates through the map calculates the map data
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
					if(x >= chunkOffset[0] && x <= (chunkOffset[0] + chunkSize))	//Check if x is out of range first, if it is, the rest will be
					{
						if(z >= chunkOffset[2] && z <= (chunkOffset[2] + chunkSize))
						{
							if(y >= chunkOffset[1] && y <= (chunkOffset[1] + chunkSize))
							{
								terrainMap[x, y, z] = xPer*xPer + yPer*yPer + zPer*zPer;
							}
						}
					}
					
					/*
					if(x >= 1)
					{
					terrainMap[x, y, z] = xPer*xPer + yPer*yPer + zPer*zPer;
					}
					else
					{
						terrainMap[x, y, z] = -1f;
					}
					*/
                }
            }
        }
    }

	//Keeps track of cube position and kicks off the rest
	private void CreateMeshData()
    {
        for (int x = 0; x < diameter; x++)
        {
            for (int y = 0; y < diameter; y++)
            {
                for (int z = 0; z < diameter; z++)
                {
                    MarchCube(new Vector3Int(x, y, z));
                }
            }
        }
        BuildMesh();
    }

	//Calculates the mesh data inside the current cube
	private void MarchCube(Vector3Int position)
    {
		//Sample terrain values at each corner of terrain
		float[] cube = new float[8];
		for (int i = 0; i < 8; i++)
		{
			cube[i] = SampleTerrain(position + MarchingCubeData.CornerTable[i]);
		}
		
        int configIndex = GetCubeConfiguration(cube);

        if(configIndex == 0 || configIndex == 255){return;}	//If all verts are on, or al verts are off, then just move on, since these wont display anything

        int edgeIndex = 0;
        for(int i = 0; i < 5; i++)  //Never more than 5 triangles per mesh
        {
            for (int j = 0; j < 3; j++) //3 points per triangle
            {
                int index = MarchingCubeData.TriangleTable[configIndex, edgeIndex];

                if(index == -1){return;} // If the current edgeIndex is -1, there are no more indices and we can exit the function

				//Get the vertices for the start and end of this edge.
                Vector3 vert1 = position + MarchingCubeData.CornerTable[MarchingCubeData.EdgeIndexes[index,0]];
                Vector3 vert2 = position + MarchingCubeData.CornerTable[MarchingCubeData.EdgeIndexes[index,1]];


				//Smooth Terrain
				Vector3 vertPosition;
				if(smoothTerrain)
				{
					//Get the terrain values of the edge vertices
					float vert1Value = cube[MarchingCubeData.EdgeIndexes[index,0]];
					float vert2Value = cube[MarchingCubeData.EdgeIndexes[index,1]];

					//Calculate difference between terrain values
					float difference = vert2Value - vert1Value;

					if(difference == 0)	//Check if terrain passes through midpoint
					{
						difference = terrainScaler;
					}
					else
					{
						difference = (terrainScaler - vert1Value) / difference;
					}

					//Calculate point on edge where terrain passes
					vertPosition = CenterVerts(vert1 + ((vert2 - vert1) * difference));
				}
				else
				{
					//Get the midpoint of this edge. For non-smooth terrain just get edge midpoint
                	vertPosition = CenterVerts((vert1 + vert2) / 2f);
				}

				//Flat shading
				//Add to our vertices and triangles list and incremement the edgeIndex.
				if(flatShaded)
				{
                	vertices.Add(vertPosition);
                	triangles.Add(vertices.Count - 1);
				}
				else
				{
					triangles.Add(VertForIndex(vertPosition));
				}
                edgeIndex ++;
            }
        }
    }

	//Constructs the visable mesh
    private void BuildMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

	//Figure out which cube mesh configuration to use, cube[] are each vertex of the cube
    private int GetCubeConfiguration(float[] cube)
    {
        int configurationIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if(cube[i] > terrainScaler)	// TODO This needed?
            {
                configurationIndex |= 1 << i;
            }
        }
        return configurationIndex;
    }

	private int VertForIndex(Vector3 vert)
	{
		//Loop through all vertices in the vert list
		for (int i = 0; i < vertices.Count; i++)
		{
			if(vertices[i] == vert)	//If a vert in the vertex array matches vert, then return the index
			{
				return i;
			}
		}

		//Else if no existing vertex is found, add it to the array and return its index
		vertices.Add(vert);
		return vertices.Count - 1;
	}

	private void ClearMeshData()
    {
        vertices.Clear();
        triangles.Clear();
    }

//******************************************************************************************************************************
//                                                     Helper Functions
//******************************************************************************************************************************

	private float SampleTerrain(Vector3Int point)
	{
		return terrainMap[point.x, point.y, point.z];
	}

	private float Remap(float value, float from1, float to1, float from2, float to2)
	{
    	return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	private Vector3 CenterVerts(Vector3 vertPosition)
	{
		return new Vector3(Remap(vertPosition.x,0f,(float)diameter,((float)diameter / 2f) * -1f,(float)diameter / 2f), Remap(vertPosition.y,0f,(float)diameter,((float)diameter / 2f) * -1f,(float)diameter / 2f), Remap(vertPosition.z,0f,(float)diameter,((float)diameter / 2f) * -1f,(float)diameter / 2f));
	}
}