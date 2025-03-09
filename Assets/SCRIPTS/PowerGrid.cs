using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerGrid : PowerComponentBase
{
    private List<Consumer> connectedConsumers = new List<Consumer>();
    private Dictionary<IPowerComponent, float> powerSources = new Dictionary<IPowerComponent, float>();
    private float totalAvailablePower = 0f;
    
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(UpdateVisualisationRoutine(0.1f));
    }
    
    void Update()
    {
        if (!isOperational)
        {
            currentPower = 0f;
            return;
        }
        
        // Sum up all available power
        currentPower = totalAvailablePower;
        
        // Distribute power to consumers
        DistributePower();
        
        // Reset available power for next frame
        totalAvailablePower = 0f;
        powerSources.Clear();
    }
    
    // Register a consumer
    public void RegisterConsumer(Consumer consumer)
    {
        if (!connectedConsumers.Contains(consumer))
        {
            connectedConsumers.Add(consumer);
        }
    }
    
    // Unregister a consumer
    public void UnregisterConsumer(Consumer consumer)
    {
        connectedConsumers.Remove(consumer);
    }
    
    // Receive power from a source (transformer or battery)
    public void ReceivePower(IPowerComponent source, float power)
    {
        powerSources[source] = power;
        totalAvailablePower += power;
    }
    
    // Get total power demand from all consumers
    public float GetTotalDemand()
    {
        float totalDemand = 0f;
        foreach (var consumer in connectedConsumers)
        {
            if (consumer != null && consumer.IsOperational())
            {
                totalDemand += consumer.GetPowerDemand();
            }
        }
        return totalDemand;
    }
    
    // Distribute available power to consumers
    private void DistributePower()
    {
        float totalDemand = GetTotalDemand();
        
        // If no consumers or no demand, nothing to do
        if (connectedConsumers.Count == 0 || totalDemand <= 0) return;
        
        // If enough power, give each consumer what they need
        if (currentPower >= totalDemand)
        {
            foreach (var consumer in connectedConsumers)
            {
                if (consumer != null && consumer.IsOperational())
                {
                    consumer.ReceivePower(consumer.GetPowerDemand());
                }
            }
        }
        // Otherwise, distribute proportionally
        else
        {
            float ratio = currentPower / totalDemand;
            foreach (var consumer in connectedConsumers)
            {
                if (consumer != null && consumer.IsOperational())
                {
                    float allocation = consumer.GetPowerDemand() * ratio;
                    consumer.ReceivePower(allocation);
                }
            }
        }
    }
    
    public override void VisualiseConnections()
    {
        if (visualiser == null) return;
        
        // Visualise connections from sources (transformer, battery)
        foreach (var source in powerSources.Keys)
        {
            if (source is MonoBehaviour mb)
            {
                visualiser.CreateOrUpdateConnection(mb.gameObject, gameObject, powerSources[source]);
            }
        }
        
        // Visualise connections to consumers
        foreach (var consumer in connectedConsumers)
        {
            if (consumer != null)
            {
                float allocatedPower = 0;
                float requiredPower = consumer.GetPowerDemand();
                
                // Calculate actual power sent to this consumer
                if (GetTotalDemand() > 0)
                {
                    if (currentPower >= GetTotalDemand())
                    {
                        allocatedPower = requiredPower;
                    }
                    else
                    {
                        allocatedPower = requiredPower * (currentPower / GetTotalDemand());
                    }
                }
                
                visualiser.CreateOrUpdateConnection(gameObject, consumer.gameObject, allocatedPower, requiredPower);
            }
        }
    }
}