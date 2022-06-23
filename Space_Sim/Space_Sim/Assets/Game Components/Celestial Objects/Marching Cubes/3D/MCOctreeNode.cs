using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
The OctreeNode class Consists of a single node of an Octree, and each node can manage its own children
*/
public class MCOctreeNode
{
    //Self
    private Vector3 position;
    private float size;

    //Children
    private MCOctreeNode[] children;

    //Children Data
    private float quarter;
    private float childSideLength;

//******************************************************************************************************************************
//                                                     Public Functions
//******************************************************************************************************************************

    //Constructor
    public MCOctreeNode(float size, Vector3 position)
    {
        this.size = size;
        this.position = position;

        quarter = size / 4.0f; //Cube center of childen move quarter of parents center location
        childSideLength = size / 2.0f; //Children radius are half of parents
    }

    //Generate Children of This Node
    public MCOctreeNode[] GenerateChildren()
    {
        children = new MCOctreeNode[8];
        children[0] = new MCOctreeNode(childSideLength, position + new Vector3(-quarter, quarter, -quarter));
        children[1] = new MCOctreeNode(childSideLength, position + new Vector3(quarter, quarter, -quarter));
        children[2] = new MCOctreeNode(childSideLength, position + new Vector3(-quarter, quarter, quarter));
        children[3] = new MCOctreeNode(childSideLength, position + new Vector3(quarter, quarter, quarter));
        children[4] = new MCOctreeNode(childSideLength, position + new Vector3(-quarter, -quarter, -quarter));
        children[5] = new MCOctreeNode(childSideLength, position + new Vector3(quarter, -quarter, -quarter));
        children[6] = new MCOctreeNode(childSideLength, position + new Vector3(-quarter, -quarter, quarter));
        children[7] = new MCOctreeNode(childSideLength, position + new Vector3(quarter, -quarter, quarter));

        return children;
    }

    //Draw Boxes
    public void DrawBounds(Transform rootTransform)
    {
        Gizmos.matrix = rootTransform.localToWorldMatrix;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(position, new Vector3(size, size, size));

        if(children != null)
        {
            for (int i = 0; i < 8; i++)
            {
                if(children[i] != null)
                {
                    children[i].DrawBounds(rootTransform);
                }
            }
        }
    }
}