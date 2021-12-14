using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnRailsObject : MonoBehaviour
{

    const float TAU = Mathf.PI * 2;     //2pi, for the orbit sliders

    //Orbital Keplerian Parameters
    [Header("Keplerian Parameters")]
    [SerializeField]                            float semiMajorAxis = 20f;      //a - size
    [SerializeField] [Range(0f, 0.95f)]         float eccentricity;             //e - shape
    [SerializeField] [Range(0f, TAU)]           float inclination = 0f;         //i - tilt
    [SerializeField] [Range(0f, TAU)]           float longitudeOfAcendingNode;  //Ω - swivel
    [SerializeField] [Range(0f, TAU)]           float argumentOfPeriapsis;      //w - orbit position  **TODO Figure this out and change name accordingly

    [Header("Position Offset")]
    [SerializeField]                            float meanLongitude;            //L - body offset     **TODO Set slider limits
    
    [Space]
    [Header("Accuracy Settings")]
    [SerializeField] float accuracyTolerance = 1e-6f;
    [SerializeField] int maxIterations = 5;           //usually converges after 3-5 iterations.

    [Space]
    [SerializeField] OnRailsReferenceBody referenceBody;

    //Numbers which only change if orbit or mass changes
    [HideInInspector] [SerializeField] float mu, n, cosLOAN, sinLOAN, sinI, cosI, trueAnomalyConstant;

    void Update()
    {
        //---Calculating Mean Anomaly (M)---
        mu = Universe.G * referenceBody.mass;                                           //Standard Gravitational Paramter
        n = Mathf.Sqrt(mu / Mathf.Pow(semiMajorAxis, 3));                               //Mean Angular Motion n=sqrt(mu/a^3)
        float meanAnomaly = (float)(n * (Time.time - meanLongitude));                   //M=M0+n*dt, mean longitude which is offsetting the objects starting position


        /*
        ---Calculating the Eccentric Anomaly (E) using newtons method---
            X_n+1 = X_n - [f(x_n)/f'(x_n)]
            E_i+1= E_i - [E_i-M+e*sin(E_i)]/[-1 + e * cos(E_i)]
        */
        float E1 = meanAnomaly;   //initial guess
        float difference = 1f;
        for (int i = 0; difference > accuracyTolerance && i < maxIterations; i++)
        {
            float E0 = E1;
            E1 = E0 - (meanAnomaly - E0 + eccentricity * Mathf.Sin(E0)) / ((-1f) + eccentricity * Mathf.Cos(E0));
            difference = Mathf.Abs(E1 - E0);
        }
        float EccentricAnomaly = E1;


        //---Calculating True Anomaly (f)---
        trueAnomalyConstant = Mathf.Sqrt((1 + eccentricity) / (1 - eccentricity));
        float trueAnomaly = 2 * Mathf.Atan(trueAnomalyConstant * Mathf.Tan(EccentricAnomaly / 2)); //f=2*arctan((1+e)/(1-e)*tan(E/2))
        float distance = semiMajorAxis * (1 - eccentricity * Mathf.Cos(EccentricAnomaly));


        /*
        ---Place Object in 3D space---
            X=r*(cos(Ω)*cos(w+f))-sin(Ω)*sin(w+f)*cos(i))
            Y=r*(sin(Ω)*cos(w+f))+cos(Ω)*sin(w+f)*cos(i)) //Switching z and y to be aligned with xz not xy (Unitys axis are different)
            Z=r*(sin(i)*sin(w+f))
        */

        cosLOAN = Mathf.Cos(longitudeOfAcendingNode);
        sinLOAN = Mathf.Sin(longitudeOfAcendingNode);
        cosI = Mathf.Cos(inclination);
        sinI = Mathf.Sin(inclination);

        float cosAOPPlusTA = Mathf.Cos(argumentOfPeriapsis + trueAnomaly);
        float sinAOPPlusTA = Mathf.Sin(argumentOfPeriapsis + trueAnomaly);
        float x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
        float z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));
        float y = distance * (sinI * sinAOPPlusTA);

        transform.position = new Vector3(x, y, z) + referenceBody.transform.position;
    }
}