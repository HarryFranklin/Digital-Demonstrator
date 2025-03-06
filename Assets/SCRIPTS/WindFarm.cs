using UnityEngine;
using System.Collections.Generic;

public class WindFarm : MonoBehaviour
{
    public List<Turbine> inputTurbines = new List<Turbine>();
    public Inverter inverter;  

    public float totalPowerInput;
    private PowerLineConnector powerLine;

    void Start()
    {
        powerLine = GetComponent<PowerLineConnector>();
    }

    void Update()
    {
        totalPowerInput = 0f;
        foreach (Turbine turbine in inputTurbines)
        {
            totalPowerInput += turbine.powerOutput;
        }

        if (inverter != null)
        {
            inverter.powerInput = totalPowerInput;
        }

        if (powerLine != null)
        {
            powerLine.powerFlow = totalPowerInput;
        }
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