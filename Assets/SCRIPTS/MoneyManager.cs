using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    [Header("References")]
    public PowerSystemManager powerSystemManager;
    
    [Header("UI Elements")]
    public Toggle displayModeToggle; // Toggle between power/financial display
    public TextMeshProUGUI powerAndPotentialRevenueText; // Power needed / Total potential revenue
    public TextMeshProUGUI powerSuppliedActualRevenue; // Power supplied / Actual revenue
    public TextMeshProUGUI powerDeficitRevenueLoss; // Power deficit / Revenue loss
    public TextMeshProUGUI periodText; // Shows current period timing
    
    [Header("Financial Settings")]
    public float basePricePerUnit = 0.15f; // Base price per kWh
    public float insuranceThreshold = 10f; // Seconds before insurance payout
    public float insurancePayout = 50f; // Fixed payout value

    private bool showFinancials = false; // Toggle between power/financial display
    private float totalPotentialRevenue = 0f;
    private float actualRevenue = 0f;
    private float revenueLoss = 0f;
    
    // Tracking variables
    private float periodTimer = 0f;
    private float updateInterval = 1f; // Update UI every second
    
    // Consumer outage tracking for insurance
    private Dictionary<Consumer, float> consumerOutages = new Dictionary<Consumer, float>();
    private float insurancePayoutTotal = 0f;
    
    private void Start()
    {
        // Initialise
        if (displayModeToggle != null)
        {
            displayModeToggle.onValueChanged.AddListener(OnDisplayModeToggled); // for toggle button
            showFinancials = displayModeToggle.isOn;
        }
        
        StartCoroutine(UpdateMoneySystem());
    }
    
    private IEnumerator UpdateMoneySystem()
    {
        while (true)
        {
            // Check for powerSystemManager
            if (powerSystemManager != null)
            {
                CalculateRevenue();
                TrackOutage();
                UpdateUI();
                
                // Increment period timer
                periodTimer += updateInterval;
            }
            
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    private void CalculateRevenue()
    {
        float totalDemand = 0f;
        float totalSupplied = 0f;
        
        // Reset totals for this interval
        totalPotentialRevenue = 0f;
        actualRevenue = 0f;
        
        // Check each consumer
        foreach (var consumer in powerSystemManager.consumers)
        {
            if (consumer != null && consumer.isOperational)
            {
                // Update demand and supplies
                totalDemand += consumer.GetPowerDemand();
                totalSupplied +=  consumer.currentPower;
                
                // Calculate potential revenue for this consumer
                float potentialRevenue = consumer.GetPowerDemand() * basePricePerUnit;
                totalPotentialRevenue += potentialRevenue;
                
                // Calculate actual revenue based on what was delivered
                float satisfactionRatio = (consumer.GetPowerDemand() > 0) ? 
                    Mathf.Clamp01( consumer.currentPower / consumer.GetPowerDemand()) : 0f;
                float actualConsumerRevenue = potentialRevenue * satisfactionRatio;
                actualRevenue += actualConsumerRevenue;
            }
        }
        
        // Calculate revenue loss
        revenueLoss = totalPotentialRevenue - actualRevenue;
    }
    
    private void TrackOutage()
    {
        // Check each consumer for outages using condition of less than 10% power supplied
        foreach (var consumer in powerSystemManager.consumers)
        {
            if (consumer == null || !consumer.isOperational) continue;
            
            float satisfactionRatio = (consumer.GetPowerDemand() > 0) ? 
                consumer.currentPower / consumer.GetPowerDemand() : 0f;
            
            // If severe power shortage (less than 10% of required)
            if (satisfactionRatio < 0.1f)
            {
                // Initialise or increment outage timer
                if (!consumerOutages.ContainsKey(consumer))
                {
                    consumerOutages[consumer] = 0f;
                }
                consumerOutages[consumer] += updateInterval;
                
                // Check for insurance threshold
                if (consumerOutages[consumer] >= insuranceThreshold && 
                    consumerOutages[consumer] < insuranceThreshold + updateInterval) // Ensure it is counted only once
                {
                    // Trigger insurance payout
                    insurancePayoutTotal += insurancePayout;
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
    
    private void UpdateUI()
    {
        // Format period timer
        int minutes = Mathf.FloorToInt(periodTimer / 60);
        int seconds = Mathf.FloorToInt(periodTimer % 60);
        string periodString = string.Format("Period: {0:00}:{1:00}", minutes, seconds);
        
        if (periodText != null)
        {
            periodText.text = periodString;
        }
        
        // Calculate total power metrics from all consumers
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
        
        // Update UI based on display mode
        if (showFinancials)
        {
            // Financial display
            if (powerAndPotentialRevenueText != null) powerAndPotentialRevenueText.text = $"Potential Revenue: £{totalPotentialRevenue:F2}";
            if (powerSuppliedActualRevenue != null) powerSuppliedActualRevenue.text = $"Actual Revenue: £{actualRevenue:F2}";
            if (powerDeficitRevenueLoss != null) powerDeficitRevenueLoss.text = $"Revenue Loss: £{revenueLoss:F2} (Insurance: £{insurancePayoutTotal:F2})";
        }
        else
        {
            // Power display
            if (powerAndPotentialRevenueText != null) powerAndPotentialRevenueText.text = $"Power Needed: {totalDemand:F2} kW";
            if (powerSuppliedActualRevenue != null) powerSuppliedActualRevenue.text = $"Power Supplied: {totalSupplied:F2} kW";
            if (powerDeficitRevenueLoss != null) powerDeficitRevenueLoss.text = $"Power Deficit: {powerDeficit:F2} kW";
        }
    }
    
    // Handle toggle change
    private void OnDisplayModeToggled(bool isFinancial)
    {
        showFinancials = isFinancial;
        UpdateUI();
    }
    
    // Public method to reset the period
    public void ResetPeriod()
    {
        periodTimer = 0f;
        totalPotentialRevenue = 0f;
        actualRevenue = 0f;
        revenueLoss = 0f;
        insurancePayoutTotal = 0f;
        consumerOutages.Clear();
        UpdateUI();
    }
    
    // Method to get current financial status (for other systems)
    public (float potential, float actual, float loss, float insurance) GetFinancialStatus()
    {
        return (totalPotentialRevenue, actualRevenue, revenueLoss, insurancePayoutTotal);
    }
    
    private void OnDestroy()
    {
        // Clean up
        if (displayModeToggle != null)
        {
            displayModeToggle.onValueChanged.RemoveListener(OnDisplayModeToggled);
        }
    }
}