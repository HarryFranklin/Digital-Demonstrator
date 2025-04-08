using UnityEngine;

public class MonitoringDisruptionAttack : CyberAttackBase
{
    public override string AttackName => "MonitoringDisruption";
    public PowerStatusIndicatorManager powerStatusIndicator;
    
    private void Start()
    {
        
    }
    
    protected override void StartAttack()
    {
        if (powerSystemManager == null || powerVisualiser == null) return;
        
        Debug.Log("Starting monitoring disruption attack - disabling system monitoring and visualisation");
        // Disable the monitoring in PowerSystemManager
        powerSystemManager.debugMode = false;
        
        // Disable visualisation by hiding all connection lines
        HideAllConnectionLines();
        
        // Disable the visualisation update coroutines in all components
        ToggleComponentVisualisation(false);
        
        // Disable the power status indicator
        if (powerStatusIndicator != null)
        {
            powerStatusIndicator.gameObject.SetActive(false);
            Debug.Log("Disabled PowerStatusIndicatorManager as part of monitoring disruption attack");
        }
    }
    
    protected override void StopAttack()
    {
        if (powerSystemManager == null || powerVisualiser == null) return;
        
        Debug.Log("Stopping monitoring disruption attack - restoring system monitoring and visualisation");
        // Re-enable monitoring
        powerSystemManager.debugMode = true;
        
        // Show connection lines again
        ShowAllConnectionLines();
        
        // Re-enable visualisation update coroutines in all components
        ToggleComponentVisualisation(true);
        
        // Re-enable the power status indicator
        if (powerStatusIndicator != null)
        {
            powerStatusIndicator.gameObject.SetActive(true);
            // Restart the monitoring coroutine when re-activating
            powerStatusIndicator.RestartMonitoring();
            Debug.Log("Re-enabled PowerStatusIndicatorManager after stopping monitoring disruption attack");
        }
    }
    
    // Helper method to hide all connection lines
    private void HideAllConnectionLines()
    {
        if (powerVisualiser != null)
        {
            powerVisualiser.HideAllConnections();
        }
    }

    // Helper method to show all connection lines
    private void ShowAllConnectionLines()
    {
        if (powerVisualiser != null)
        {
            powerVisualiser.ShowAllConnections();
            
            // Force an update to restore all connections
            powerVisualiser.ForceUpdateAllConnections();
        }
    }

    // Method to toggle visualisation in all power components
    private void ToggleComponentVisualisation(bool enabled)
    {
        // Toggle visualisation for turbines
        foreach (var turbine in powerSystemManager.turbines)
        {
            if (turbine != null)
            {
                turbine.visualisationEnabled = enabled;
            }
        }
        
        // Toggle visualisation for wind farm
        if (powerSystemManager.windFarm != null)
        {
            powerSystemManager.windFarm.visualisationEnabled = enabled;
        }
        
        // Toggle visualisation for inverter
        if (powerSystemManager.inverter != null)
        {
            powerSystemManager.inverter.visualisationEnabled = enabled;
        }
        
        // Toggle visualisation for transformer
        if (powerSystemManager.transformer != null)
        {
            powerSystemManager.transformer.visualisationEnabled = enabled;
        }
        
        // Toggle visualisation for battery
        if (powerSystemManager.battery != null)
        {
            powerSystemManager.battery.visualisationEnabled = enabled;
        }
        
        // Toggle visualisation for power grid
        if (powerSystemManager.powerGrid != null)
        {
            powerSystemManager.powerGrid.visualisationEnabled = enabled;
        }
        
        // Toggle visualisation for consumers
        foreach (var consumer in powerSystemManager.consumers)
        {
            if (consumer != null)
            {
                consumer.visualisationEnabled = enabled;
            }
        }
    }
}