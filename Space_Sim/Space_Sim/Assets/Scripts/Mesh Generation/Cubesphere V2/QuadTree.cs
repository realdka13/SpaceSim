using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    Mesh mesh;

    int resolution;
    float radius;

    //Axes https://academo.org/demos/3d-vector-plotter/ to visualize
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    //Objects
    Chunk parentChunk;
    public Planet planetScript;

    //Verts and Tris
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();

//------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //Constructor *** TODO radius and planetscript.radius is redundant? ***
    public QuadTree(Mesh mesh, Vector3 localUp, float radius, Planet planetScript)
    {
        this.mesh = mesh;
        this.localUp = localUp;
        this.radius = radius;
        this.planetScript = planetScript;

        //Find axis A and B
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }



    //This method Constructs a Quad tree for use with the LOD system
    public void ConstructTree()
    {
        //Reset Verts and tris
        vertices.Clear();
        triangles.Clear();

        //Generate Chunks
        parentChunk = new Chunk(planetScript, null, null, localUp.normalized * planetScript.radius, radius, 0, localUp, axisA, axisB);  //Create parent chunk from Terrain face
        if(planetScript.enableLOD)
        {
            parentChunk.GenerateChildren();
        }

        //Get chunk mesh data
        if(parentChunk.children != null)
        {
            int triangleIndex = 0;
            foreach (Chunk child in parentChunk.GetVisableChildren())
            {
                (Vector3[], int[]) verticesAndTriangles = child.CalculateVerticesAndTriangles(triangleIndex);      //Asks for vertex info from only visable chunks
                vertices.AddRange(verticesAndTriangles.Item1);
                triangles.AddRange(verticesAndTriangles.Item2);
                triangleIndex += verticesAndTriangles.Item1.Length;
            }
        }
        else
        {
            (Vector3[], int[]) verticesAndTriangles = parentChunk.CalculateVerticesAndTriangles(0);      //Asks for vertex info from only visable chunks
            vertices.AddRange(verticesAndTriangles.Item1);
            triangles.AddRange(verticesAndTriangles.Item2);
        }

        //Reset Mesh and apply new data
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }


    //This method updates the Quad Tree on the the LOD system
    public void UpdateTree()
    {
        //Reset Mesh
        vertices.Clear();
        triangles.Clear();

        parentChunk.UpdateChunk();

        //Get chunk mesh data
        int triangleIndex = 0;
        foreach (Chunk child in parentChunk.GetVisableChildren())
        {
            (Vector3[], int[]) verticesAndTriangles = (new Vector3[0], new int[0]);
            if(child.vertices == null)
            {
                verticesAndTriangles = child.CalculateVerticesAndTriangles(triangleIndex);
            }
            else if (child.vertices.Length == 0)
            {
                verticesAndTriangles = child.CalculateVerticesAndTriangles(triangleIndex);
            }
            else
            {
                verticesAndTriangles = (child.vertices, child.GetTrianglesWithOffset(triangleIndex));
            }

            vertices.AddRange(verticesAndTriangles.Item1);
            triangles.AddRange(verticesAndTriangles.Item2);
            triangleIndex += verticesAndTriangles.Item1.Length;
        }

        // Reset mesh and apply new data
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

}