using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
public float mass;
public float radius;
public Vector3 initialVelocity;
Vector3 currentVelocity;

void Awake()
{
    currentVelocity = initialVelocity;
}

/*
Gets all Celestial Bodies
Foreach CelestialBody..
    Calculate the distance from the focus object
    Calculate the direction of the force of gravity (using a normal vector)
    Calculate the force of gravity
    calculate the acceleration
    Set current velocity
*/
public void UpdateVelocity (CelestialBody[] allBodies, float timeStep)
{
    foreach (var otherBodies in allBodies)
    {
        if (otherBodies != this)
        {
            float sqrDst = (otherBodies.GetComponent<Rigidbody>().position - GetComponent<Rigidbody>().position).sqrMagnitude;
            Vector3 forceDir = (otherBodies.GetComponent<Rigidbody>().position - GetComponent<Rigidbody>().position).normalized;
            Vector3 force = forceDir * Universe.gravitationalConstant * mass * otherBodies.mass / sqrDst;
            Vector3 acceleration = force / mass;
            currentVelocity += acceleration * timeStep;
        }
    }
}

public void UpdatePosition (float timeStep)
{
    GetComponent<Rigidbody>().position += currentVelocity * timeStep;
}


}