using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnRailsObject : MonoBehaviour
{

    const float TAU = Mathf.PI * 2;     //2pi, for the orbit sliders

    //Orbital Keplerian Parameters
    [Header("Keplerian Parameters")]
    [SerializeField]                            float semiMajorAxis = 20f;      //a - Size
    [SerializeField] [Range(0f, 0.95f)]         float eccentricity;             //e - Shape
    [SerializeField] [Range(0f, TAU)]           float inclination = 0f;         //i - Tilt
    [SerializeField] [Range(0f, TAU)]           float longitudeOfAcendingNode;  //Ω - Swivel
    [SerializeField] [Range(0f, TAU)]           float argumentOfPeriapsis;      //w - Rotation

    [Header("Body Position Offset")]
    [SerializeField]                            float meanLongitude;            //L - Body Position Offset
    
    [Header("Accuracy Settings")]
    [SerializeField] float accuracyTolerance = 1e-6f;
    [SerializeField] int maxIterations = 5;           //usually converges after 3-5 iterations.

    [Header("Orbit Drawing")]
    [SerializeField] bool redrawEveryFrame = false;    //Useful for bodies such as moons which orbit around a planet that is orbiting another body
    [SerializeField] int orbitResolution = 50;
    [SerializeField] [Range(0f,1)] float lineWidth = .25f;

    [Space]
    [SerializeField] RailBodyProperties referenceBody;
    LineRenderer lineRender;

    //Numbers which only change if orbit or mass changes
    [HideInInspector] [SerializeField] float mu, n, trueAnomalyConstant, cosLOAN, sinLOAN, sinI, cosI; //


    void Awake()
    {
        CalculateSemiConstants();
        DrawOrbit();
    }

    void Update()
    {
        //---Calculating Mean Anomaly (M)---
        //Calulate n here..
        float meanAnomaly = (float)(n * (Time.time + meanLongitude));      //M=M0+n*dt, mean longitude which is offsetting the objects starting position



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
        //Calculating true anomaly constant here...
        float trueAnomaly = 2 * Mathf.Atan(trueAnomalyConstant * Mathf.Tan(EccentricAnomaly / 2)); //f=2*arctan((1+e)/(1-e)*tan(E/2))
        float distance = semiMajorAxis * (1 - eccentricity * Mathf.Cos(EccentricAnomaly));



        /*
        ---Place Object in 3D space---
            X=r*(cos(Ω)*cos(w+f))-sin(Ω)*sin(w+f)*cos(i))
            Y=r*(sin(Ω)*cos(w+f))+cos(Ω)*sin(w+f)*cos(i)) //Switching z and y to be aligned with xz not xy (Unitys axis are different)
            Z=r*(sin(i)*sin(w+f))
        */

        //Calculate sin/cos consts here...
        float cosAOPPlusTA = Mathf.Cos(argumentOfPeriapsis + trueAnomaly);
        float sinAOPPlusTA = Mathf.Sin(argumentOfPeriapsis + trueAnomaly);

        float x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
        float z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));
        float y = distance * (sinI * sinAOPPlusTA);

        transform.position = new Vector3(x, y, z) + referenceBody.transform.position;

        if(redrawEveryFrame)
        {
            DrawOrbit();
        }
    }

    void CalculateSemiConstants()
    {
        //For Calculating mean anomaly
        mu = Universe.G * referenceBody.mass;                       //Standard Gravitational Paramter
        n = Mathf.Sqrt(mu / Mathf.Pow(semiMajorAxis, 3));           //Mean Angular Motion n=sqrt(mu/a^3)

        //For Calculating true anomaly
        trueAnomalyConstant = Mathf.Sqrt((1 + eccentricity) / (1 - eccentricity));

        //For adding the orbit to 3D space
        cosLOAN = Mathf.Cos(longitudeOfAcendingNode);
        sinLOAN = Mathf.Sin(longitudeOfAcendingNode);
        cosI = Mathf.Cos(inclination);
        sinI = Mathf.Sin(inclination);
    }

    void DrawOrbit()
    {
        //CalculateSemiConstants must always be called before this
        Vector3[] orbitalPoints = new Vector3[orbitResolution + 1];
        if (referenceBody == null)
        {
            Debug.LogWarning($"Add a reference body to {gameObject.name}");
            return;
        }
        else
        {
            Vector3 pos = referenceBody.transform.position;     //Get position of reference body
            float orbitFraction = 1f / orbitResolution;         //breakup orbit into chunks

            //Calculate Points
            for (int i = 0; i < orbitResolution + 1; i++)
            {
                float EccentricAnomaly = i * orbitFraction * TAU;

                float trueAnomaly = 2 * Mathf.Atan(trueAnomalyConstant * Mathf.Tan(EccentricAnomaly / 2));
                float distance = semiMajorAxis * (1 - eccentricity * Mathf.Cos(EccentricAnomaly));

                float cosAOPPlusTA = Mathf.Cos(argumentOfPeriapsis + trueAnomaly);
                float sinAOPPlusTA = Mathf.Sin(argumentOfPeriapsis + trueAnomaly);

                float x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
                float z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));
                float y = distance * (sinI * sinAOPPlusTA);

                float meanAnomaly = EccentricAnomaly - eccentricity * Mathf.Sin(EccentricAnomaly);

                orbitalPoints[i] = pos + new Vector3(x, y, z);
            }

            lineRender = GetComponent<LineRenderer>();
            lineRender.startWidth = lineWidth;
            lineRender.endWidth = lineWidth;
            lineRender.positionCount = orbitResolution + 1;
            lineRender.SetPositions(orbitalPoints);
        }
    }

    void RecalculateBodyPosition()
    {
        float meanAnomaly = (float)(n * (0 + meanLongitude));      //M=M0+n*dt, mean longitude which is offsetting the objects starting position
        float E1 = meanAnomaly;
        float difference = 1f;
        for (int i = 0; difference > accuracyTolerance && i < maxIterations; i++)
        {
            float E0 = E1;
            E1 = E0 - (meanAnomaly - E0 + eccentricity * Mathf.Sin(E0)) / ((-1f) + eccentricity * Mathf.Cos(E0));
            difference = Mathf.Abs(E1 - E0);
        }
        float EccentricAnomaly = E1;
        float trueAnomaly = 2 * Mathf.Atan(trueAnomalyConstant * Mathf.Tan(EccentricAnomaly / 2)); //f=2*arctan((1+e)/(1-e)*tan(E/2))
        float distance = semiMajorAxis * (1 - eccentricity * Mathf.Cos(EccentricAnomaly));
        float cosAOPPlusTA = Mathf.Cos(argumentOfPeriapsis + trueAnomaly);
        float sinAOPPlusTA = Mathf.Sin(argumentOfPeriapsis + trueAnomaly);
        float x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
        float z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));
        float y = distance * (sinI * sinAOPPlusTA);
        transform.position = new Vector3(x, y, z) + referenceBody.transform.position;
    }

    private void OnValidate() 
    {
        CalculateSemiConstants();
        RecalculateBodyPosition();
        DrawOrbit();
    }
}