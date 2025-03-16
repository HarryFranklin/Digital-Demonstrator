using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CyberAttack : MonoBehaviour
{
    [Header("Management System and Visualiser")]
    public PowerVisualiser powerVisualiser;
    public PowerSystemManager powerSystemManager;
    
    [Header("Attack Controls")]
    public Button visualDisruptionButton;
    public Button safetyOverrideButton;
    public Button monitoringDisruptionButton;
    public Button powerGenerationButton;
    
    [Header("Visual Disruption Attack")]
    public bool visualDisruptionActive = false;
    public float visualDisruptionUpdateInterval = 0.5f; // How often to change colours
    
    // Colours for the visual disruption attack - limited to red, green, yellow
    private Color[] disruptionColours;
    private Coroutine visualDisruptionCoroutine;
    
    // Store original visualiser colours to restore them
    private Color originalPowerFlowingColor;
    private Color originalNoPowerColor;
    private Color originalInsufficientPowerColor;
    
    // Track if we're currently executing an attack
    private Dictionary<string, bool> activeAttacks = new Dictionary<string, bool>();

    // Track which turbines are currently being manipulated during power generation attack
    private HashSet<Turbine> manipulatedTurbines = new HashSet<Turbine>();
    
    private void Start()
    {
        // Initialise attack colours - limited to red, green, yellow
        disruptionColours = new Color[]
        {
            Color.red,
            Color.green,
            Color.yellow
        };
        
        // Store original colours
        if (powerVisualiser != null)
        {
            originalPowerFlowingColor = powerVisualiser.powerFlowingColor;
            originalNoPowerColor = powerVisualiser.noPowerColor;
            originalInsufficientPowerColor = powerVisualiser.insufficientPowerColor;
        }
        
        // Initialise active attacks dictionary
        activeAttacks["VisualDisruption"] = false;
        activeAttacks["SafetyOverride"] = false;
        activeAttacks["MonitoringDisruption"] = false;
        activeAttacks["PowerGeneration"] = false;
        
        // Set up button listeners
        SetupButtons();
    }
    
    private void SetupButtons()
    {
        // Visual Disruption button
        if (visualDisruptionButton != null)
        {
            visualDisruptionButton.onClick.AddListener(ToggleVisualDisruptionAttack);
        }
        
        // Safety Override button
        if (safetyOverrideButton != null)
        {
            safetyOverrideButton.onClick.AddListener(ToggleSafetyOverrideAttack);
        }
        
        // Monitoring Disruption button
        if (monitoringDisruptionButton != null)
        {
            monitoringDisruptionButton.onClick.AddListener(ToggleMonitoringDisruptionAttack);
        }
        
        // Power Generation button
        if (powerGenerationButton != null)
        {
            powerGenerationButton.onClick.AddListener(TogglePowerGenerationAttack);
        }
    }
    
    // Method to toggle visual disruption attack
    public void ToggleVisualDisruptionAttack()
    {
        if (powerVisualiser == null) return;
        
        visualDisruptionActive = !visualDisruptionActive;
        
        if (visualDisruptionActive)
        {
            // Start the attack
            Debug.Log("Starting visual disruption attack");
            visualDisruptionCoroutine = StartCoroutine(VisualDisruptionAttack());
            activeAttacks["VisualDisruption"] = true;
            
            // Update button text/colour
            UpdateButtonState(visualDisruptionButton, true);
        }
        else
        {
            // Stop the attack and restore original settings
            Debug.Log("Stopping visual disruption attack");
            if (visualDisruptionCoroutine != null)
            {
                StopCoroutine(visualDisruptionCoroutine);
            }
            
            // Restore original colours
            powerVisualiser.powerFlowingColor = originalPowerFlowingColor;
            powerVisualiser.noPowerColor = originalNoPowerColor;
            powerVisualiser.insufficientPowerColor = originalInsufficientPowerColor;
            
            activeAttacks["VisualDisruption"] = false;
            
            // Call method to force update lines
            ForceVisualiserUpdate();
            
            // Update button text/colour
            UpdateButtonState(visualDisruptionButton, false);
        }
    }
    
    private IEnumerator VisualDisruptionAttack()
    {
        while (visualDisruptionActive)
        {
            // Randomly shuffle visualiser colours
            powerVisualiser.powerFlowingColor = GetRandomColor();
            powerVisualiser.noPowerColor = GetRandomColor();
            powerVisualiser.insufficientPowerColor = GetRandomColor();
            
            // Force update all existing connections
            ForceVisualiserUpdate();
            
            yield return new WaitForSeconds(visualDisruptionUpdateInterval);
        }
    }
    
    private Color GetRandomColor()
    {
        return disruptionColours[Random.Range(0, disruptionColours.Length)];
    }
    
    private void ForceVisualiserUpdate()
    {
        // Call the ForceUpdateAllConnections method in PowerVisualiser
        if (powerVisualiser != null)
        {
            powerVisualiser.ForceUpdateAllConnections();
        }
    }
    
    // Safety Override Attack - Turn off safety mechanisms for overcharging
    public void ToggleSafetyOverrideAttack()
    {
        if (powerSystemManager == null) return;
        
        bool isActive = activeAttacks["SafetyOverride"];
        activeAttacks["SafetyOverride"] = !isActive;
        
        if (activeAttacks["SafetyOverride"])
        {
            Debug.Log("Starting safety override attack - disabling battery safety mechanisms");
            // Implementation would depend on your battery/safety code
            // Example: if (powerSystemManager.battery != null) powerSystemManager.battery.DisableSafetyMechanisms();
            
            UpdateButtonState(safetyOverrideButton, true);
        }
        else
        {
            Debug.Log("Stopping safety override attack - restoring safety mechanisms");
            // Example: if (powerSystemManager.battery != null) powerSystemManager.battery.EnableSafetyMechanisms();
            
            UpdateButtonState(safetyOverrideButton, false);
        }
    }
    
    // Monitoring Disruption Attack - Turn off monitoring
    public void ToggleMonitoringDisruptionAttack()
    {
        if (powerSystemManager == null) return;
        
        bool isActive = activeAttacks["MonitoringDisruption"];
        activeAttacks["MonitoringDisruption"] = !isActive;
        
        if (activeAttacks["MonitoringDisruption"])
        {
            Debug.Log("Starting monitoring disruption attack - disabling system monitoring");
            // Disable the monitoring in PowerSystemManager
            // powerSystemManager.debugMode = false;
            
            UpdateButtonState(monitoringDisruptionButton, true);
        }
        else
        {
            Debug.Log("Stopping monitoring disruption attack - restoring system monitoring");
            // Re-enable monitoring
            powerSystemManager.debugMode = true;
            
            UpdateButtonState(monitoringDisruptionButton, false);
        }
    }
    
    // Power Generation Attack - Impact on power generation
    public void TogglePowerGenerationAttack()
    {
        if (powerSystemManager == null) return;
        
        bool isActive = activeAttacks["PowerGeneration"];
        activeAttacks["PowerGeneration"] = !isActive;
        
        if (activeAttacks["PowerGeneration"])
        {
            Debug.Log("Starting power generation attack - manipulating turbine outputs");
            
            // Start random turbine manipulation
            StartCoroutine(RandomTurbineManipulation());
            
            // UpdateButtonState(powerGenerationButton, true);
        }
        else
        {
            Debug.Log("Stopping power generation attack - restoring normal operations");
            // Stop related coroutines
            StopCoroutine(RandomTurbineManipulation());
            
            // Reset turbines to automatic control
            powerSystemManager.ResetTurbinesToAutomatic();

            // Clear the set of manipulated turbines
            manipulatedTurbines.Clear();

            // Update the visualiser to change the colour of turbine-wind farm lines back to normal
            ForceVisualiserUpdate();
            
            // UpdateButtonState(powerGenerationButton, false);
        }

        UpdateButtonState(powerGenerationButton, activeAttacks["PowerGeneration"]);
    }
    
    private IEnumerator RandomTurbineManipulation()
    {
        while (activeAttacks["PowerGeneration"])
        {
            // Clear the previous set of manipulated turbines
            manipulatedTurbines.Clear();
            
            foreach (var turbine in powerSystemManager.turbines)
            {
                if (turbine != null && Random.value > 0.7f) // 30% chance to manipulate each turbine
                {
                    float randomSpeed = Random.Range(0f, turbine.maxSpeed * 1.5f); // Potentially exceed safe limits
                    if (Random.value > 0.8f) { randomSpeed = 0f; } // 20% chance that, ignore the above and turn the turbine
                    turbine.ToggleManualControl(true, randomSpeed); // Set manual control to true in order to parse a "false" speed
                
                    // Add this turbine to the set of currently manipulated turbines.
                    manipulatedTurbines.Add(turbine);
                }
            }

            // Force visualiser to show yellow connections to indicate a turbine has been manipulated.
            ForceVisualiserUpdate();

            yield return new WaitForSeconds(3f); // Re-do every 3 seconds
        }
    }
    
    // Helper method to update button visual states - Updated for TMPro
    private void UpdateButtonState(Button button, bool active)
    {
        if (button == null) return;
        
        // Check for both regular Text and TextMeshPro components
        TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
        Text regularText = button.GetComponentInChildren<Text>();
        
        if (active)
        {
            // Visual change for active state
            // ColorBlock colors = button.colors;
            // colors.normalColor = Color.red;
            // button.colors = colors;
            
            // Handle specific text change for visualisation attack button
            if (button == visualDisruptionButton)
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
            if (button == visualDisruptionButton)
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

    // Method to check if a turbine is currently being manipulated
    public bool IsTurbineManipulated(Turbine turbine)
    {
        return manipulatedTurbines.Contains(turbine);
    }
    
    // Clean up on destroy for sanity check
    private void OnDestroy()
    {
        // Unbind button listeners
        if (visualDisruptionButton != null) visualDisruptionButton.onClick.RemoveListener(ToggleVisualDisruptionAttack);
        if (safetyOverrideButton != null) safetyOverrideButton.onClick.RemoveListener(ToggleSafetyOverrideAttack);
        if (monitoringDisruptionButton != null) monitoringDisruptionButton.onClick.RemoveListener(ToggleMonitoringDisruptionAttack);
        if (powerGenerationButton != null) powerGenerationButton.onClick.RemoveListener(TogglePowerGenerationAttack);
    }
}