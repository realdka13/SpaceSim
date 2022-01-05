using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Chunk
//TODO LOD/Culling
//TODO Upgrade Interpolation code
//TODO shows in editor/Actively changes
public class MarchingBody : MonoBehaviour
{
	//Settings
    [Header("Shape")]
	public int radius;
    [Range(0f,1f)]
    public float terrainScaler;

    [Header("Smoothness")]
	public bool smoothTerrain;
	public bool flatShaded;	//***WARNING, INCREASES VERTEX COUNT AS VERTICES GET DUPLICATED

    //Chunks
    private MarchingChunk chunk;

    private void Awake()
    {
        chunk = new MarchingChunk(radius, terrainScaler, smoothTerrain, flatShaded);
    }

    void Start()
    {
        chunk.Initialize(transform);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow cube at the transform position
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, new Vector3(radius, radius, radius));
    }
}
