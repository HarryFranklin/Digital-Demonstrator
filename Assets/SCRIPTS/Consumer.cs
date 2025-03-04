using UnityEngine;

public class Consumer : MonoBehaviour
{
    /**
    Current stage: CONSUMER
    **/

    // I/O
    public PowerGrid inputGrid;  // Reference to the grid this consumer is connected to
    public float powerInput;  // Amount of power this consumer receives from the grid
    public float powerConsumption = 20f;  // Amount of power this consumer requires
    // I/O

    void Update()
    {
        // Functionality added later
        if (powerInput == powerConsumption)
        {
            Debug.Log("Power Demands Met");
        }
        if (powerInput < powerConsumption)
        {
            Debug.Log("Not Enough Power");
        }
    }
}