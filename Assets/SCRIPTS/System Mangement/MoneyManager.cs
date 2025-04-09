using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [Header("References")]
    public PowerSystemManager powerSystemManager;
    public MoneyManagerUI UIManager;
    
    [Header("Financial Settings")]
    public float basePricePerUnit = 0.15f; // Base price per kWh
    public float hourDuration = 10f; // How many seconds constitute an "hour" in game
    
    [Header("Insurance Settings")]
    public float powerOutageThreshold = 0.1f; // Power below 10% is considered an outage
    public float outageTimerThreshold = 10f; // Seconds before insurance payout
    public float insurancePayout = 50f; // Fixed payout value
    
    private bool showFinancials = false;
    private bool showTotalData = false;
    
    // Live data variables
    private float totalPotentialRevenue = 0f;
    private float actualRevenue = 0f;
    private float revenueLoss = 0f;
    
    // Cumulative totals
    private float cumulativePotentialRevenue = 0f;
    private float cumulativeActualRevenue = 0f;
    private float cumulativeRevenueLoss = 0f;
    private float cumulativePowerDemand = 0f;
    private float cumulativePowerSupplied = 0f;
    private float cumulativePowerDeficit = 0f;
    
    // Tracking variables
    private float periodTimer = 0f;
    private float hourlyTimer = 0f;
    private float updateInterval = 1f;
    
    // Consumer outage tracking for insurance
    private Dictionary<Consumer, float> consumerOutages = new Dictionary<Consumer, float>();
    private float insurancePayoutTotal = 0f;
    
    private void Start()
    {
        StartCoroutine(UpdateMoneySystem());
    }
    
    private IEnumerator UpdateMoneySystem()
    {
        while (true)
        {
            if (powerSystemManager != null)
            {
                CalculateRevenue();
                CheckForOutages();
                UpdateHourlyData();
                
                if (UIManager != null)
                {
                    UpdateUIData();
                }
                
                // Increment timers with intervals
                periodTimer += updateInterval;
                hourlyTimer += updateInterval;
            }
            
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    private void CalculateRevenue()
    {
        float totalDemand = 0f;
        float totalSupplied = 0f;
        
        totalPotentialRevenue = 0f;
        actualRevenue = 0f;
        
        foreach (var consumer in powerSystemManager.consumers)
        {
            if (consumer != null && consumer.isOperational)
            {
                totalDemand += consumer.GetPowerDemand();
                totalSupplied += consumer.currentPower;
                
                float potentialRevenue = consumer.GetPowerDemand() * basePricePerUnit;
                totalPotentialRevenue += potentialRevenue;
                
                float satisfactionRatio = (consumer.GetPowerDemand() > 0) ? 
                    Mathf.Clamp01(consumer.currentPower / consumer.GetPowerDemand()) : 0f;
                    
                actualRevenue += potentialRevenue * satisfactionRatio;
            }
        }
        
        revenueLoss = totalPotentialRevenue - actualRevenue;
    }
    
    private void UpdateHourlyData()
    {
        if (hourlyTimer >= hourDuration)
        {
            float hourlyMultiplier = hourDuration / updateInterval;
            
            float totalDemand = 0f;
            float totalSupplied = 0f;
            
            foreach (var consumer in powerSystemManager.consumers)
            {
                if (consumer != null && consumer.isOperational)
                {
                    totalDemand += consumer.GetPowerDemand();
                    totalSupplied += consumer.currentPower;
                }
            }
            
            float powerDeficit = totalDemand - totalSupplied;
            
            cumulativePotentialRevenue += totalPotentialRevenue * hourlyMultiplier;
            cumulativeActualRevenue += actualRevenue * hourlyMultiplier;
            cumulativeRevenueLoss += revenueLoss * hourlyMultiplier;
            cumulativePowerDemand += totalDemand;
            cumulativePowerSupplied += totalSupplied;
            cumulativePowerDeficit += powerDeficit;
            
            Debug.Log($"Hour completed! Added £{totalPotentialRevenue * hourlyMultiplier:F2} potential, £{actualRevenue * hourlyMultiplier:F2} actual to totals.");
            
            hourlyTimer = 0f;
        }
    }
    
    private void CheckForOutages()
    {
        // Check each consumer for outages
        foreach (var consumer in powerSystemManager.consumers)
        {
            if (consumer == null || !consumer.isOperational) continue; // Ignore if no consumer or consumer is not operational
            
            float satisfactionRatio = (consumer.GetPowerDemand() > 0) ? consumer.currentPower / consumer.GetPowerDemand() : 0f;
            
            // If power is below the outage threshold (severe power shortage)
            if (satisfactionRatio < powerOutageThreshold)
            {
                // Initialise or increment outage timer
                if (!consumerOutages.ContainsKey(consumer))
                {
                    consumerOutages[consumer] = 0f;
                }
                consumerOutages[consumer] += updateInterval;
                
                // Check if outage duration exceeds the timer threshold
                if (consumerOutages[consumer] >= outageTimerThreshold && 
                    consumerOutages[consumer] < outageTimerThreshold + updateInterval) // Ensure payout triggers only once
                {
                    // Trigger insurance payout
                    insurancePayoutTotal += insurancePayout;
                    cumulativeRevenueLoss += insurancePayout; // Add payout to deficit total
                    Debug.Log($"Insurance payout triggered for {consumer.gameObject.name}: £{insurancePayout}");
                }
            }
            else
            {
                // Reset outage timer if power is restored
                consumerOutages.Remove(consumer);
            }
        }
    }
    
    private void UpdateUIData()
    {
        int minutes = Mathf.FloorToInt(periodTimer / 60);
        int seconds = Mathf.FloorToInt(periodTimer % 60);
        string periodString = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        // Reset to 0
        float totalDemand = 0f;
        float totalSupplied = 0f;
        
        foreach (var consumer in powerSystemManager.consumers)
        {
            if (consumer != null && consumer.isOperational) // Check there's an operational consumer
            {
                totalDemand += consumer.GetPowerDemand();
                totalSupplied += consumer.currentPower;
            }
        }
        
        // Calculate power deficit
        float powerDeficit = totalDemand - totalSupplied;
        
        // Update UIData object for UI updates
        UIData data = new UIData
        {
            PeriodTime = periodString,
            ShowFinancials = showFinancials,
            ShowTotalData = showTotalData,
            
            CurrentPowerDemand = totalDemand,
            CurrentPowerSupplied = totalSupplied,
            CurrentPowerDeficit = powerDeficit,
            
            CurrentPotentialRevenue = totalPotentialRevenue,
            CurrentActualRevenue = actualRevenue,
            CurrentRevenueLoss = revenueLoss,
            
            TotalPowerDemand = cumulativePowerDemand,
            TotalPowerSupplied = cumulativePowerSupplied,
            TotalPowerDeficit = cumulativePowerDeficit,
            
            TotalPotentialRevenue = cumulativePotentialRevenue,
            TotalActualRevenue = cumulativeActualRevenue,
            TotalRevenueLoss = cumulativeRevenueLoss,
            
            InsurancePayoutTotal = insurancePayoutTotal,
            HourDuration = hourDuration
        };
        
        UIManager.UpdateUIWithData(data);
    }
    
    // Update UI display mode for financial/power
    public void SetDisplayMode(bool isFinancial)
    {
        showFinancials = isFinancial;
        // Instantly update UI on click
        if (UIManager != null) UpdateUIData();
    }
    
    // Update UI display mode for total/hourly data
    public void SetDataViewMode(bool isTotal)
    {
        showTotalData = isTotal;
        // Instantly update on click
        if (UIManager != null) UpdateUIData();
    }
    

    // Reset data on reset period button click
    public void ResetPeriod()
    {
        periodTimer = 0f;
        hourlyTimer = 0f;
        
        totalPotentialRevenue = 0f;
        actualRevenue = 0f;
        revenueLoss = 0f;
        
        cumulativePotentialRevenue = 0f;
        cumulativeActualRevenue = 0f;
        cumulativeRevenueLoss = 0f;
        cumulativePowerDemand = 0f;
        cumulativePowerSupplied = 0f;
        cumulativePowerDeficit = 0f;
        
        insurancePayoutTotal = 0f;
        consumerOutages.Clear();
        
        if (UIManager != null) UpdateUIData();
    }
}