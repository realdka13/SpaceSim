using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyGenerator : MonoBehaviour
{
    public int subdivisions = 0;
    public float bodySize = 1f;
    public Material bodyMaterial;

    [Space]
    public int vertexCount;
    public int triangleCount;

    GameObject body;
    Mesh bodyMesh;
    MeshRenderer bodyMeshRenderer;
    MeshFilter bodyMeshFilter;
    MeshCollider bodyMeshCollider;

    public void CreatebodyGameObject()
    {
        body = new GameObject();
        body.name = "New Icosphere";

        bodyMeshFilter = body.AddComponent<MeshFilter>();
        bodyMesh = bodyMeshFilter.mesh;
        bodyMeshRenderer = body.AddComponent<MeshRenderer>();
        bodyMeshRenderer.material = bodyMaterial;

        body.transform.localScale = new Vector3(bodySize, bodySize, bodySize);

        Icosphere.Create(body, subdivisions, out vertexCount, out triangleCount);
    }
}
