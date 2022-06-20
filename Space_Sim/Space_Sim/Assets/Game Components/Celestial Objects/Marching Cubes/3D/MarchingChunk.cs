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

	//Terrain
    private float[,,] terrainMap;
	private int[] terrainStart;
	private int[] terrainEnd;

//******************************************************************************************************************************
//                                                     Public Functions
//******************************************************************************************************************************

	//Constructor
	public MarchingChunk(Transform parentTransform, int diameter, float terrainScaler, bool smoothTerrain, bool flatShaded, float[,,] terrainMap, int[] terrainStart, int[] terrainEnd)
	{
		//Set object variables to passed in variables
		this.diameter = diameter;
		this.terrainScaler = terrainScaler;
		this.smoothTerrain = smoothTerrain;
		this.flatShaded = flatShaded;
		this.terrainMap = terrainMap;
		this.terrainStart = terrainStart;
		this.terrainEnd = terrainEnd;

		//Create chunk gameobject
		chunkObj = new GameObject();
		chunkObj.name = "chunk";
		chunkObj.transform.parent = parentTransform;

		//create chunk gameobject components
        meshFilter = chunkObj.AddComponent<MeshFilter>();
		meshRenderer = chunkObj.AddComponent<MeshRenderer>();

		//Set chunk color
		chunkObj.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard")); 

        CreateMeshData();
	}

//******************************************************************************************************************************
//                                                     Private Functions
//******************************************************************************************************************************

	//Keeps track of cube position and kicks off the rest
	private void CreateMeshData()
    {
        for (int x = terrainStart[0]; x < terrainEnd[0]; x++) //TODO change diameter to whatever is passed in
        {
            for (int y = terrainStart[1]; y < terrainEnd[1]; y++)
            {
                for (int z = terrainStart[2]; z < terrainEnd[2]; z++)
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

	private Vector3 CenterVerts(Vector3 vertPosition) //TODO diameter needed here??
	{
		return new Vector3(Remap(vertPosition.x,0f,(float)diameter,((float)diameter / 2f) * -1f,(float)diameter / 2f), Remap(vertPosition.y,0f,(float)diameter,((float)diameter / 2f) * -1f,(float)diameter / 2f), Remap(vertPosition.z,0f,(float)diameter,((float)diameter / 2f) * -1f,(float)diameter / 2f));
	}
}