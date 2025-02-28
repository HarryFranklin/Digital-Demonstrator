using UnityEngine;
using System.Collections.Generic;

public class WindFarm : MonoBehaviour
{
    public List<Turbine> turbines = new List<Turbine>(); // List of all turbines
    public float totalPowerOutput = 0f;

    void Start()
    {
        if (turbines.Count == 0)
        {
            turbines.AddRange(GetComponentsInChildren<Turbine>());
        }
    }

    void Update()
    {
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

    public Vector3 GetCenterPoint()
    {
        if (turbines.Count == 0) return Vector3.zero;

        Vector3 center = Vector3.zero;
        foreach (Turbine turbine in turbines)
        {
            center += turbine.transform.position;
        }
        return center / turbines.Count;
    }
}