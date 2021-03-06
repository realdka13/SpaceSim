using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent (typeof (Rigidbody))]
public class CelestialBody : MonoBehaviour{

public float surfaceGravity;
public float radius;
public Vector3 initialVelocity;

Vector3 currentVelocity;
float mass;

void OnValidate ()
    {
        mass = surfaceGravity * radius * radius / UniverseConstants.gravitationalConstant;
        GetComponent<Transform>().localScale = new Vector3(radius * 2, radius * 2, radius * 2);
    }

void Awake()
    {
        currentVelocity = initialVelocity;
        mass = (radius * radius * surfaceGravity) / UniverseConstants.gravitationalConstant;
    }

public void UpdateVelocity(CelestialBody[] allBodies, float timeStep)                                                               //NBodySim will activate this function
{
    foreach (var otherBodies in allBodies)
    {
        if (otherBodies != this)
        {
            float sqrDst = (otherBodies.GetComponent<Rigidbody>().position - GetComponent<Rigidbody>().position).sqrMagnitude;      //Calculates r
            Vector3 forceDir = (otherBodies.GetComponent<Rigidbody>().position - GetComponent<Rigidbody>().position).normalized;    //Calculates the normal vector direction for the force of gravity
            Vector3 force = forceDir * (UniverseConstants.gravitationalConstant * mass * otherBodies.mass) / sqrDst;                         //Calculates the force vector for gravity (Newtons equation); Fdir*(G*m1*m2)/r
            Vector3 acceleration = force / mass;                                                                                    //Converts force to an acceleration; a=f/m
            currentVelocity += acceleration * timeStep;                                                                             //Finds new velocity based off off acceleration
        }
    }
}

public void UpdatePosition(float timeStep){GetComponent<Rigidbody>().position += currentVelocity * timeStep;}                       //NBodySim will activate this function; Calculates position off of current velocity


}