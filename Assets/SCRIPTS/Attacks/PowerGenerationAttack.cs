using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerGenerationAttack : CyberAttackBase
{
    public override string AttackName => "PowerGeneration";
    
    // Track which turbines are currently being manipulated during power generation attack
    private HashSet<Turbine> manipulatedTurbines = new HashSet<Turbine>();
    private Coroutine manipulationCoroutine;
    
    protected override void StartAttack()
    {
        if (powerSystemManager == null) return;
        
        Debug.Log("Starting power generation attack - manipulating turbine outputs");
        
        // Start random turbine manipulation
        manipulationCoroutine = StartCoroutine(RandomTurbineManipulation());
    }
    
    protected override void StopAttack()
    {
        if (powerSystemManager == null) return;
        
        Debug.Log("Stopping power generation attack - restoring normal operations");
        // Stop related coroutines
        if (manipulationCoroutine != null)
        {
            StopCoroutine(manipulationCoroutine);
        }
        
        // Reset turbines to automatic control
        powerSystemManager.ResetTurbinesToAutomatic();

        // Clear the set of manipulated turbines
        manipulatedTurbines.Clear();

        // Update the visualiser to change the colour of turbine-wind farm lines back to normal
        if (powerVisualiser != null)
        {
            powerVisualiser.ForceUpdateAllConnections();
        }
    }
    
    private IEnumerator RandomTurbineManipulation()
    {
        // Keep track of previously manipulated turbines
        HashSet<Turbine> previouslyManipulatedTurbines = new HashSet<Turbine>();
        
        while (isActive)
        {
            // Store previously manipulated turbines before clearing
            previouslyManipulatedTurbines = new HashSet<Turbine>(manipulatedTurbines);
            
            // Clear the current set of manipulated turbines
            manipulatedTurbines.Clear();
            
            foreach (var turbine in powerSystemManager.turbines)
            {
                if (turbine != null && Random.value > 0.7f) // 30% chance to manipulate each turbine
                {
                    float randomSpeed = Random.Range(0f, turbine.maxSpeed * 1.5f); // Potentially exceed safe limits
                    if (Random.value > 0.8f) { randomSpeed = 0f; } // 20% chance to turn off the turbine
                    turbine.ToggleManualControl(true, randomSpeed);
                    
                    // Add this turbine to the set of currently manipulated turbines
                    manipulatedTurbines.Add(turbine);
                }
            }

            // Instead of forcing update on all connections, only update the ones for turbines
            // that have changed state (either newly manipulated or no longer manipulated)
            UpdateChangedTurbineConnections(previouslyManipulatedTurbines);

            yield return new WaitForSeconds(3f); // Re-do every 3 seconds
        }
    }

    // Method to only update connections for turbines that changed state
    private void UpdateChangedTurbineConnections(HashSet<Turbine> previouslyManipulated)
    {
        if (powerVisualiser == null) return;
        
        // Find turbines that were manipulated before but aren't now
        foreach (var turbine in previouslyManipulated)
        {
            if (turbine != null && !manipulatedTurbines.Contains(turbine))
            {
                // This turbine is no longer manipulated, update its connection
                turbine.VisualiseConnections();
            }
        }
        
        // Find turbines that are now manipulated but weren't before
        foreach (var turbine in manipulatedTurbines)
        {
            if (turbine != null && !previouslyManipulated.Contains(turbine))
            {
                // This is a newly manipulated turbine, update its connection
                turbine.VisualiseConnections();
            }
        }
    }
    
    // Public method to check if a turbine is currently being manipulated
    public bool IsTurbineManipulated(Turbine turbine)
    {
        return manipulatedTurbines.Contains(turbine);
    }
}