using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Power System Manager
public class PowerSystemManager : MonoBehaviour
{
    [Header("System Components")]
    public List<Turbine> turbines = new List<Turbine>();
    public WindFarm windFarm;
    public Inverter inverter;
    public Transformer transformer;
    public Battery battery;
    public PowerGrid powerGrid;
    public List<Consumer> consumers = new List<Consumer>();
    
    [Header("Monitoring")]
    public bool debugMode = true;
    public float monitorInterval = 1f;
    
    [Header("Control")]
    public bool emergencyShutdown = false;
    
    private void Start()
    {
        StartCoroutine(MonitorSystem());
    }
    
    private IEnumerator MonitorSystem()
    {
        while (true)
        {
            if (debugMode)
            {
                // Log system status
                float totalGeneration = 0f;
                foreach (var turbine in turbines)
                {
                    if (turbine != null && turbine.isOperational)
                    {
                        totalGeneration += turbine.GetCurrentPower();
                    }
                }
                
                float totalDemand = 0f;
                foreach (var consumer in consumers)
                {
                    if (consumer != null && consumer.isOperational)
                    {
                        totalDemand += consumer.GetPowerDemand();
                    }
                }
                
                string batteryStatus = battery != null ? $"{battery.GetChargePercentage():F1}%" : "N/A";
                
                Debug.Log($"Power System Status: Generation: {totalGeneration:F1}, Demand: {totalDemand:F1}, Battery: {batteryStatus}");
            }
            
            // Handle emergency shutdown
            if (emergencyShutdown)
            {
                SetAllTurbineWindSpeeds(0f);
                emergencyShutdown = false;
            }
            
            yield return new WaitForSeconds(monitorInterval);
        }
    }
    
    // Method to manually control all turbines
    public void SetAllTurbineWindSpeeds(float speed)
    {
        foreach (var turbine in turbines)
        {
            if (turbine != null)
            {
                turbine.ToggleManualControl(true, speed);
            }
        }
    }
    
    // Method to return turbines to automatic control
    public void ResetTurbinesToAutomatic()
    {
        foreach (var turbine in turbines)
        {
            if (turbine != null)
            {
                turbine.ToggleManualControl(false);
            }
        }
    }
    
    // Method to reduce turbine output when battery is full
    public void HandleBatteryFullScenario()
    {
        if (battery == null) return;
        
        float batteryPercentage = battery.GetChargePercentage();
        float totalDemand = powerGrid != null ? powerGrid.GetTotalDemand() : 0f;
        float totalGeneration = 0f;
        
        foreach (var turbine in turbines)
        {
            if (turbine != null && turbine.isOperational)
            {
                totalGeneration += turbine.GetCurrentPower();
            }
        }
        
        // If battery is almost full and we're generating excess power
        if (batteryPercentage > 95f && totalGeneration > totalDemand * 1.05f)
        {
            // Calculate ideal generation level (slightly above demand)
            float targetGeneration = totalDemand * 1.05f;
            float reductionFactor = targetGeneration / totalGeneration;
            
            // Reduce all turbine speeds proportionally
            foreach (var turbine in turbines)
            {
                if (turbine != null && turbine.isOperational)
                {
                    float currentSpeed = turbine.windSpeed;
                    float newSpeed = currentSpeed * reductionFactor;
                    turbine.ToggleManualControl(true, newSpeed);
                }
            }
            
            Debug.Log($"Battery nearly full - reduced turbine output to match demand. New generation target: {targetGeneration:F1}");
        }
    }
}