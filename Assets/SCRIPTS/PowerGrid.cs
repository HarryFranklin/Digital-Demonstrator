using UnityEngine;
using System.Collections.Generic;

public class PowerGrid : ElectricalComponent
{
    public Transformer inputTransformer;
    public Battery inputBattery;
    public List<Consumer> outputConsumers = new List<Consumer>();

    public override void ReceivePower(float power)
    {
        SetOutputPower(power);
    }

    void Update()
    {
        float totalDemand = 0;
        foreach (Consumer consumer in outputConsumers)
        {
            totalDemand += consumer.outputConsumption;
        }

        float providedPower = ProvidePower(totalDemand);

        // If transformer power isn't enough, pull from battery
        if (providedPower < totalDemand && inputBattery != null)
        {
            float batteryPower = inputBattery.Discharge(totalDemand - providedPower);
            providedPower += batteryPower;
        }

        // Distribute power to consumers
        foreach (Consumer consumer in outputConsumers)
        {
            if (providedPower >= consumer.outputConsumption)
            {
                consumer.ReceivePower(consumer.outputConsumption);
                providedPower -= consumer.outputConsumption;
            }
            else
            {
                consumer.ReceivePower(providedPower);
                Debug.LogWarning($"{consumer.gameObject.name} hasn't enough power!");
                providedPower = 0;
            }
        }
    }
}