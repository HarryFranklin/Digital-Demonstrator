using UnityEngine;
using System.Collections.Generic;

public class WindFarm : ElectricalComponent
{
    /**
    Turbine class is the source of the power system.  One or many turbines make up a wind farm.
    One or many wind farms send power to an inverter, then to a transformer.
    If power demand is too low, excess power is sent from the transformer to a battery, then to the grid as required.
    If demand is sufficient, power is sent from transformer to the grid.
    If the source power is too low, the battery is discharged to make up the difference.
    Power is sent from the grid to the consumer.

    Current stage: WIND FARM
    Next stage: INVERTER
    **/

    // I/O
    public List<Turbine> inputTurbines = new List<Turbine>(); // One or many turbines per farm.
    public Inverter inverter; // One inverter per farm.
    // I/O

    [Header("Connected Turbines")]
    [SerializeField] private List<float> turbinePowerOutputs = new List<float>(); // Show turbine power

    void Update()
    {
        outputPower = 0;
        turbinePowerOutputs.Clear();  // Reset the list

        foreach (Turbine turbine in inputTurbines)
        {
            if (turbine.isOperational)
            {
                float power = turbine.GetPowerOutput();
                outputPower += power;
                turbinePowerOutputs.Add(power); // Store each turbine's power for visibility
            }
        }

        inverter.ReceivePower(outputPower);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.green;
        foreach (Turbine turbine in inputTurbines)
        {
            if (turbine != null)
            {
                Gizmos.DrawLine(transform.position, turbine.transform.position);
            }
        }
    }

    public Vector3 GetCenterPoint()
    {
        if (inputTurbines.Count == 0) return transform.position;

        Vector3 center = Vector3.zero;
        foreach (Turbine turbine in inputTurbines)
        {
            center += turbine.transform.position;
        }
        return center / inputTurbines.Count;
    }
}