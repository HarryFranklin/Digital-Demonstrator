using UnityEngine;
using System.Collections.Generic;

public class Transformer : MonoBehaviour
{
    // I/O
    public Inverter inputInverter;  // Reference to the input inverter
    public PowerGrid outputGrid;  // Reference to the output grid
    public Battery outputBattery;  // Reference to the battery
    // I/O

    public float powerInput;  // Power from the inverter
    public float powerToGrid;
    public float powerToBattery;

    void Update()
    {
        float gridDemand = outputGrid.totalConsumption; // Get total consumption

        // Reset powerToBattery for this frame
        powerToBattery = 0f;

        // If generated power matches consumption, send all power to the grid
        if (powerInput == gridDemand)
        {
            powerToGrid = powerInput;
        }
        // If generated power is more than needed, send excess to the battery
        else if (powerInput > gridDemand)
        {
            powerToGrid = gridDemand;
            float excessPower = powerInput - gridDemand;

            if (!outputBattery.isFull())
            {
                powerToBattery = excessPower;
            }
            else
            {
                Debug.Log("Battery full! Implement logic to reduce turbine power output.");
            }
        }
        // If there isn't enough generated power, send all available power to the grid
        else 
        {
            powerToGrid = powerInput;
        }

        // Apply calculated power values
        outputGrid.powerInput = powerToGrid;
        outputBattery.powerInput = powerToBattery;
    }
}