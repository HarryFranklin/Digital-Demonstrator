using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerGrid : PowerComponentBase
{
    [System.Serializable]
    public class PrioritisedConsumer
    {
        public Consumer consumer;
        public string description;
    }
    
    [Header("Consumer Priorities")]
    [Tooltip("Add consumers in priority order. First consumer (index 0) has highest priority.")]
    public List<PrioritisedConsumer> prioritisedConsumers = new List<PrioritisedConsumer>();
    
    private List<Consumer> connectedConsumers = new List<Consumer>();
    private Dictionary<IPowerComponent, float> powerSources = new Dictionary<IPowerComponent, float>();
    private float totalAvailablePower = 0f;
    private float lastTotalPower = 0f;
    private bool needsRecalculation = true;
    
    [Header("Debug")]
    public bool showPowerDistributionDebug = true;
    
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(UpdateVisualisationRoutine(0.1f));
        
        // Add prioritised consumers to connected list
        foreach (var pc in prioritisedConsumers)
        {
            if (pc.consumer != null && !connectedConsumers.Contains(pc.consumer))
                connectedConsumers.Add(pc.consumer);
        }
    }
    
    void Update()
    {
        if (!isOperational)
        {
            currentPower = 0f;
            needsRecalculation = true;
            return;
        }
        
        currentPower = totalAvailablePower;
        
        if (Mathf.Abs(lastTotalPower - currentPower) > 0.01f || needsRecalculation)
        {
            DistributePowerByPriority();
            lastTotalPower = currentPower;
            needsRecalculation = false;
            
            if (showPowerDistributionDebug)
                Debug.Log($"Power distribution recalculated. Total Power: {currentPower}");
        }
        
        totalAvailablePower = 0f;
        powerSources.Clear();
    }
    
    public void RegisterConsumer(Consumer consumer)
    {
        if (!connectedConsumers.Contains(consumer))
        {
            connectedConsumers.Add(consumer);
            
            // Auto-add to prioritised consumers if not already there
            if (!IsPrioritised(consumer))
            {
                prioritisedConsumers.Add(new PrioritisedConsumer { 
                    consumer = consumer,
                    description = "Auto-added"
                });
            }
            
            needsRecalculation = true;
        }
    }
    
    private bool IsPrioritised(Consumer consumer) => 
        prioritisedConsumers.Exists(pc => pc.consumer == consumer);
    
    private int GetPriorityIndex(Consumer consumer)
    {
        for (int i = 0; i < prioritisedConsumers.Count; i++)
        {
            if (prioritisedConsumers[i].consumer == consumer) 
                return i;
        }
        return -1;
    }
    
    public void UnregisterConsumer(Consumer consumer)
    {
        if (connectedConsumers.Remove(consumer))
        {
            // Remove from prioritised list
            prioritisedConsumers.RemoveAll(pc => pc.consumer == consumer);
            needsRecalculation = true;
        }
    }
    
    public void ReceivePower(IPowerComponent source, float power)
    {
        powerSources[source] = power;
        totalAvailablePower += power;
        
        if (Mathf.Abs(totalAvailablePower - lastTotalPower) > 0.01f)
            needsRecalculation = true;
    }
    
    public float GetTotalDemand()
    {
        float totalDemand = 0f;
        foreach (var consumer in connectedConsumers)
        {
            if (consumer != null && consumer.IsOperational())
                totalDemand += consumer.GetPowerDemand();
        }
        return totalDemand;
    }
    
    private void DistributePowerByPriority()
    {
        if (connectedConsumers.Count == 0) return;
        
        // Sort consumers by priority
        List<Consumer> sortedConsumers = new List<Consumer>(connectedConsumers);
        sortedConsumers.Sort((a, b) => {
            int indexA = GetPriorityIndex(a);
            int indexB = GetPriorityIndex(b);
            
            // Sorting
            if (indexA >= 0 && indexB >= 0)
                return indexA.CompareTo(indexB);
            else if (indexA >= 0)
                return -1;
            else if (indexB >= 0)
                return 1;
            
            return 0;
        });
        
        float remainingPower = currentPower;
        
        // Reset all consumers' power
        foreach (var consumer in connectedConsumers)
        {
            if (consumer != null)
                consumer.ReceivePower(0f);
        }
        
        // Distribute by priority order
        foreach (var consumer in sortedConsumers)
        {
            // Sanity check
            if (consumer == null || !consumer.IsOperational())
                continue;
                
            // Get consumer demand and consumer priority index
            float demand = consumer.GetPowerDemand();
            int priorityIndex = GetPriorityIndex(consumer);
            string priorityDesc = (priorityIndex >= 0) ? 
                $"Priority: {priorityIndex} ({prioritisedConsumers[priorityIndex].description})" : 
                "Unregistered";
            
            // Give the consumer as little as it needs to meet demand, rather than an equal share
            float allocated = Mathf.Min(demand, remainingPower);
            consumer.ReceivePower(allocated);
            remainingPower -= allocated;
            
            if (showPowerDistributionDebug)
            {
                string status = allocated >= demand ? "full" : (allocated > 0 ? "partial" : "no");
                Debug.Log($"Consumer {consumer.gameObject.name} ({priorityDesc}) received {status} power: {allocated} of {demand} needed");
            }
            
            if (remainingPower <= 0)
                break;
        }
    }
    
    public override void VisualiseConnections()
    {
        if (visualiser == null) return;
        
        // Visualise source connections
        foreach (var source in powerSources.Keys)
        {
            if (source is MonoBehaviour mb)
                visualiser.CreateOrUpdateConnection(mb.gameObject, gameObject, powerSources[source]);
        }
        
        // Visualise consumer connections
        foreach (var consumer in connectedConsumers)
        {
            if (consumer != null)
            {
                visualiser.CreateOrUpdateConnection(
                    gameObject, 
                    consumer.gameObject, 
                    consumer.GetCurrentPower(), 
                    consumer.GetPowerDemand()
                );
            }
        }
    }
}