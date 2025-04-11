using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Buttons and Panels")]
    public Button controlPanelButton;
    public GameObject controlPanelPanel;
    
    public Button cyberAttackLauncherButton;
    public GameObject cyberAttackLauncherPanel;
    
    public Button financialPanelButton;
    public GameObject financialPanelPanel;
    
    [Header("Control Panel Buttons")]
    public Button emergencyStopButton;
    public Button resetButton;
    public Button demandMatchingButton;
    public TextMeshProUGUI demandMatchingButtonText;
    
    [Header("Button Stacking")]
    public bool areButtonsStacked = true;
    public Button topButton;
    public Button bottomButton;
    
    [Header("System References")]
    public PowerSystemManager powerSystemManager;
    
    // Store original positions
    private Vector3 controlButtonOriginalPos;
    private Vector3 cyberButtonOriginalPos;
    private Vector3 financialButtonOriginalPos;

    private void Start()
    {
        // Deactivate all panels on start
        controlPanelPanel.SetActive(false);
        cyberAttackLauncherPanel.SetActive(false);
        financialPanelPanel.SetActive(false);
        
        // Store original positions
        controlButtonOriginalPos = controlPanelButton.transform.position;
        cyberButtonOriginalPos = cyberAttackLauncherButton.transform.position;
        financialButtonOriginalPos = financialPanelButton.transform.position;
        
        // Add listeners to buttons
        controlPanelButton.onClick.AddListener(ToggleControlPanel);
        cyberAttackLauncherButton.onClick.AddListener(ToggleCyberAttackPanel);
        financialPanelButton.onClick.AddListener(ToggleFinancialPanel);
        
        // Setup control panel buttons
        SetupControlPanelButtons();
    }
    
    private void SetupControlPanelButtons()
    {
        // Initial state - hide reset button, show emergency stop
        resetButton.gameObject.SetActive(false);
        emergencyStopButton.gameObject.SetActive(true);
        
        // Set demand matching button text
        if (demandMatchingButtonText == null)
        {
            demandMatchingButtonText = demandMatchingButton.GetComponentInChildren<TextMeshProUGUI>();
        }
        
        // Add listeners for control panel buttons
        emergencyStopButton.onClick.AddListener(ActivateEmergencyStop);
        resetButton.onClick.AddListener(ResetEmergencyStop);
        demandMatchingButton.onClick.AddListener(ToggleDemandMatching);
        
        // Set initial demand matching button text
        UpdateDemandMatchingButtonText();
    }
    
    private void ActivateEmergencyStop()
    {
        if (powerSystemManager != null)
        {
            // Activate emergency shutdown in power system
            powerSystemManager.emergencyShutdown = true;
            
            // Toggle button visibility
            emergencyStopButton.gameObject.SetActive(false);
            resetButton.gameObject.SetActive(true);
        }
    }
    
    private void ResetEmergencyStop()
    {
        if (powerSystemManager != null)
        {
            // Reset turbines to automatic operation
            powerSystemManager.ResetTurbinesToAutomatic();
            
            // Toggle button visibility
            resetButton.gameObject.SetActive(false);
            emergencyStopButton.gameObject.SetActive(true);
        }
    }
    
    private void ToggleDemandMatching()
    {
        if (powerSystemManager != null)
        {
            // Toggle demand matching in power system
            powerSystemManager.SetDemandMatchingEnabled(!powerSystemManager.enableDemandMatching);
            
            // Update button text
            UpdateDemandMatchingButtonText();
        }
    }
    
    private void UpdateDemandMatchingButtonText()
    {
        if (demandMatchingButtonText != null && powerSystemManager != null)
        {
            if (powerSystemManager.enableDemandMatching)
            {
                demandMatchingButtonText.text = "Disable\nDemand\nMatching";
            }
            else
            {
                demandMatchingButtonText.text = "Enable\nDemand\nMatching";
            }
        }
    }
    
    private void ToggleControlPanel()
    {
        // If cyber panel is active, deactivate it first
        if (cyberAttackLauncherPanel.activeSelf)
        {
            cyberAttackLauncherPanel.SetActive(false);
            // Reset cyber button position
            cyberAttackLauncherButton.transform.position = cyberButtonOriginalPos;
        }
        
        // Toggle control panel and move related buttons
        bool isActive = !controlPanelPanel.activeSelf;
        
        if (isActive)
        {
            // Get panel height
            float panelHeight = controlPanelPanel.GetComponent<RectTransform>().rect.height;
            
            // Move the control button itself up
            controlPanelButton.transform.position = new Vector3(
                controlButtonOriginalPos.x,
                controlButtonOriginalPos.y + panelHeight,
                controlButtonOriginalPos.z
            );
            
            // Check if we need to move the stacked button
            if (areButtonsStacked && bottomButton == controlPanelButton && topButton == cyberAttackLauncherButton)
            {
                // Move cyber button up too since it's stacked above
                cyberAttackLauncherButton.transform.position = new Vector3(
                    cyberButtonOriginalPos.x,
                    cyberButtonOriginalPos.y + panelHeight,
                    cyberButtonOriginalPos.z
                );
            }
            
            // Sync UI with current state
            UpdateDemandMatchingButtonText();
            
            // Activate the panel after moving buttons
            controlPanelPanel.SetActive(true);
        }
        else
        {
            // Deactivate panel
            controlPanelPanel.SetActive(false);
            
            // Reset button positions
            controlPanelButton.transform.position = controlButtonOriginalPos;
            
            // Reset stacked button if needed
            if (areButtonsStacked && bottomButton == controlPanelButton && topButton == cyberAttackLauncherButton)
            {
                cyberAttackLauncherButton.transform.position = cyberButtonOriginalPos;
            }
        }
    }
    
    private void ToggleCyberAttackPanel()
    {
        // If control panel is active, deactivate it first
        if (controlPanelPanel.activeSelf)
        {
            controlPanelPanel.SetActive(false);
            // Reset control button position
            controlPanelButton.transform.position = controlButtonOriginalPos;
        }
        
        // Toggle cyber attack panel and move related buttons
        bool isActive = !cyberAttackLauncherPanel.activeSelf;
        
        if (isActive)
        {
            // Get panel height
            float panelHeight = cyberAttackLauncherPanel.GetComponent<RectTransform>().rect.height;
            
            // Move the cyber button itself up
            cyberAttackLauncherButton.transform.position = new Vector3(
                cyberButtonOriginalPos.x,
                cyberButtonOriginalPos.y + panelHeight,
                cyberButtonOriginalPos.z
            );
            
            // Check if we need to move the stacked button
            if (areButtonsStacked && bottomButton == cyberAttackLauncherButton && topButton == controlPanelButton)
            {
                // Move control button up too since it's stacked above
                controlPanelButton.transform.position = new Vector3(
                    controlButtonOriginalPos.x,
                    controlButtonOriginalPos.y + panelHeight,
                    controlButtonOriginalPos.z
                );
            }
            
            // Activate the panel after moving buttons
            cyberAttackLauncherPanel.SetActive(true);
        }
        else
        {
            // Deactivate panel
            cyberAttackLauncherPanel.SetActive(false);
            
            // Reset button positions
            cyberAttackLauncherButton.transform.position = cyberButtonOriginalPos;
            
            // Reset stacked button if needed
            if (areButtonsStacked && bottomButton == cyberAttackLauncherButton && topButton == controlPanelButton)
            {
                controlPanelButton.transform.position = controlButtonOriginalPos;
            }
        }
    }
    
    private void ToggleFinancialPanel()
    {
        // Toggle the financial panel without affecting button position as it's independent from the others
        financialPanelPanel.SetActive(!financialPanelPanel.activeSelf);
    }
}