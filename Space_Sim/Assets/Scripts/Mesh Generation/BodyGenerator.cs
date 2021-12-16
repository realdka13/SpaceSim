using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyGenerator : MonoBehaviour
{
    public Material planetMaterial;
    public float planetSize = 1f;
    GameObject planet;
    Mesh planetMesh;
    Vector3[] planetVertices;
    int[] planetTriangles;
    MeshRenderer planetMeshRenderer;
    MeshFilter planetMeshFilter;
    MeshCollider planetMeshCollider;


    private void Start()
    {
        CreatePlanet();
    }


    public void CreatePlanet()
    {
        CreatePlanetGameObject();
        RecalculateMesh();
    }

    void CreatePlanetGameObject()
    {
        planet = new GameObject();
        planet.name = "New Icosphere";
        planetMeshFilter = planet.AddComponent<MeshFilter>();
        planetMesh = planetMeshFilter.mesh;
        planetMeshRenderer = planet.AddComponent<MeshRenderer>();
        planetMeshRenderer.material = planetMaterial;
        planet.transform.localScale = new Vector3(planetSize, planetSize, planetSize);
        Icosphere.Create(planet);
    }

    void RecalculateMesh()
    {
        planetMesh.RecalculateBounds();
        planetMesh.RecalculateTangents();
        planetMesh.RecalculateNormals();
    }
}
