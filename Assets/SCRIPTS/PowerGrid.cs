using UnityEngine;
using System.Collections.Generic;

public class PowerGrid : MonoBehaviour
{
    public Transformer inputTransformer;
    public Battery backupBattery;
    public List<Consumer> consumers;
    private float totalPower;

    public void ReceivePower(float power)
    {
        totalPower += power;
    }

    public float GetPowerDemand()
    {
        float demand = 0;
        foreach (var consumer in consumers)
        {
            demand += consumer.GetPowerDemand();
        }
        return demand;
    }

    void Update()
    {
        float demand = GetPowerDemand();
        float availablePower = totalPower;

        if (availablePower < demand && backupBattery != null)
        {
            float shortfall = demand - availablePower;
            availablePower += backupBattery.Discharge(shortfall);
        }

        foreach (var consumer in consumers)
        {
            consumer.ReceivePower(availablePower / consumers.Count);
        }

        totalPower = 0;
    }
}