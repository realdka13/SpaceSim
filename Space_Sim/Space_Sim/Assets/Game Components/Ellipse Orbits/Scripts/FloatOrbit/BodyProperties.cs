using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyProperties : MonoBehaviour
{
    [SerializeField]  public float surfaceGravity;
    [SerializeField]  public float radius = .5f;

    [HideInInspector] public float mass;

    void Awake()
    {
        mass = (radius * radius * surfaceGravity) / UniverseConstants.gravitationalConstant;
    }

    void OnValidate ()
    {
        mass = surfaceGravity * radius * radius / UniverseConstants.gravitationalConstant;
        GetComponent<Transform>().localScale = new Vector3(radius * 2, radius * 2, radius * 2);
    }
}
