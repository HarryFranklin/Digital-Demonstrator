using UnityEngine;
using System.Collections.Generic;

public class WindFarm : ElectricalComponent
{
    public List<Turbine> inputTurbines = new List<Turbine>();
    public Inverter inverter;  // Only connect to an inverter

    void Update()
    {
        float totalPower = 0;

        foreach (Turbine turbine in inputTurbines)
        {
            if (turbine.isOperational)
            {
                totalPower += turbine.OutputPower; // Use getter
            }
        }

        SetOutputPower(totalPower);
        inverter.ReceivePower(totalPower);
    }

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