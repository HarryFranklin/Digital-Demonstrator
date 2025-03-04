using UnityEngine;

public class Consumer : MonoBehaviour
{
    /**
    Current stage: CONSUMER
    **/

    // I/O
    public PowerGrid inputGrid;  // Reference to the grid this consumer is connected to
    public float powerInput;  // Amount of power this consumer receives from the grid
    public float powerConsumption = 10f;  // Amount of power this consumer requires
    // I/O

    void Update()
    {
        if (powerInput >= powerConsumption)
        {
            Debug.Log($"Consumer {gameObject.name}: Power Demands Met (Received {powerInput} / Required {powerConsumption})");
        }
        else
        {
            Debug.Log($"Consumer {gameObject.name}: Not Enough Power (Received {powerInput} / Required {powerConsumption})");
        }
    }
}