using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RailBody
{
    [SerializeField]
    private String name;
    private Vector3d coordinates;

    const float TAU = Mathf.PI * 2;     //2pi, for the orbit sliders

    //Orbital Keplerian Parameters
    [Header("Keplerian Parameters")]
    [SerializeField]                            double semiMajorAxis = 20d;     //a - Size
    [SerializeField] [Range(0f, 0.95f)]         float eccentricity;             //e - Shape
    [SerializeField] [Range(0f, TAU)]           float inclination = 0f;         //i - Tilt
    [SerializeField] [Range(0f, TAU)]           float longitudeOfAcendingNode;  //Ω - Swivel
    [SerializeField] [Range(0f, TAU)]           float argumentOfPeriapsis;      //w - Rotation

    [Header("Body Position Offset")]
    [SerializeField]                            float meanLongitude;            //L - Body Position Offset
 
    [Header("Accuracy Settings")]
    [SerializeField] float accuracyTolerance = 1e-6f;
    [SerializeField] int maxIterations = 5;           //usually converges after 3-5 iterations.

    [HideInInspector] [SerializeField] double mu, n, trueAnomalyConstant, cosLOAN, sinLOAN, sinI, cosI;

//*********************************************************************************************************************

    //This must be called anytime theres an update to the Keplerian Parameters
    public void CalculateSemiConstants(double mass)
    {
        //For Calculating mean anomaly
        mu = UniverseConstants.gravitationalConstant * mass;                       //Standard Gravitational Paramter
        n = Mathd.Sqrt(mu / Mathd.Pow(semiMajorAxis, 3));           //Mean Angular Motion n=sqrt(mu/a^3)

        //For Calculating true anomaly
        trueAnomalyConstant = Mathd.Sqrt((1 + eccentricity) / (1 - eccentricity));

        //For adding the orbit to 3D space
        cosLOAN = Mathd.Cos(longitudeOfAcendingNode);
        sinLOAN = Mathd.Sin(longitudeOfAcendingNode);
        cosI = Mathd.Cos(inclination);
        sinI = Mathd.Sin(inclination);
    }

    public void CalculateCoordinates(float time)
    {
        //---Calculating Mean Anomaly (M)---
        //Calulate n here..
        double meanAnomaly = (float)(n * (time + meanLongitude));      //M=M0+n*dt, mean longitude which is offsetting the objects starting position



        /*
        ---Calculating the Eccentric Anomaly (E) using newtons method---
            X_n+1 = X_n - [f(x_n)/f'(x_n)]
            E_i+1= E_i - [E_i-M+e*sin(E_i)]/[-1 + e * cos(E_i)]
        */
        double E1 = meanAnomaly;   //initial guess
        double difference = 1f;
        for (int i = 0; difference > accuracyTolerance && i < maxIterations; i++)
        {
            double E0 = E1;
            E1 = E0 - (meanAnomaly - E0 + eccentricity * Mathd.Sin(E0)) / ((-1f) + eccentricity * Mathd.Cos(E0));
            difference = Mathd.Abs(E1 - E0);
        }
        double EccentricAnomaly = E1;



        //---Calculating True Anomaly (f)---
        //Calculating true anomaly constant here...
        double trueAnomaly = 2d * Mathd.Atan(trueAnomalyConstant * Mathd.Tan(EccentricAnomaly / 2d)); //f=2*arctan((1+e)/(1-e)*tan(E/2))
        double distance = semiMajorAxis * (1d - eccentricity * Mathd.Cos(EccentricAnomaly));



        /*
        ---Place Object in 3D space---
            X=r*(cos(Ω)*cos(w+f))-sin(Ω)*sin(w+f)*cos(i))
            Y=r*(sin(Ω)*cos(w+f))+cos(Ω)*sin(w+f)*cos(i)) //Switching z and y to be aligned with xz not xy (Unitys axis are different)
            Z=r*(sin(i)*sin(w+f))
        */
        //Calculate sin/cos consts here...
        double cosAOPPlusTA = Mathd.Cos(argumentOfPeriapsis + trueAnomaly);
        double sinAOPPlusTA = Mathd.Sin(argumentOfPeriapsis + trueAnomaly);

        double x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
        double z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));
        double y = distance * (sinI * sinAOPPlusTA);

        coordinates = new Vector3d(x, y, z) + Vector3d.zero;    // TODO the Vector3d.zero is the reference body location
    }

    public Vector3d GetCoordinates()
    {
        return coordinates;
    }
}