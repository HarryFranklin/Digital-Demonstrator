using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubstationAttack : CyberAttackBase
{
    // Transformer reference to manipulate
    private Transformer targetTransformer;
    
    // Variable to track the original operational state
    private bool originalOperationalState;
    
    // Flag to track attack status for visualization
    private bool isUnderAttack = false;
    
    // Override for attack name
    public override string AttackName => "SubstationAttack";
    
    // Reference to the original VisualiseConnections method
    private System.Delegate originalVisualiseMethod;
    
    public override void Initialise(PowerVisualiser visualiser, PowerSystemManager systemManager)
    {
        base.Initialise(visualiser, systemManager);
        
        // Find the transformer in the system
        if (systemManager != null && systemManager.transformer != null)
        {
            targetTransformer = systemManager.transformer;
        }
        else
        {
            Debug.LogError("SubstationAttack: Transformer not found in system manager");
        }
    }
    
    protected override void StartAttack()
    {
        Debug.Log("Starting Substation Attack - Disabling transformer");
        
        if (targetTransformer != null)
        {
            // Store the original state before disabling
            originalOperationalState = targetTransformer.isOperational;
            
            // Disable the transformer
            targetTransformer.isOperational = false;
            
            // Set attack flag to true
            isUnderAttack = true;
            
            // Override the transformer's visualization method
            targetTransformer.visualisationEnabled = false;
            
            // Apply our own visualization
            StartCoroutine(AttackVisualisationRoutine());
        }
        else
        {
            Debug.LogError("SubstationAttack: No transformer to attack");
            isActive = false;
        }
    }
    
    protected override void StopAttack()
    {
        Debug.Log("Stopping Substation Attack - Restoring transformer operation");
        
        if (targetTransformer != null)
        {
            // Restore the original operational state
            targetTransformer.isOperational = originalOperationalState;
            
            // Reset attack flag
            isUnderAttack = false;
            
            // Re-enable transformer's original visualization
            targetTransformer.visualisationEnabled = true;
        }
        
        // Stop our visualization coroutine
        StopAllCoroutines();
    }
    
    // Coroutine to continuously update visualizations during attack
    private IEnumerator AttackVisualisationRoutine()
    {
        while (isUnderAttack)
        {
            if (powerVisualiser != null && targetTransformer != null)
            {
                GameObject transformerObj = targetTransformer.gameObject;
                
                // Update connection to grid
                if (targetTransformer.outputGrid != null)
                {
                    powerVisualiser.CreateOrUpdateConnection(
                        transformerObj, 
                        targetTransformer.outputGrid.gameObject, 
                        0 // Force zero power during attack
                    );
                }
                
                // Update connection to battery
                if (targetTransformer.outputBattery != null)
                {
                    powerVisualiser.CreateOrUpdateConnection(
                        transformerObj, 
                        targetTransformer.outputBattery.gameObject, 
                        0 // Force zero power during attack
                    );
                }
            }
            
            yield return new WaitForSeconds(0.05f); // Update slightly faster than transformer's routine
        }
    }
}