using System.Collections;
using UnityEngine;

public class SubstationAttack : CyberAttackBase
{
    private Transformer targetTransformer;
    private Battery backupBattery;
    private PowerGrid targetGrid;
    private bool originalOperationalState;
    private float batteryPowerOutput = 0f;
    private const float POWER_THRESHOLD = 0.01f;
    
    public override string AttackName => "SubstationAttack";
    
    public override void Initialise(PowerVisualiser visualiser, PowerSystemManager systemManager)
    {
        base.Initialise(visualiser, systemManager);
        
        if (systemManager?.transformer != null)
        {
            targetTransformer = systemManager.transformer;
            backupBattery = targetTransformer.outputBattery;
            targetGrid = targetTransformer.outputGrid;
        }
        else
        {
            Debug.LogError("SubstationAttack: Transformer not found");
        }
    }
    
    protected override void StartAttack()
    {
        if (targetTransformer == null)
        {
            isActive = false;
            return;
        }
        
        // Create battery connection before disabling transformer
        if (powerVisualiser != null && backupBattery != null && targetGrid != null)
        {
            float currentDemand = targetGrid.GetTotalDemand();
            batteryPowerOutput = currentDemand > POWER_THRESHOLD ? currentDemand : 0f;
            
            powerVisualiser.CreateOrUpdateConnection(
                backupBattery.gameObject,
                targetGrid.gameObject,
                batteryPowerOutput
            );
        }
        
        // Disable transformer
        originalOperationalState = targetTransformer.isOperational;
        targetTransformer.isOperational = false;
        targetTransformer.visualisationEnabled = false;
        
        // Transfer power from battery to grid
        if (backupBattery != null && targetGrid != null && batteryPowerOutput > 0)
        {
            backupBattery.RequestPower(batteryPowerOutput);
            targetGrid.ReceivePower(backupBattery, batteryPowerOutput);
        }
        
        StartCoroutine(AttackRoutine());
    }
    
    protected override void StopAttack()
    {
        StopAllCoroutines();
        
        // Check for destroyed objects before accessing
        if (targetTransformer != null)
        {
            targetTransformer.isOperational = originalOperationalState;
            targetTransformer.visualisationEnabled = true;
        }
        
        // Clear battery connection safely
        if (powerVisualiser != null && backupBattery != null && targetGrid != null)
        {
            // Check if objects are still valid before creating connections
            if (backupBattery != null && !ReferenceEquals(backupBattery, null) && 
                targetGrid != null && !ReferenceEquals(targetGrid, null))
            {
                if (backupBattery.gameObject != null && targetGrid.gameObject != null)
                {
                    powerVisualiser.CreateOrUpdateConnection(
                        backupBattery.gameObject,
                        targetGrid.gameObject,
                        0f
                    );
                }
            }
        }
    }
    
    private IEnumerator AttackRoutine()
    {
        while (isActive)
        {
            // Safely check for destroyed objects
            if (backupBattery == null || targetGrid == null || powerVisualiser == null)
            {
                isActive = false;
                yield break;
            }
            
            // Update power flow and visualisation
            float gridDemand = targetGrid.GetTotalDemand();
            batteryPowerOutput = (gridDemand > POWER_THRESHOLD && backupBattery.GetChargePercentage() > 0) 
                ? gridDemand : 0f;
            
            // Update power transfer
            if (batteryPowerOutput > 0)
            {
                backupBattery.RequestPower(batteryPowerOutput);
                targetGrid.ReceivePower(backupBattery, batteryPowerOutput);
            }
            
            // Update visualisation if objects still exist
            if (backupBattery.gameObject != null && targetGrid.gameObject != null)
            {
                powerVisualiser.CreateOrUpdateConnection(
                    backupBattery.gameObject,
                    targetGrid.gameObject,
                    batteryPowerOutput
                );
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
}