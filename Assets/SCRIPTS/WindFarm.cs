using UnityEngine;
using System.Collections.Generic;

public class WindFarm : MonoBehaviour
{
    // I/O
    public List<Turbine> inputTurbines = new List<Turbine>();
    public Inverter inverter;  // Reference to inverter (next stage)
    // I/O

    private float totalPowerInput;

    void Update()
    {
        // Calculate total power input from turbines
        totalPowerInput = 0f;
        foreach (Turbine turbine in inputTurbines)
        {
            totalPowerInput += turbine.powerOutput; // Directly use turbine's powerOutput
        }

        // Send power to inverter if available
        if (inverter != null)
        {
            inverter.powerInput = totalPowerInput;
        }
    }

    // Power receiving method, can be extended for managing power more efficiently
    public void ReceivePower(float power)
    {
        totalPowerInput += power; // Accumulate power from turbines or other sources
    }

    // Method to visualise wind farm centre (average position of turbines)
    public Vector3 GetCenterPoint()
    {
        if (inputTurbines.Count == 0) return Vector3.zero;

        Vector3 center = Vector3.zero;
        foreach (Turbine turbine in inputTurbines)
        {
            center += turbine.transform.position;
        }
        return center / inputTurbines.Count;
    }
}