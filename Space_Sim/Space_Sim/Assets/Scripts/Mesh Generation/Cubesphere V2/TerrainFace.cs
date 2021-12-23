using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This creates a single big, subdividable Quad, it can be combined later with 6 others to make a cube
*/
public class TerrainFace
{
    Mesh mesh;

    int resolution;
    float radius;

    //Axes https://academo.org/demos/3d-vector-plotter/ to visualize
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    //Verts and Tris
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();

    //Constructor
    public TerrainFace(Mesh mesh, int resolution, Vector3 localUp, float radius)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        this.radius = radius;

        //Set find axis A and B
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
        Chunk parentChunk = new Chunk(null, null, localUp.normalized * Planet.size, radius, 0, localUp, axisA, axisB);  //Create parent chunk from Terrain face
        parentChunk.GenerateChildren();

        //Get chunk mesh data
        int triangleOffset = 0; // *** TODO add note for this ***
        foreach (Chunk child in parentChunk.GetVisableChildren())
        {
            (Vector3[], int[]) verticesAndTriangles = child.CalculateVerticesAndTriangles(triangleOffset);
            vertices.AddRange(verticesAndTriangles.Item1);
            triangles.AddRange(verticesAndTriangles.Item2);
            triangleOffset += verticesAndTriangles.Item1.Length;
        }

        //Reset Mesh and apply new data
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}













//*** TODO move this to different script? ***
public class Chunk
{
    public Chunk parent;
    public Chunk[] children;

    public Vector3 position;
    public float radius;
    public int detailLevel;

    public Vector3 localUp;
    public Vector3 axisA;
    public Vector3 axisB;

    // Constructor
    public Chunk(Chunk[] children, Chunk parent, Vector3 position, float radius, int detailLevel, Vector3 localUp, Vector3 axisA, Vector3 axisB)
    {
        this.children = children;
        this.parent = parent; // Unused
        this.position = position;
        this.radius = radius;
        this.detailLevel = detailLevel;
        this.localUp = localUp;
        this.axisA = axisA;
        this.axisB = axisB;
    }


    public void GenerateChildren()
    {
        //If detail level is under max level and above 0. Max level is defined in Planets *** TODO change max detail level hard coding ***
        if(detailLevel <= 8 && detailLevel >= 0)
        {
            if(Vector3.Distance(position.normalized * Planet.size, Planet.player.position) <= Planet.detailLevelDistances[detailLevel])
            {
                //Assign the chunks children (but not grandchildren)
                //Position is calculated on a cube and base on the fact that each child has 1/2 the radius of the parent
                //Detail level is increased by 1. This doesnt change anything itself, but rather symbolizes something HAS been change (the detail)
                children = new Chunk[4];
                children[0] = new Chunk(new Chunk[0], this, position + axisA * radius / 2 + axisB * radius / 2, radius / 2, detailLevel + 1, localUp, axisA, axisB);
                children[1] = new Chunk(new Chunk[0], this, position + axisA * radius / 2 - axisB * radius / 2, radius / 2, detailLevel + 1, localUp, axisA, axisB);
                children[2] = new Chunk(new Chunk[0], this, position - axisA * radius / 2 + axisB * radius / 2, radius / 2, detailLevel + 1, localUp, axisA, axisB);
                children[3] = new Chunk(new Chunk[0], this, position - axisA * radius / 2 - axisB * radius / 2, radius / 2, detailLevel + 1, localUp, axisA, axisB);

                //Create grandchildren
                foreach (Chunk child in children)
                {
                    child.GenerateChildren();
                }
            }
        }
    }


    //Returns the latest chunnk in every branch, aka the ones to be rendered
    public Chunk[] GetVisableChildren()
    {
        List<Chunk> toBeRendered = new List<Chunk>();

        if(children.Length > 0)
        {
            foreach (Chunk child in children)
            {
                toBeRendered.AddRange(child.GetVisableChildren());
            }
        }
        else
        {
            toBeRendered.Add(this);
        }

        return toBeRendered.ToArray();
    }


    //This method calculates the verts and tris
    public (Vector3[], int[]) CalculateVerticesAndTriangles(int triangleOffset)
    {
        int resolution = 8; //Chunk resolution *** TODO Get this on a slider? ***

        //Vert and Tri arrays
        Vector3[] vertices = new Vector3[resolution * resolution]; //Total verts = resolution^2
        int[] triangles = new int[((resolution - 1) * (resolution - 1)) * 6]; //Think about this array as triangle vertex Indecies. (res - 1)^2 is number of faces, * 2 triangles per face, * 3 vertices per triangle

        int triIndex = 0;  //Keeps track of where we are in the triangle array

        //Iterate through x and y
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                //Calculate Vertices
                int i = x + y * resolution; //Keeps track of which index we are on in the vertices array
                Vector2 percent = new Vector2(x, y) / (resolution - 1); //Tells us how close to completion each side is, can be used to decide position of each vertex

                //2 Big differences here *** TODO understand this ***
                //1: The origin is the position variable rather than the middle of the terrain face
                //2: the offset is scaled using the radius variable
                Vector3 pointOnUnitCube = position + ((percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB) * radius;

                /***Convert to Sphere*** https://catlikecoding.com/unity/tutorials/cube-sphere/
                float px = pointOnUnitCube.x;
                float py = pointOnUnitCube.y;
                float pz = pointOnUnitCube.z;

                Vector3 pointOnUnitSphere = new Vector3();
                pointOnUnitSphere.x = px * Mathf.Sqrt(1f - ((py*py)/2) - ((pz*pz)/2) + (((py*py)*(pz*pz))/3));
                pointOnUnitSphere.y = py * Mathf.Sqrt(1f - ((px*px)/2) - ((pz*pz)/2) + ((px*px)*(pz*pz)/3));
                pointOnUnitSphere.z = pz * Mathf.Sqrt(1f - ((px*px)/2) - ((py*py)/2) + ((px*px)*(py*py)/3));
                //***********************/

                //*** TODO change to my vert distribution
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized * Planet.size;

                vertices[i] = pointOnUnitSphere;

                //Create Triangles
                if(x != resolution - 1 && y != resolution - 1)  //Making sure we are trying to draw triangles over the edges (no vertices exist there)
                {
                    //*****In each Quad*****
                    //Tri 1
                    triangles[triIndex] = i + triangleOffset;
                    triangles[triIndex + 1] = i + resolution + 1 + triangleOffset;
                    triangles[triIndex + 2] = i + resolution + triangleOffset;

                    //Tri 2
                    triangles[triIndex + 3] = i + triangleOffset;
                    triangles[triIndex + 4] = i + 1 + triangleOffset;
                    triangles[triIndex + 5] = i + resolution + 1 + triangleOffset;
                    //**********************

                    triIndex += 6; //We created 6 vertices
                }
            }
        }
        return (vertices, triangles);
    }

}