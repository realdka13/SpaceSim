using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    //Planet
    public Planet planetScript;

    //Planet Details
    public Vector3 position;
    public float radius;

    //LOD
    public int detailLevel;
    int maxDetail;
    int resolution;

    //Objects
    public Chunk parent; //Unused
    public Chunk[] children;

    //Axes https://academo.org/demos/3d-vector-plotter/ to visualize
    public Vector3 localUp;
    public Vector3 axisA;
    public Vector3 axisB;

    //Verts and Tris
    public Vector3[] vertices;
    public int[] triangles;

//------------------------------------------------------------------------------------------------------------------------------------------------------------------

    // Constructor
    public Chunk(Planet planetScript, Chunk[] children, Chunk parent, Vector3 position, float radius, int detailLevel, Vector3 localUp, Vector3 axisA, Vector3 axisB)
    {
        //Planet
        this.planetScript = planetScript;

        //Planet Details
        this.position = position;
        this.radius = radius;

        //LOD
        this.detailLevel = detailLevel;
        this.maxDetail = planetScript.detailLevelDistances.Length;
        this.resolution = planetScript.LOD0Resolution;

        //Objects
        this.parent = parent;   //Unused
        this.children = children;

        //Find axis A and B
        this.localUp = localUp;
        this.axisA = axisA;
        this.axisB = axisB;
    }


    public void GenerateChildren()
    {
        //If detail level is under max level and above 0. Max level is defined in Planets
        if(detailLevel <= maxDetail - 1 && detailLevel >= 0)
        {
            if (Vector3.Distance(planetScript.transform.TransformDirection(position.normalized * planetScript.radius), planetScript.player.position) <= planetScript.detailLevelDistances[detailLevel])
            {
                //Assign the chunks children (but not grandchildren)
                //Position is calculated on a cube and base on the fact that each child has 1/2 the radius of the parent
                //Detail level is increased by 1. This doesnt change anything itself, but rather symbolizes something HAS been change (the detail)
                children = new Chunk[4];
                children[0] = new Chunk(planetScript, new Chunk[0], this, position + axisA * radius / 2 + axisB * radius / 2, radius / 2, detailLevel + 1, localUp, axisA, axisB);
                children[1] = new Chunk(planetScript, new Chunk[0], this, position + axisA * radius / 2 - axisB * radius / 2, radius / 2, detailLevel + 1, localUp, axisA, axisB);
                children[2] = new Chunk(planetScript, new Chunk[0], this, position - axisA * radius / 2 + axisB * radius / 2, radius / 2, detailLevel + 1, localUp, axisA, axisB);
                children[3] = new Chunk(planetScript, new Chunk[0], this, position - axisA * radius / 2 - axisB * radius / 2, radius / 2, detailLevel + 1, localUp, axisA, axisB);

                //Create grandchildren
                foreach (Chunk child in children)
                {
                    child.GenerateChildren();
                }
            }
        }
    }



    public void UpdateChunk()
    {
        float distancePlayerFromCenter = Vector3.Distance(planetScript.transform.TransformDirection(position.normalized * planetScript.radius), planetScript.player.position);
        if(detailLevel <= maxDetail - 1)
        {
            if (distancePlayerFromCenter > planetScript.detailLevelDistances[detailLevel])
            {
                children = new Chunk[0];
            }
            else
            {
                if(children.Length > 0)
                {
                    foreach (Chunk child in children)
                    {
                        child.UpdateChunk();
                    }
                }
                else
                {
                    GenerateChildren();
                }
            }
        }
    }



    //Returns the latest chunk in every branch, aka the ones to be rendered
    public Chunk[] GetVisableChildren()
    {
        List<Chunk> toBeRendered = new List<Chunk>();

        if(children.Length > 0)                 //If the chunk has children, it adds it to toBeRendered
        {
            foreach (Chunk child in children)
            {
                toBeRendered.AddRange(child.GetVisableChildren());
            }
        }
        else                                    //If chunk doesn't have children, decide if it should even be visable
        {
            float playerVertexAngle = Mathf.Acos((Mathf.Pow(planetScript.radius, 2) + Mathf.Pow(planetScript.distancePlayerFromCenter, 2) - Mathf.Pow(Vector3.Distance(planetScript.transform.TransformDirection(position.normalized * planetScript.radius), planetScript.player.position), 2)) / (2 * planetScript.radius * planetScript.distancePlayerFromCenter));
            if (playerVertexAngle < planetScript.currentCullingAngle * Mathf.Deg2Rad)
            {
                toBeRendered.Add(this);
            }
        }

        return toBeRendered.ToArray();
    }


    //Return triangles including offset
    public int[] GetTrianglesWithOffset(int triangleOffset)
    {
        int[] triangles = new int[this.triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            triangles[i] = this.triangles[i] + triangleOffset;
        }
        return triangles;
    }


    //This method calculates the verts and tris
    public (Vector3[], int[]) CalculateVerticesAndTriangles(int triangleOffset)
    {
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

                //2 Big differences here
                //1: The origin is the position variable rather than the middle of the terrain face
                //2: the offset is scaled using the radius variable
                Vector3 pointOnUnitCube = position + ((percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB) * radius;

                /*** TODO Reimplement this math/is it needed? ***
                //***Convert to Sphere***
                https://catlikecoding.com/unity/tutorials/cube-sphere/

                float px = pointOnUnitCube.x;
                float py = pointOnUnitCube.y;
                float pz = pointOnUnitCube.z;

                Vector3 pointOnUnitSphere = new Vector3();
                pointOnUnitSphere.x = px * Mathf.Sqrt(1f - ((py*py)/2) - ((pz*pz)/2) + (((py*py)*(pz*pz))/3));
                pointOnUnitSphere.y = py * Mathf.Sqrt(1f - ((px*px)/2) - ((pz*pz)/2) + ((px*px)*(pz*pz)/3));
                pointOnUnitSphere.z = pz * Mathf.Sqrt(1f - ((px*px)/2) - ((py*py)/2) + ((px*px)*(py*py)/3));

                //pointOnUnitSphere = pointOnUnitSphere * planetScript.radius;
                //***********************/

                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized * planetScript.radius; // Inflate the cube by projecting the vertices onto a sphere with the radius of Planet.radius

                vertices[i] = pointOnUnitSphere;

                //Create Triangles
                if(x < resolution - 1 && y < resolution - 1)  //Making sure we are trying to draw triangles over the edges (no vertices exist there)
                {
                    //*****In each Quad*****
                    //Tri 1
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    //Tri 2
                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    //**********************

                    triIndex += 6; //We created 6 vertices
                }
            }
        }

        // Store the vertices and triangles
        this.vertices = vertices;
        this.triangles = triangles;

        return (vertices, GetTrianglesWithOffset(triangleOffset));
    }

}