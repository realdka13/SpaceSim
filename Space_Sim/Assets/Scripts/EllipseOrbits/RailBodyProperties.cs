using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailBodyProperties : MonoBehaviour
{
    [SerializeField]  public float surfaceGravity;
    [SerializeField]  public float radius;

    [HideInInspector] public float mass;

    void Awake()
    {
        mass = (radius * radius * surfaceGravity) / Universe.gravitationalConstant;
    }

    void OnValidate ()
    {
        mass = surfaceGravity * radius * radius / Universe.gravitationalConstant;
        GetComponent<Transform>().localScale = new Vector3(radius * 2, radius * 2, radius * 2);
    }
}
