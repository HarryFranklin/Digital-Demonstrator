using UnityEngine;
using System.Collections.Generic;

public class Transformer : ElectricalComponent
{
    public Inverter inputInverter;
    public Battery outputBattery;  // Optional, if a battery is connected
    public PowerGrid outputGrid;

    void Update()
    {
        float remainingPower = inputPower;

        // Charge battery first if there is excess power
        if (outputBattery != null && remainingPower > 0)
        {
            float stored = outputBattery.Charge(remainingPower);
            remainingPower -= stored;
        }

        // Send remaining power to the grid
        if (remainingPower > 0 && outputGrid != null)
        {
            outputGrid.ReceivePower(remainingPower);
        }
    }
}