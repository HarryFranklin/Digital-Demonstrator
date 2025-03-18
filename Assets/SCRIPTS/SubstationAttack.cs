using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubstationAttack : CyberAttackBase
{
    // Transformer reference to manipulate
    private Transformer targetTransformer;
    
    // Variable to track the original operational state
    private bool originalOperationalState;
    
    // Override for attack name
    public override string AttackName => "SubstationAttack";
    
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
            
            // Force an immediate visualisation update
            targetTransformer.VisualiseConnections();
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
            
            // Force an immediate visualisation update
            targetTransformer.VisualiseConnections();
        }
    }
}