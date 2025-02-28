using UnityEngine;
using System.Collections.Generic;

public class WindFarm : MonoBehaviour
{
    public List<Turbine> turbines = new List<Turbine>(); // List of all turbines in the farm
    public float totalPowerOutput = 0f;

    void Start()
    {
        // Automatically find all turbines in the farm if not assigned manually
        if (turbines.Count == 0)
        {
            turbines.AddRange(GetComponentsInChildren<Turbine>());
        }
    }

    void Update()
    {
        // Continuously update total power output
        totalPowerOutput = GetTotalPowerOutput();
    }

    public float GetTotalPowerOutput()
    {
        float totalPowerOutput = 0;
        foreach (Turbine turbine in turbines)
        {
            if (turbine != null && turbine.isOperational)
            {
                totalPowerOutput += turbine.GetPowerOutput();
            }
        }
        return totalPowerOutput;
    }

    public void ToggleTurbine(int index, bool state)
    {
        // Enable or disable a specific turbine by index
        if (index >= 0 && index < turbines.Count)
        {
            turbines[index].isOperational = state;
        }
    }
}