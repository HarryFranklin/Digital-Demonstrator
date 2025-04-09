using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Power System Manager
public class PowerSystemManager : MonoBehaviour
{
    [Header("System Components")] // References to all components
    public List<Turbine> turbines = new List<Turbine>();
    public WindFarm windFarm;
    public Inverter inverter;
    public Transformer transformer;
    public Battery battery;
    public PowerGrid powerGrid;
    public List<Consumer> consumers = new List<Consumer>();
    
    [Header("Monitoring")] // Control over status updates (Debug.Log()s)
    public bool debugMode = true;
    public float monitorInterval = 1f;
    
    [Header("Control")]
    public bool emergencyShutdown = false;
    
    [Header("Smart Power Management")] // Smart power management to simulate 
    public bool enableDemandMatching = false;
    public float targetBatteryChargePercentage = 80f;
    public float batteryChargeBuffer = 10f;
    public float minTurbineSpeed = 0f;
    
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
            
            // Perform demand matching if enabled
            if (enableDemandMatching)
            {
                MatchPowerToLoad();
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
    
    // Smart power management to match demand and optimise battery usage
    // Manual power generation to simulate Nacelle yaw and blade pitch
    public void MatchPowerToLoad()
    {
        if (powerGrid == null || turbines.Count == 0) return;
        
        // Calculate total current consumer demand
        float totalDemand = powerGrid.GetTotalDemand();
        
        // Calculate battery parameters for optimal charging
        float batteryChargeRate = 0f;
        if (battery != null && battery.isOperational)
        {
            float currentChargePercentage = battery.GetChargePercentage();
            
            // If battery is below target, allocate power for charging
            if (currentChargePercentage < targetBatteryChargePercentage)
            {
                float chargeDeficit = battery.maxCapacity * (targetBatteryChargePercentage / 100f) - battery.currentCharge;
                // Limit charge rate to 50% of total demand to prevent excessive generation
                batteryChargeRate = Mathf.Min(chargeDeficit / battery.chargeEfficiency, totalDemand * 0.5f);
            }
            // If battery is above target + buffer, no need for additional charging
            else if (currentChargePercentage > (targetBatteryChargePercentage + batteryChargeBuffer))
            {
                batteryChargeRate = 0f;
            }
            // Otherwise maintain a small charge rate to keep the battery at target level
            else
            {
                batteryChargeRate = totalDemand * 0.05f; // Small buffer (5% of demand)
            }
        }
        
        // Calculate required power generation (demand + battery charging)
        float requiredPower = totalDemand + batteryChargeRate;
        
        // Calculate maximum potential power generation
        float maxPotentialPower = 0f;
        foreach (var turbine in turbines)
        {
            if (turbine != null && turbine.isOperational)
            {
                maxPotentialPower += turbine.maxSpeed;
            }
        }
        
        // Calculate power ratio (how much of max capacity we need)
        float powerRatio = Mathf.Clamp01(requiredPower / maxPotentialPower);
        
        // Set all turbines to appropriate speed
        foreach (var turbine in turbines)
        {
            if (turbine != null && turbine.isOperational)
            {
                float optimisedSpeed = Mathf.Max(minTurbineSpeed, turbine.maxSpeed * powerRatio);
                turbine.ToggleManualControl(true, optimisedSpeed);
            }
        }
        
        if (debugMode)
        {
            Debug.Log($"Smart Power Management: Total demand: {totalDemand:F2}, " +
                     $"Battery charge rate: {batteryChargeRate:F2}, Required power: {requiredPower:F2}, " +
                     $"Turbine power ratio: {powerRatio:P0}");
        }
    }
    
    // Toggle demand matching on/off
    public void SetDemandMatchingEnabled(bool enabled)
    {
        enableDemandMatching = enabled;
        
        // If disabling, return turbines to automatic control
        if (!enabled)
        {
            ResetTurbinesToAutomatic();
        }
        else
        {
            // If enabling, run matching immediately
            MatchPowerToLoad();
        }
    }
}