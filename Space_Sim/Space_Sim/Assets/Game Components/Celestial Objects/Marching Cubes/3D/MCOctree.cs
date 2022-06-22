using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCOctree
{
    private MCOctreeNode rootNode;

//******************************************************************************************************************************
//                                                     Public Functions
//******************************************************************************************************************************
    public MCOctree(float rootSize, Vector3 rootPosition, int octreeSubdivions)
    {
        //Create root node
        rootNode = new MCOctreeNode(rootSize, rootPosition);
        DivideOctree(rootNode, octreeSubdivions);
    }

    //***WARNING RECURSIVE*** to generate all of the needed children
    private void DivideOctree(MCOctreeNode node, int subdivionsRemaining)
    {
        if(subdivionsRemaining > 0)
        {
            subdivionsRemaining--;
            MCOctreeNode[] nodesChildren = node.GenerateChildren();
            for (int i = 0; i < 8; i++)
            {
                DivideOctree(nodesChildren[i], subdivionsRemaining);
            }
        }
    }

    //Just for drawing the gizmo cubes
    public void DrawBounds(Transform rootTransform)
    {
        rootNode.DrawBounds(rootTransform);
    }
}