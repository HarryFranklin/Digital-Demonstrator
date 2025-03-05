using UnityEngine;

public class Battery : MonoBehaviour
{
    // I/O
    public Transformer inputTransformer;
    public PowerGrid outputGrid;
    // I/O

    // Battery Information
    public float capacity = 1000f;  // Battery capacity
    public float storedPower = 0f;  // Power currently stored in the battery
    public float powerInput;
    public float powerOutput; // Power discharged to grid

    void Update()
    {
        // Reset power output each frame
        powerOutput = 0f;

        // If receiving power, store it
        if (powerInput > 0)
        {
            storedPower += powerInput;
            storedPower = Mathf.Min(storedPower, capacity); // Ensure it doesn't exceed max capacity
        }

        // If grid needs extra power, discharge the battery
        float missingPower = outputGrid.totalConsumption - outputGrid.powerInput;

        if (missingPower > 0 && storedPower > 0)
        {
            powerOutput = Mathf.Min(missingPower, storedPower);
            storedPower -= powerOutput;
        }

        // Send discharged power to the grid
        outputGrid.powerInput += powerOutput;
    }

    public bool isFull()
    {
        return storedPower >= capacity;
    }
}