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

        // Save state
        originalOperationalState = targetTransformer.isOperational;

        // Disable transformer
        targetTransformer.isOperational = false;
        targetTransformer.visualisationEnabled = false;

        // Set transformer-to-grid connection to red (no power)
        if (powerVisualiser != null && targetTransformer.gameObject != null && targetGrid != null)
        {
            powerVisualiser.CreateOrUpdateConnection(
                targetTransformer.gameObject,
                targetGrid.gameObject,
                0f // No power flowing
            );
        }

        // Provide battery backup power if available
        if (backupBattery != null && targetGrid != null)
        {
            float currentDemand = targetGrid.GetTotalDemand();
            batteryPowerOutput = currentDemand > POWER_THRESHOLD ? currentDemand : 0f;

            if (batteryPowerOutput > 0f)
            {
                backupBattery.RequestPower(batteryPowerOutput);
                targetGrid.ReceivePower(backupBattery, batteryPowerOutput);
            }

            if (powerVisualiser != null)
            {
                powerVisualiser.CreateOrUpdateConnection(
                    backupBattery.gameObject,
                    targetGrid.gameObject,
                    batteryPowerOutput
                );
            }
        }

        StartCoroutine(AttackRoutine());
    }

    protected override void StopAttack()
    {
        StopAllCoroutines();

        // Restore transformer state
        if (targetTransformer != null)
        {
            targetTransformer.isOperational = originalOperationalState;
            targetTransformer.visualisationEnabled = true;
        }

        // Clear transformer and battery connections
        if (powerVisualiser != null)
        {
            if (targetTransformer != null && targetGrid != null)
            {
                powerVisualiser.CreateOrUpdateConnection(
                    targetTransformer.gameObject,
                    targetGrid.gameObject,
                    targetTransformer.isOperational ? targetTransformer.GetCurrentPower() : 0f
                );
            }

            if (backupBattery != null && targetGrid != null)
            {
                powerVisualiser.CreateOrUpdateConnection(
                    backupBattery.gameObject,
                    targetGrid.gameObject,
                    0f
                );
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (isActive)
        {
            if (backupBattery == null || targetGrid == null || powerVisualiser == null)
            {
                isActive = false;
                yield break;
            }

            float gridDemand = targetGrid.GetTotalDemand();
            batteryPowerOutput = (gridDemand > POWER_THRESHOLD && backupBattery.GetChargePercentage() > 0)
                ? gridDemand : 0f;

            if (batteryPowerOutput > 0f)
            {
                backupBattery.RequestPower(batteryPowerOutput);
                targetGrid.ReceivePower(backupBattery, batteryPowerOutput);
            }

            // Keep transformer output visual red
            if (targetTransformer != null && targetGrid != null)
            {
                powerVisualiser.CreateOrUpdateConnection(
                    targetTransformer.gameObject,
                    targetGrid.gameObject,
                    0f
                );
            }

            if (backupBattery != null && targetGrid != null)
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
