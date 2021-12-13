using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class dictates when to do things
public class NBodySim : MonoBehaviour
{

CelestialBody[] bodies;

void Awake()
{
    bodies = FindObjectsOfType<CelestialBody>();
    Time.fixedDeltaTime = Universe.physicsTimeStep;
}

private void FixedUpdate() {
    for (int i = 0; i < bodies.Length; i++){bodies[i].UpdateVelocity(bodies, Universe.physicsTimeStep);}    //Tell Celestial Bodies to calculate new velocity
    for (int i = 0; i < bodies.Length; i++){bodies[i].UpdatePosition(Universe.physicsTimeStep);}            //Tell Celestial Bodies to calculate new position
}


}