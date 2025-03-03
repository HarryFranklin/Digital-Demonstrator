using UnityEngine;
using System.Collections.Generic;

public class Transformer : MonoBehaviour
{
    /**
    Current stage: TRANSFORMER
    Next stage: BATTERY or GRID
    **/

    // I/O
    public Inverter inputInverter;
    public List<Battery> outputBatteries = new List<Battery>();
    public PowerGrid outputGrid;
    // I/O

    public float inputPower;

    public void ReceivePower(float power)
    {
        inputPower = power;
    }

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