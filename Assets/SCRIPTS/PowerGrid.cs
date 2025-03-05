using UnityEngine;
using System.Collections.Generic;

public class PowerGrid : MonoBehaviour
{
    // I/O
    public List<Consumer> consumers;
    public Battery outputBattery;  // Reference to the battery supplying power
    public float powerInput;  // Power from the battery and other sources
    public float totalConsumption;
    private float dischargeRate = 50f;

    void Update()
    {
        // Reset total consumption before recalculating
        totalConsumption = 0f;

        // Recalculate total consumption from all consumers
        foreach (Consumer consumer in consumers)
        {
            totalConsumption += consumer.powerConsumption;
        }

        // If not enough power, add battery power to it
        if (powerInput < totalConsumption)
        {
            powerInput += outputBattery.storedPower / dischargeRate; 
        }

        // Ensure we don't assign more power than available
        float availablePower = powerInput;

        foreach (Consumer consumer in consumers)
        {
            if (totalConsumption > 0f)
            {   
                float requestedPower = consumer.powerConsumption;
                float allocatedPower = Mathf.Min(requestedPower, availablePower); // Allocate power
                consumer.powerInput = allocatedPower;
                availablePower -= allocatedPower;  // Subtract allocated power from available power
            }
            else
            {
                consumer.powerInput = 0f;
            }
        }

        // Debugging
        Debug.Log($"PowerGrid: Total Consumption = {totalConsumption}, Power Input = {powerInput}");
    }
}