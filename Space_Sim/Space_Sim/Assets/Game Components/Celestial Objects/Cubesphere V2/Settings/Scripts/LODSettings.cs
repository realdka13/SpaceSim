using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LODSettings", menuName = "LODSettings")]
public class LODSettings : ScriptableObject 
{
    public float meshUpdateFrequency = .1f;
    public int LOD0Resolution = 9;

    [Space]
    public float[] detailLevelDistances = new float[] { //1 Square is subdivided into 4 at every LOD level
        Mathf.Infinity, //LOD0
        6000f,          //LOD1
        2500f,          //LOD2
        1000f,          //LOD3
        400f,           //LOD4
        150f,           //LOD5
        70f,            //LOD6
        30f,            //LOD7
        10f             //LOD8
    };

    public float[,] cullingAngle = {    //*** Note that this cannot be changed per planet, this will apply to all ***
    {Mathf.Infinity, 90f},
    {6000f, 80f}, 
    {2000f, 60f},
    {1500f, 45f},
    {1000, 30f},
    {500, 20f},
    {100f, 10f},
    };
}