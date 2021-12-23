using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
//Mesh is the data, set of vertices
//Mesh filter stores the mesh data
//Mesh renderer actually draws it on the screen

[Range(2,256)]
public int resolution = 10;

[SerializeField, HideInInspector]   //Will make sure the mesh filters are saved in the editor
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    Vector3[] cardinalDirections = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};
    
    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
    }












    //This function just creates the necessary objects and assigns them, it does not actually calculate anything
    void Initialize()
    {
        if(meshFilters == null || meshFilters.Length == 0)//Only creates new mesh filters if none exist
        {
            meshFilters = new MeshFilter[6]; //Storage Array for Mesh Filters
        }
            terrainFaces = new TerrainFace[6];   //Storage array for all terrain faces

        for (int i = 0; i < 6; i++) //Creating 6 Quads as Gameobjects
        {
            if(meshFilters[i] == null)//Only sets up new mesh filters if none exist
            {
                GameObject meshObj = new GameObject("mesh");    //Creating an actual gameobject on screen
                meshObj.transform.parent = transform;           //Attach to a parent object

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));           //Add mesh renderer component, and assign a defailt material for now
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();    //Add mesh filter component to meshObj, and save a reference to them in this array for easy access
                meshFilters[i].sharedMesh = new Mesh();                 //Initialize a mesh on the mesh filter components
            }

            //Calculate Meshes
            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, cardinalDirections[i]);
        }
    }

    //This method actually calculates the meshes
    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.Constructmesh();
        }
    }

}
