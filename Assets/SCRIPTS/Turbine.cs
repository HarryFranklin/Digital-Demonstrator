using UnityEngine;
using System.Collections.Generic;

public class Turbine : MonoBehaviour
{
    /**
    Current stage: TURBINE
    Next stage: WIND FARM
    **/

    // I/O
    public WindFarm outputWindFarm;
    // I/O

    public float windSpeed = 10f;
    public float maxSpeed = 20f;
    public bool isOperational = true;

    public GameObject rotationPoint;
    public GameObject pivot;

    private float powerOutput;

    void Update()
    {
        if (isOperational)
        {
            RotateBlades();
            powerOutput = CalculatePowerOutput(windSpeed);
        }
        else
        {
            powerOutput = 0f;
        }
    }

    private void RotateBlades()
    {
        if (rotationPoint != null && pivot != null)
        {
            float rotationSpeed = Mathf.Clamp(windSpeed, 0, maxSpeed);
            rotationPoint.transform.RotateAround(pivot.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime * 10);
        }
    }

    private float CalculatePowerOutput(float speed)
    {
        return speed;
    }

    public float GetPowerOutput()
    {
        return powerOutput;
    }
}