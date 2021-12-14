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
    [SerializeField] [Range(0f, TAU)]           float longitudeOfAcendingNode;  //n - swivel
    [SerializeField] [Range(0f, TAU)]           float argumentOfPeriapsis;      //w - position
    [SerializeField]                            float meanLongitude;            //L - offset
    float meanAnomaly;
    
    [Space]
    [Header("Accuracy Settings")]
    [SerializeField] float accuracyTolerance = 1e-6f;
    [SerializeField] int maxIterations = 5;           //usually converges after 3-5 iterations.

    [Space]
    [SerializeField] OnRailsReferenceBody referenceBody;

    //Numbers which only change if orbit or mass changes
    [HideInInspector] [SerializeField] float mu;
    [HideInInspector] [SerializeField] float n, cosLOAN, sinLOAN, sinI, cosI, trueAnomalyConstant;

    void Update()
    {
        //Calculating Mean Anomaly
        mu = Universe.G * referenceBody.mass;                                           //Standard Gravitational Paramter
        n = Mathf.Sqrt(mu / Mathf.Pow(semiMajorAxis, 3));                               //Mean Angular Motion n=sqrt(mu/a^3)
        meanAnomaly = (float)(n * (Time.time - meanLongitude));                         //M=M0+n*dt, mean longitude which is offsetting the objects starting position


        //Calculating the Eccentric Anomaly using newtons method
        //X_n+1 = X_n - [f(x_n)/f'(x_n)]
        //E_i+1= E_i - [E_i-M+e*sin(E_i)]/[-1 + e * cos(E_i)]
        float E1 = meanAnomaly;   //initial guess
        float difference = 1f;
        for (int i = 0; difference > accuracyTolerance && i < maxIterations; i++)
        {
            float E0 = E1;
            E1 = E0 - (meanAnomaly - E0 + eccentricity * Mathf.Sin(E0)) / ((-1f) + eccentricity * Mathf.Cos(E0));
            difference = Mathf.Abs(E1 - E0);
        }
        float EccentricAnomaly = E1;


        //Calculating True Anomaly
        trueAnomalyConstant = Mathf.Sqrt((1 + eccentricity) / (1 - eccentricity));
        float trueAnomaly = 2 * Mathf.Atan(trueAnomalyConstant * Mathf.Tan(EccentricAnomaly / 2));
        float distance = semiMajorAxis * (1 - eccentricity * Mathf.Cos(EccentricAnomaly));


        //Using the Keplarian values are placing them in 3D space
        cosLOAN = Mathf.Cos(longitudeOfAcendingNode);
        sinLOAN = Mathf.Sin(longitudeOfAcendingNode);
        cosI = Mathf.Cos(inclination);
        sinI = Mathf.Sin(inclination);

        float cosAOPPlusTA = Mathf.Cos(argumentOfPeriapsis + trueAnomaly);
        float sinAOPPlusTA = Mathf.Sin(argumentOfPeriapsis + trueAnomaly);
        float x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
        float z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));      //Switching z and y to be aligned with xz not xy
        float y = distance * (sinI * sinAOPPlusTA);

        transform.position = new Vector3(x, y, z) + referenceBody.transform.position;
    }
}