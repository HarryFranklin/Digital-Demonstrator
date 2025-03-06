using UnityEngine;
using System.Collections.Generic;

public class PowerGrid : MonoBehaviour
{
    public List<Consumer> consumers;
    public Transformer inputTransformer;
    public Battery inputBattery;
    public float powerInput;
    public float totalConsumption;

    private PowerLineConnector powerLine;

    void Start()
    {
        powerLine = GetComponent<PowerLineConnector>();
    }

    void Update()
    {
        // Reset total consumption and recalculate
        totalConsumption = 0f;
        foreach (Consumer consumer in consumers)
        {
            totalConsumption += consumer.powerConsumption;
        }

        // Aggregate power from transformer and battery
        powerInput = 0f;
        if (inputTransformer != null) powerInput += inputTransformer.powerInput;
        if (inputBattery != null) powerInput += inputBattery.powerOutput;

        // Update visualisation
        if (powerLine != null)
        {
            powerLine.powerFlow = powerInput;
        }

        // Distribute power to consumers
        float availablePower = powerInput;
        foreach (Consumer consumer in consumers)
        {
            float allocatedPower = Mathf.Min(consumer.powerConsumption, availablePower);
            consumer.powerInput = allocatedPower;
            availablePower -= allocatedPower;
        }
    }
}
