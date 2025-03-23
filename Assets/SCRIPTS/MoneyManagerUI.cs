using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyManagerUI : MonoBehaviour
{
    [Header("References")]
    public MoneyManager moneyManager;
    
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public Toggle displayModeToggle;
    public TextMeshProUGUI toggleLabelText;
    public Button periodResetButton;
    
    private void Start()
    {
        if (periodResetButton != null)
        {
            periodResetButton.onClick.AddListener(OnResetPeriodClicked);
        }
        
        // Setup toggle text
        UpdateToggleText(displayModeToggle != null ? displayModeToggle.isOn : false);
        
        if (displayModeToggle != null)
        {
            displayModeToggle.onValueChanged.AddListener(UpdateToggleText);
        }
    }
    
    private void UpdateToggleText(bool isFinancial)
    {
        if (toggleLabelText != null)
        {
            toggleLabelText.text = isFinancial ? "Financial View" : "Power View";
        }
    }
    
    private void OnResetPeriodClicked()
    {
        if (moneyManager != null)
        {
            moneyManager.ResetPeriod();
        }
    }
    
    private void OnDestroy()
    {
        if (periodResetButton != null)
        {
            periodResetButton.onClick.RemoveAllListeners();
        }
        
        if (displayModeToggle != null)
        {
            displayModeToggle.onValueChanged.RemoveAllListeners();
        }
    }
}