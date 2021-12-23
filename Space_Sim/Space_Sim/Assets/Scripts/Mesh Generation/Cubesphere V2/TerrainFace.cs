using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This creates a single big, subdividable Quad, it can be combined later with 6 others to make a cube
*/
public class TerrainFace
{
    Mesh mesh;
    int resolution; //Number of verts of a single edge of a face

    //Axes https://academo.org/demos/3d-vector-plotter/ to visualize
    Vector3 localUp;    //Up Vector
    Vector3 axisA;
    Vector3 axisB;

    public TerrainFace(Mesh mesh, int resolution, Vector3 localUp)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        //Set find axis A and B
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void Constructmesh()
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
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y -.5f) * 2 * axisB;

                //***Convert to Sphere*** https://catlikecoding.com/unity/tutorials/cube-sphere/
                float px = pointOnUnitCube.x;
                float py = pointOnUnitCube.y;
                float pz = pointOnUnitCube.z;

                Vector3 pointOnUnitSphere = new Vector3();
                pointOnUnitSphere.x = px * Mathf.Sqrt(1f - ((py*py)/2) - ((pz*pz)/2) + (((py*py)*(pz*pz))/3));
                pointOnUnitSphere.y = py * Mathf.Sqrt(1f - ((px*px)/2) - ((pz*pz)/2) + ((px*px)*(pz*pz)/3));
                pointOnUnitSphere.z = pz * Mathf.Sqrt(1f - ((px*px)/2) - ((py*py)/2) + ((px*px)*(py*py)/3));
                //***********************

                vertices[i] = pointOnUnitSphere;

                //Create Triangles
                if(x != resolution - 1 && y != resolution - 1)  //Making sure we are trying to draw triangles over the edges (no vertices exist there)
                {
                    //*****In each Quad*****
                    //Tri 1
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + (resolution + 1);
                    triangles[triIndex + 2] = i + resolution;

                    //Tri 2
                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + (resolution + 1);
                    //**********************

                    triIndex += 6; //We created 6 vertices
                }
            }
        }

        //Assign to an actual mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
