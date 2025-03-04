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

    void Update()
    {
        // If there’s power coming to the battery, store it
        if (powerInput > 0)
        {
            storedPower += powerInput;
            if (storedPower > capacity)
            {
                storedPower = capacity; // Ensure the battery doesn't exceed capacity
            }
        }
        else
        {
            // If there's no power coming to the battery, transfer power to the grid (when battery is full)
            if (storedPower > 0)
            {
                outputGrid.powerInput += storedPower;  // Add stored power to the grid
                storedPower = 0; // Reset stored power after it’s used
            }
        }
    }

    public bool isFull()
    {
        return storedPower >= capacity;
    }
}