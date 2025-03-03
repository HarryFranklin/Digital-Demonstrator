using UnityEngine;

public class Consumer : MonoBehaviour
{
    /**
    Current stage: CONSUMER
    **/

    // I/O
    public PowerGrid inputGrid; // One grid per consumer.
    public float outputConsumption = 20f; // Float variable for power consumption of this consumer.
    // I/O

    public void ReceivePower(float power)
    {
        Debug.Log($"Consumer received {power} units of power.");
    }

    public float GetPowerDemand()
    {
        return outputConsumption;
    }
}
