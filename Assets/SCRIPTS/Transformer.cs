using UnityEngine;
using System.Collections.Generic;

public class Transformer : MonoBehaviour
{
    // I/O
    public Inverter inputInverter;  // Reference to the input inverter
    public List<Battery> outputBatteries = new List<Battery>();  // List of batteries to store excess power
    public PowerGrid outputGrid;  // Reference to the output grid
    // I/O

    public float powerInput;
    public float powerToGrid;
    public float powerToBattery;

    void Start()
    {
    }

    void Update()
    {
        powerInput = inputInverter.powerInput;

        // Handle power distribution between grid and batteries
        foreach (Battery battery in outputBatteries)
        {
            // If battery is full, no power should go to it, send power to grid
            if (battery.isFull())
            {
                battery.powerInput = 0;
                outputGrid.powerInput += powerInput; // Add transformer power to output grid input
            }
            else if (battery.storedPower < battery.capacity && powerInput > 0)
            {
                // If battery isn't full, send power to the battery
                float availablePower = powerInput;
                float chargeAmount = Mathf.Min(availablePower, battery.capacity - battery.storedPower); // Charge battery with available power
                battery.powerInput = chargeAmount;
                powerToBattery = chargeAmount;
                powerInput -= chargeAmount; // Subtract the sent power from available transformer power
            }

            // Add any leftover power to the grid
            if (powerInput > 0)
            {
                outputGrid.powerInput += powerInput;
            }
        }
    }
}