using UnityEngine;
using System.Collections.Generic;

public class PowerGrid : MonoBehaviour
{
    /**
    Current stage: GRID
    Next stage: CONSUMER
    **/

    // I/O
    public List<Consumer> consumers;  // List of consumers connected to this grid
    // I/O

    public float powerInput;  // Total power available from the grid

    void Update()
    {
        float totalConsumption = 0f;
        foreach (Consumer consumer in consumers)
        {
            totalConsumption += consumer.powerConsumption;  // Sum all consumers' power consumption needs
        }

        // Distribute power based on each consumer's consumption needs
        foreach (Consumer consumer in consumers)
        {
            if (totalConsumption > 0f) // Avoid division by zero
            {
                consumer.powerInput = (consumer.powerConsumption / totalConsumption) * powerInput;
            }
            else
            {
                consumer.powerInput = 0f;  // No power if no consumption
            }
        }
    }
}