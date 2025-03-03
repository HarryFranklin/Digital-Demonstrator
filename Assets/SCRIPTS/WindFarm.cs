using UnityEngine;
using System.Collections.Generic;

public class WindFarm : MonoBehaviour
{
    /**
    Current stage: WIND FARM
    Next stage: INVERTER
    **/

    // I/O
    public List<Turbine> inputTurbines = new List<Turbine>();
    public Inverter inverter;
    // I/O

    private float totalPowerOutput;

    void Update()
    {
        totalPowerOutput = CalculateTotalPower();
        inverter.ReceivePower(totalPowerOutput);
    }

    private float CalculateTotalPower()
    {
        float total = 0;
        foreach (Turbine turbine in inputTurbines)
        {
            total += turbine.GetPowerOutput();
        }
        return total;
    }

    public void ReceivePower(float power)
    {
        totalPowerOutput += power;
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
