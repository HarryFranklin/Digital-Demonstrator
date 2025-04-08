using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CyberAttackManager : MonoBehaviour
{
    [Header("Management System and Visualiser")]
    public PowerVisualiser powerVisualiser;
    public PowerSystemManager powerSystemManager;
    public PowerStatusIndicatorManager powerStatusIconManager;
    
    [Header("Attack Controls")]
    public Button visualDisruptionButton;
    public Button safetyOverrideButton;
    public Button monitoringDisruptionButton;
    public Button powerGenerationButton;
    public Button substationAttackButton; // New button for substation attack
    
    // Dictionary of all available attacks
    private Dictionary<string, CyberAttackBase> attacks = new Dictionary<string, CyberAttackBase>();
    
    private void Awake()
    {
        // Create attack objects
        CreateAttacks();
    }
    
    private void Start()
    {
        // Initialise all attacks
        foreach (var attack in attacks.Values)
        {
            attack.Initialise(powerVisualiser, powerSystemManager);
        }
        
        // Set up button listeners
        SetupButtons();
        
        // Update the PowerVisualiser to use this manager for turbine manipulation checking
        if (powerVisualiser != null)
        {
            powerVisualiser.cyberAttack = this;
        }
    }
    
    private void CreateAttacks()
    {
        // Create each attack and add to dictionary
        
        // Visual Disruption Attack
        VisualDisruptionAttack visualDisruptionAttack = gameObject.AddComponent<VisualDisruptionAttack>();
        visualDisruptionAttack.SetPowerStatusIndicator(powerStatusIconManager);
        attacks.Add(visualDisruptionAttack.AttackName, visualDisruptionAttack);
        
        // Safety Override Attack
        SafetyOverrideAttack safetyOverrideAttack = gameObject.AddComponent<SafetyOverrideAttack>();
        attacks.Add(safetyOverrideAttack.AttackName, safetyOverrideAttack);
        
        // Monitoring Disruption Attack
        MonitoringDisruptionAttack monitoringDisruptionAttack = gameObject.AddComponent<MonitoringDisruptionAttack>();
        monitoringDisruptionAttack.powerStatusIndicator = powerStatusIconManager;
        attacks.Add(monitoringDisruptionAttack.AttackName, monitoringDisruptionAttack);
        
        // Power Generation Attack
        PowerGenerationAttack powerGenerationAttack = gameObject.AddComponent<PowerGenerationAttack>();
        attacks.Add(powerGenerationAttack.AttackName, powerGenerationAttack);
        
        // Substation Attack
        SubstationAttack substationAttack = gameObject.AddComponent<SubstationAttack>();
        attacks.Add(substationAttack.AttackName, substationAttack);
    }
    
    private void SetupButtons()
    {
        // Visual Disruption button
        if (visualDisruptionButton != null)
        {
            visualDisruptionButton.onClick.AddListener(() => ToggleAttack("VisualDisruption", visualDisruptionButton));
        }
        
        // Safety Override button
        if (safetyOverrideButton != null)
        {
            safetyOverrideButton.onClick.AddListener(() => ToggleAttack("SafetyOverride", safetyOverrideButton));
        }
        
        // Monitoring Disruption button
        if (monitoringDisruptionButton != null)
        {
            monitoringDisruptionButton.onClick.AddListener(() => ToggleAttack("MonitoringDisruption", monitoringDisruptionButton));
        }
        
        // Power Generation button
        if (powerGenerationButton != null)
        {
            powerGenerationButton.onClick.AddListener(() => ToggleAttack("PowerGeneration", powerGenerationButton));
        }
        
        // Substation Attack button (NEW)
        if (substationAttackButton != null)
        {
            substationAttackButton.onClick.AddListener(() => ToggleAttack("SubstationAttack", substationAttackButton));
        }
    }
    
    // Method to toggle an attack by name
    public void ToggleAttack(string attackName, Button associatedButton = null)
    {
        if (attacks.TryGetValue(attackName, out CyberAttackBase attack))
        {
            // Toggle the attack
            attack.ToggleAttack();
            
            // Update button state if provided
            if (associatedButton != null)
            {
                UpdateButtonState(associatedButton, attack.IsActive(), attackName);
            }
        }
        else
        {
            Debug.LogError($"Attack '{attackName}' not found!");
        }
    }
    
    // Method to check if a turbine is currently being manipulated
    public bool IsTurbineManipulated(Turbine turbine)
    {
        // Check with the power generation attack
        if (attacks.TryGetValue("PowerGeneration", out CyberAttackBase attack) && 
            attack is PowerGenerationAttack powerGenAttack && 
            powerGenAttack.IsActive())
        {
            return powerGenAttack.IsTurbineManipulated(turbine);
        }
        
        return false;
    }
    
    // Helper method to update button visual states - Updated for TMPro
    private void UpdateButtonState(Button button, bool active, string attackName)
    {
        if (button == null) return;
        
        // Check for both regular Text and TextMeshPro components
        TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
        Text regularText = button.GetComponentInChildren<Text>();
        
        if (active)
        {
            // Visual change for active state
            ColorBlock colors = button.colors;
            colors.normalColor = Color.red;
            button.colors = colors;
            
            // Handle specific text change for visualisation attack button
            if (attackName == "VisualDisruption")
            {
                if (tmpText != null)
                {
                    tmpText.text = "Disengage Attack";
                }
                else if (regularText != null)
                {
                    regularText.text = "Disengage Attack";
                }
            }
            else
            {
                // For other buttons, use generic replacement
                if (tmpText != null)
                {
                    tmpText.text = tmpText.text.Replace("Launch", "Stop");
                    tmpText.text = tmpText.text.Replace("Start", "Stop");
                }
                else if (regularText != null)
                {
                    regularText.text = regularText.text.Replace("Launch", "Stop");
                    regularText.text = regularText.text.Replace("Start", "Stop");
                }
            }
        }
        else
        {
            // Reset to default state
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            button.colors = colors;
            
            // Handle specific text change for visualisation attack button
            if (attackName == "VisualDisruption")
            {
                if (tmpText != null)
                {
                    tmpText.text = "Launch Visualisation Attack";
                }
                else if (regularText != null)
                {
                    regularText.text = "Launch Visualisation Attack";
                }
            }
            else
            {
                // For other buttons, use generic replacement
                if (tmpText != null)
                {
                    tmpText.text = tmpText.text.Replace("Stop", "Launch");
                    tmpText.text = tmpText.text.Replace("Stop", "Start");
                }
                else if (regularText != null)
                {
                    regularText.text = regularText.text.Replace("Stop", "Launch");
                    regularText.text = regularText.text.Replace("Stop", "Start");
                }
            }
        }
    }
    
    // Clean up on destroy for sanity check
    private void OnDestroy()
    {
        // Unbind button listeners
        if (visualDisruptionButton != null) visualDisruptionButton.onClick.RemoveAllListeners();
        if (safetyOverrideButton != null) safetyOverrideButton.onClick.RemoveAllListeners();
        if (monitoringDisruptionButton != null) monitoringDisruptionButton.onClick.RemoveAllListeners();
        if (powerGenerationButton != null) powerGenerationButton.onClick.RemoveAllListeners();
        if (substationAttackButton != null) substationAttackButton.onClick.RemoveAllListeners(); // New cleanup
    }
}