using UnityEngine;
using System.Collections.Generic;

public class PowerGrid : ElectricalComponent
{    
    /**
    Turbine class is the source of the power system.  One or many turbines make up a wind farm.
    One or many wind farms send power to an inverter, then to a transformer.
    If power demand is too low, excess power is sent from the transformer to a battery, then to the grid as required.
    If demand is sufficient, power is sent from transformer to the grid.
    If the source power is too low, the battery is discharged to make up the difference.
    Power is sent from the grid to the consumer.

    Current stage: POWER_GRID
    Next stage: CONSUMER
    **/

    // I/O
    public Transformer inputTransformer; // One transformer per grid.
    public Battery inputBattery; // One battery per grid.
    public List<Consumer> outputConsumers = new List<Consumer>();

    public override void ReceivePower(float power)
    {
        outputPower += power;
    }

    public override float ProvidePower(float requestedPower)
    {
        float providedPower = Mathf.Min(requestedPower, outputPower);
        outputPower -= providedPower;
        return providedPower;
    }

    void Update()
    {
        float totalDemand = 0f;
        foreach (Consumer consumer in outputConsumers)
        {
            totalDemand += consumer.outputConsumption;
        }

        float providedPower = ProvidePower(totalDemand);

        if (providedPower < totalDemand && inputBattery != null)
        {
            float batteryPower = inputBattery.Discharge(totalDemand - providedPower);
            providedPower += batteryPower;
        }

        foreach (Consumer consumer in outputConsumers)
        {
            consumer.ReceivePower(providedPower / outputConsumers.Count);
        }
    }
}
