using UnityEngine;
using System.Collections.Generic;

public class Transformer : ElectricalComponent
{
    /**
    Turbine class is the source of the power system.  One or many turbines make up a wind farm.
    One or many wind farms send power to an inverter, then to a transformer.
    If power demand is too low, excess power is sent from the transformer to a battery, then to the grid as required.
    If demand is sufficient, power is sent from transformer to the grid.
    If the source power is too low, the battery is discharged to make up the difference.
    Power is sent from the grid to the consumer.

    Current stage: TRANSFORMER
    Next stage: BATTERY or GRID
    **/

    // I/O
    public Inverter inputInverter; // One inverter per transformer.
    public List<Battery> outputBatteries = new List<Battery>(); // Multiple batteries per transformer.
    public PowerGrid outputGrid; // One grid per transformer.
    // I/O

    void Update()
    {
        float remainingPower = inputPower;

        // Charge batteries first
        foreach (Battery battery in outputBatteries)
        {
            if (remainingPower > 0)
            {
                float stored = battery.Charge(remainingPower / outputBatteries.Count);
                remainingPower -= stored;
            }
        }

        // Send any remaining power to the grid
        if (remainingPower > 0 && outputGrid != null)
        {
            outputGrid.ReceivePower(remainingPower);
        }
    }
}
