using UnityEngine;
using System.Collections.Generic;

public class Turbine : MonoBehaviour
{
    /**
    Turbine class is the source of the power system.  One or many turbines make up a wind farm.
    One or many wind farms send power to an inverter, then to a transformer.
    If power demand is too low, excess power is sent from the transformer to a battery, then to the grid as required.
    If demand is sufficient, power is sent from transformer to the grid.
    If the source power is too low, the battery is discharged to make up the difference.
    Power is sent from the grid to the consumer.

    Current stage: TURBINE
    Next stage: WIND FARM
    **/

    // I/O
    public WindFarm outputWindFarm; // One or multiple turbines per farm.
    // I/O

    public float windSpeed = 10f; // Controls rotation speed
    public float maxSpeed = 20f; // Max speed before efficiency drops off, and side effects can happen
    public bool isOperational = true; // Is the turbine on? Set to True by default

    public float powerOutput;
    public GameObject rotationPoint; // The part that should rotate (blades)
    public GameObject pivot; // The part that the blades should rotate around

    void Start()
    {
        
    }

    void Update()
    {
        if (isOperational && rotationPoint != null)
        {
            float rotationSpeed = Mathf.Clamp(windSpeed, 0, maxSpeed);
            rotationPoint.transform.RotateAround(pivot.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime * 10);
            powerOutput = CalculatePower(rotationSpeed);
        }
        else 
        { 
            powerOutput = 0; 
        } 
    }

    private float CalculatePower(float speed)
    {
        return speed; // Placeholder logic
    }

    public float GetPowerOutput()
    {
        return powerOutput;
    }
}