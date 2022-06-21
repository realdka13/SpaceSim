using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOctree : MonoBehaviour
{
    public GameObject[] worldObjects;
    public int nodeMinSize = 5;
    Octree octree;

    void Start()
    {
        octree = new Octree(worldObjects, nodeMinSize);  
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            octree.rootNode.Draw();
        }
    }
}