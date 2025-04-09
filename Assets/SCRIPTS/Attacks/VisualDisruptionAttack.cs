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
    
    // Reference to status indicator manager (will be set during initialization)
    private PowerStatusIndicatorManager statusIndicator;
    
    // Store original status icon sprites
    private Sprite originalWarningSprite;
    private Sprite originalCriticalSprite;
    private Sprite originalGoodSprite;
    
    // Method to set the status indicator reference from CyberAttackManager
    public void SetPowerStatusIndicator(PowerStatusIndicatorManager indicatorManager)
    {
        statusIndicator = indicatorManager;
    }
    
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
        
        // Store original sprites if status indicator is set
        if (statusIndicator != null)
        {
            originalWarningSprite = statusIndicator.warningSprite;
            originalCriticalSprite = statusIndicator.criticalSprite;
            originalGoodSprite = statusIndicator.goodSprite;
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
        
        // Restore original status icon sprites
        if (statusIndicator != null)
        {
            statusIndicator.warningSprite = originalWarningSprite;
            statusIndicator.criticalSprite = originalCriticalSprite;
            statusIndicator.goodSprite = originalGoodSprite;
            
            // Force update all consumers to refresh icons
            statusIndicator.ForceRefreshAllIcons();
        }
        
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
            
            // Randomly shuffle status icons
            if (statusIndicator != null)
            {
                ScrambleStatusIcons();
            }
            
            // Force update all existing connections
            ForceVisualiserUpdate();
            
            yield return new WaitForSeconds(visualDisruptionUpdateInterval);
        }
    }
    
    private void ScrambleStatusIcons()
    {
        // Create an array of the available sprites
        Sprite[] availableSprites = new Sprite[] 
        {
            originalWarningSprite,
            originalCriticalSprite,
            originalGoodSprite
        };
        
        // Randomly assign sprites to different statuses
        statusIndicator.warningSprite = availableSprites[Random.Range(0, availableSprites.Length)];
        statusIndicator.criticalSprite = availableSprites[Random.Range(0, availableSprites.Length)];
        statusIndicator.goodSprite = availableSprites[Random.Range(0, availableSprites.Length)];
        
        // Force refresh all icons
        statusIndicator.ForceRefreshAllIcons();
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