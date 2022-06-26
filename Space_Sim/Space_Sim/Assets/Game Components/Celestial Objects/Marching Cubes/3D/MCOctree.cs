using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
The Octree class maintains the whole tree of OctreeNodes, each chunk of the Marching body is its own octree
*/
public class MCOctree
{
    private MCOctreeNode rootNode;

    //Verts and Tris Lists
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    //Mesh
    private GameObject mesh;

    //Mesh Components
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    //Settings
    private float terrainScaler;
    private bool smoothTerrain;
    private bool flatShaded;

    //Terrain
    private float[,,] terrainMap;

    //Size
    private float radius;
    private int octreeSubdivions;
    private int cubeSegments;

//******************************************************************************************************************************
//                                                     Public Functions
//******************************************************************************************************************************
    public MCOctree(float radius, float terrainScaler, bool smoothTerrain, bool flatShaded, float[,,] terrainMap, Transform rootTransform, int octreeSubdivions)
    {
        //Set object variables to passed in variables
        this.radius = radius;
        this.terrainScaler = terrainScaler;
        this.smoothTerrain = smoothTerrain;
        this.flatShaded = flatShaded;
        this.terrainMap = terrainMap;
        this.octreeSubdivions = octreeSubdivions;
        this.cubeSegments = (int)(Mathf.Pow(2, octreeSubdivions));

        //Create Gameobject
        mesh = new GameObject();
        mesh.transform.SetParent(rootTransform);
        mesh.name = "mesh";
        mesh.transform.position = rootTransform.position;
        mesh.transform.rotation = rootTransform.rotation;

        //Create mesh gameobject componenets
        meshFilter = mesh.AddComponent<MeshFilter>();
        meshRenderer = mesh.AddComponent<MeshRenderer>();

        //Set mesh color
        mesh.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));

        //Create root node
        rootNode = new MCOctreeNode(radius * 2, rootTransform.position);
        DivideOctree(rootNode, octreeSubdivions);
    }

    //Keeps track of cube position and kicks off the rest
	public void GenerateMesh()
    {
        for (int x = 0; x < cubeSegments; x++)
        {
            for (int y = 0; y < cubeSegments; y++)
            {
                for (int z = 0; z < cubeSegments; z++)
                {
                    MarchCube(new Vector3Int(x, y, z));
                }
            }
        }
        BuildMesh();
    }

//******************************************************************************************************************************
//                                                     Private Functions
//******************************************************************************************************************************
    //***WARNING RECURSIVE*** Generates all of the needed children
    private void DivideOctree(MCOctreeNode node, int subdivionsRemaining)
    {
        if(subdivionsRemaining > 0)
        {
            subdivionsRemaining--;
            MCOctreeNode[] nodesChildren = node.GenerateChildren();
            for (int i = 0; i < 8; i++)
            {
                DivideOctree(nodesChildren[i], subdivionsRemaining);
            }
        }
    }

    private void MarchCube(Vector3Int marchPosition)
    {
        //Sample terrain values at each corner of the cube
		float[] cube = new float[8];
		for (int i = 0; i < 8; i++)
		{
			cube[i] = SampleTerrain(marchPosition + MarchingCubeData.CornerTable[i]);
		}

        int edgeIndex = 0;
        int configIndex = GetCubeConfiguration(cube);

        if(configIndex == 0 || configIndex == 255){return;}	//If all verts are on, or all verts are off, then just move on, since these wont display anything
    
        for (int i = 0; i < 5; i++) //Never more than 5 triangles per mesh
        {
            for (int j = 0; j < 3; j++) //3 Verts per triangle
            {
                int index = MarchingCubeData.TriangleTable[configIndex, edgeIndex];

                if(index == -1){return;} // If the current edgeIndex is -1, there are no more indices and we can exit the function

				//Get the vertices for the start and end of this edge.
                Vector3 vert1 = marchPosition + MarchingCubeData.CornerTable[MarchingCubeData.EdgeIndexes[index,0]];
                Vector3 vert2 = marchPosition + MarchingCubeData.CornerTable[MarchingCubeData.EdgeIndexes[index,1]];

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
					vertPosition = ConvertToWorldCoords(vert1 + ((vert2 - vert1) * difference));
				}
				else
				{
					//Get the midpoint of this edge. For non-smooth terrain just get edge midpoint
                	vertPosition = ConvertToWorldCoords((vert1 + vert2) / 2f);
				}

				//Flat shading
				//Add to our vertices and triangles list and incriment the edgeIndex
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
            if(cube[i] > terrainScaler)
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

	private Vector3 ConvertToWorldCoords(Vector3 vertPosition)
	{
        Vector3 centeredPosition = new Vector3(Remap(vertPosition.x,0f,(float)cubeSegments,((float)cubeSegments / 2f) * -1f,(float)cubeSegments / 2f), Remap(vertPosition.y,0f,(float)cubeSegments,((float)cubeSegments / 2f) * -1f,(float)cubeSegments / 2f), Remap(vertPosition.z,0f,(float)cubeSegments,((float)cubeSegments / 2f) * -1f,(float)cubeSegments / 2f));
		return ((radius * centeredPosition) / Mathf.Pow(2,(octreeSubdivions - 1)));
	}

//******************************************************************************************************************************
//                                                     Editor Functions
//******************************************************************************************************************************
    //Just for drawing the gizmo cubes
    public void DrawBounds(Transform rootTransform)
    {
        rootNode.DrawBounds(rootTransform);
    }
}