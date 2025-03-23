using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyManagerUI : MonoBehaviour
{
    [Header("References")]
    public MoneyManager moneyManager;
    
    [Header("Toggle UI Elements")]
    public Toggle powerFinancialToggle;
    public Text displayModeLabel;  
    public Toggle historicalLiveToggle;
    public Text dataViewLabel; 

    
    [Header("Text Display Elements")]
    public TextMeshProUGUI line1Text; // Potential Power/Revenue
    public TextMeshProUGUI line2Text; // Actual Power/Revenue
    public TextMeshProUGUI line3Text; // Deficit Power/Revenue
    public TextMeshProUGUI periodText; // Period count 00:00 
    
    [Header("Button")]
    public Button resetPeriodButton; // Period reset button
    
    private void Start()
    {   
        // Set up event listeners for UI controls
        SetupEventListeners(true);
        
        // Initialise toggle labels
        UpdateToggleLabels();
    }
    
    private void SetupEventListeners(bool isAdding)
    {
        // Check and add/remove listeners for each toggle and button
        if (powerFinancialToggle != null)
        {
            if (isAdding)
                powerFinancialToggle.onValueChanged.AddListener(OnDisplayModeToggled);
            else
                powerFinancialToggle.onValueChanged.RemoveListener(OnDisplayModeToggled);
        }
        
        if (historicalLiveToggle != null)
        {
            if (isAdding)
                historicalLiveToggle.onValueChanged.AddListener(OnDataViewToggled);
            else
                historicalLiveToggle.onValueChanged.RemoveListener(OnDataViewToggled);
        }
        
        if (resetPeriodButton != null)
        {
            if (isAdding)
                resetPeriodButton.onClick.AddListener(OnResetPeriodClicked);
            else
                resetPeriodButton.onClick.RemoveListener(OnResetPeriodClicked);
        }
    }
    
    // Financial/Power display mode toggle
    private void OnDisplayModeToggled(bool isFinancial)
    {
        if (moneyManager != null)
        {
            moneyManager.SetDisplayMode(isFinancial);
        }
        UpdateToggleLabels();
    }
    
    // Historical/Live data view toggle
    private void OnDataViewToggled(bool isTotal)
    {
        if (moneyManager != null)
        {
            moneyManager.SetDataViewMode(isTotal);
        }
        UpdateToggleLabels();
    }
    
    // Reset period button
    private void OnResetPeriodClicked()
    {
        if (moneyManager != null)
        {
            moneyManager.ResetPeriod();
        }
    }
    
    private void UpdateToggleLabels()
    {
        // Update display mode toggle label
        if (displayModeLabel != null)
        {
            displayModeLabel.text = powerFinancialToggle.isOn ? "Financial View" : "Power View";
        }

        // Update data view toggle label
        if (dataViewLabel != null)
        {
            dataViewLabel.text = historicalLiveToggle.isOn ? "Total View" : "Current View";
        }
    }

    // Use UIData object to update UI text elements
    public void UpdateUIWithData(UIData data)
    {
        if (periodText != null)
        {
            periodText.text = $"Period: {data.PeriodTime}";
        }
        
        UpdateTextDisplay(data);
    }
    
    // Use UIData object to update UI text elements
    private void UpdateTextDisplay(UIData data)
    {
        if (data.ShowFinancials)
        {
            UpdateFinancialDisplay(data);
        }
        else
        {
            UpdatePowerDisplay(data);
        }
    }
    
    // Update and format UI text for each field
    private void UpdateFinancialDisplay(UIData data)
    {
        if (data.ShowTotalData)
        {
            SetTextLines(
                $"Total Potential Revenue: £{data.TotalPotentialRevenue:F2}",
                $"Total Actual Revenue: £{data.TotalActualRevenue:F2}",
                $"Total Revenue Loss: £{data.TotalRevenueLoss:F2} (Insurance Paid: £{data.InsurancePayoutTotal:F2})"
            );
        }
        else
        {
            float hourlyRate = data.HourDuration / 1f;
            SetTextLines(
                $"Potential Revenue: £{data.CurrentPotentialRevenue * hourlyRate:F2}/h",
                $"Actual Revenue: £{data.CurrentActualRevenue * hourlyRate:F2}/h",
                $"Revenue Loss: £{data.CurrentRevenueLoss * hourlyRate:F2}/h"
            );
        }
    }
    
    private void UpdatePowerDisplay(UIData data)
    {
        if (data.ShowTotalData)
        {
            SetTextLines(
                $"Total Power Needed: {data.TotalPowerDemand:F2} kWh",
                $"Total Power Supplied: {data.TotalPowerSupplied:F2} kWh",
                $"Total Power Deficit: {data.TotalPowerDeficit:F2} kWh"
            );
        }
        else
        {
            SetTextLines(
                $"Power Needed: {data.CurrentPowerDemand:F2} kW",
                $"Power Supplied: {data.CurrentPowerSupplied:F2} kW",
                $"Power Deficit: {data.CurrentPowerDeficit:F2} kW"
            );
        }
    }
    
    private void SetTextLines(string line1, string line2, string line3)
    {
        if (line1Text != null) line1Text.text = line1;
        if (line2Text != null) line2Text.text = line2;
        if (line3Text != null) line3Text.text = line3;
    }
    
    private void OnDestroy()
    {
        SetupEventListeners(false);
    }
}