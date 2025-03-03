using UnityEngine;
using System.Collections.Generic;

public class Transformer : MonoBehaviour
{
    /**
    Current stage: TRANSFORMER
    Next stage: BATTERY or GRID
    **/

    // I/O
    public Inverter inputInverter;  // Reference to the input inverter
    public List<Battery> outputBatteries = new List<Battery>();  // List of batteries to store excess power
    public PowerGrid outputGrid;  // Reference to the output grid
    // I/O

    public float powerInput;

    void Update()
    {
        // Only functionality for one battery for now
        foreach (Battery battery in outputBatteries)
        {  
            // If battery isn't full, send it power
            if (battery.isFull() == false)
            {
                battery.powerInput = powerInput;
            }
            else
            {
                battery.powerInput = 0;
                outputGrid.powerInput = powerInput;
            }
        }
    }
}