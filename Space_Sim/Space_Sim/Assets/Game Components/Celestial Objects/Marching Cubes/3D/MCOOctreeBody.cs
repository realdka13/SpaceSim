using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCOOctreeBody : MonoBehaviour
{
    //Octree
    [Header("Octree")]
    public float rootSize;
    private MCOctree mcOctree;
    [Range(0,3)]
    public int octreeSubdivions;

    //Gizmos
    public bool drawGizmos;

//******************************************************************************************************************************
//                                                     Private Functions
//******************************************************************************************************************************
    private void Start()
    {
        //Create new marching Cubes Octree
        mcOctree = new MCOctree(rootSize, this.transform.position, octreeSubdivions);

        //Error handling (Checks if Size of Root node is invalid and sends a warning)
        if(rootSize == 0){Debug.LogWarning("Root Size = 0");}else if(rootSize < 0){Debug.LogWarning("Root Size < 0");}
    }

    private void OnValidate()
    {
        //Check if subdivisions have changed
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying && drawGizmos)
        {
            mcOctree.DrawBounds(this.transform);
        }
    }
}