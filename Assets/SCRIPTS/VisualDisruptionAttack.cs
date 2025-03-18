using System.Collections;
using UnityEngine;

public class VisualDisruptionAttack : CyberAttackBase
{
    public override string AttackName => "VisualDisruption";
    
    public float visualDisruptionUpdateInterval = 0.5f; // How often to change colours
    
    // Colours for the visual disruption attack - limited to red, green, yellow
    private Color[] disruptionColours;
    private Coroutine visualDisruptionCoroutine;
    
    // Store original visualiser colours to restore them
    private Color originalPowerFlowingColor;
    private Color originalNoPowerColor;
    private Color originalInsufficientPowerColor;
    
    public override void Initialise(PowerVisualiser visualiser, PowerSystemManager systemManager)
    {
        base.Initialise(visualiser, systemManager);
        
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
    }
    
    protected override void StartAttack()
    {
        if (powerVisualiser == null) return;
        
        Debug.Log("Starting visual disruption attack");
        visualDisruptionCoroutine = StartCoroutine(VisualDisruptionAttackRoutine());
    }
    
    protected override void StopAttack()
    {
        if (powerVisualiser == null) return;
        
        Debug.Log("Stopping visual disruption attack");
        if (visualDisruptionCoroutine != null)
        {
            StopCoroutine(visualDisruptionCoroutine);
        }
        
        // Restore original colours
        powerVisualiser.powerFlowingColor = originalPowerFlowingColor;
        powerVisualiser.noPowerColor = originalNoPowerColor;
        powerVisualiser.insufficientPowerColor = originalInsufficientPowerColor;
        
        // Call method to force update lines
        ForceVisualiserUpdate();
    }
    
    private IEnumerator VisualDisruptionAttackRoutine()
    {
        while (isActive)
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
}