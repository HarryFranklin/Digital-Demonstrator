using UnityEngine;

public class Battery : MonoBehaviour
{
    /**
    Current stage: BATTERY
    Next stage: GRID
    **/

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
        if (storedPower < capacity)
        {
            storedPower += powerInput;
            if (storedPower > capacity)
            {
                storedPower = capacity;
            }
        }
        else
        {
            outputGrid.powerInput = powerInput;
        }
    }

    public bool isFull()
    {
        return (storedPower >= capacity);
    }
}