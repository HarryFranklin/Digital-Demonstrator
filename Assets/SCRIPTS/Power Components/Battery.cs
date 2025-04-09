using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Battery class
public class Battery : PowerComponentBase
{
    // I/O
    public PowerGrid outputGrid;
    
    public float maxCapacity;
    public float currentCharge = 0f;
    public float chargeEfficiency = 0.9f; // Energy stored vs energy received
    public float dischargeEfficiency = 0.95f; // Energy output vs energy used from storage
    
    private float dischargeAmount = 0f;
    
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(UpdateVisualisationRoutine(0.1f));
    }
    
    void Update()
    {
        if (!isOperational)
        {
            currentPower = 0f;
            return;
        }
        
        // Send power to grid if requested
        if (dischargeAmount > 0 && currentCharge > 0)
        {
            // Only discharge what's needed to meet the demand
            float actualDischarge = Mathf.Min(dischargeAmount, currentCharge);
            
            // Apply efficiency to the output (how much actually reaches the grid)
            currentPower = actualDischarge * dischargeEfficiency;
            
            // Deduct the full amount from the battery
            currentCharge -= actualDischarge;
            
            if (outputGrid != null)
            {
                outputGrid.ReceivePower(this, currentPower);
            }
        }
        else
        {
            currentPower = 0f;
        }
        
        // Reset discharge amount for next frame
        dischargeAmount = 0f;
    }
    
    // Store excess power - take whatever is available
    public void StorePower(float excessPower)
    {
        if (!isOperational) return;
        
        float spaceAvailable = maxCapacity - currentCharge;
        
        // Store as much as possible with efficiency loss
        float actualCharge = Mathf.Min(excessPower * chargeEfficiency, spaceAvailable);
        currentCharge += actualCharge;
        
        // If battery is full and still receiving power, log message
        if (currentCharge >= maxCapacity * 0.99f && excessPower > 0)
        {
            Debug.Log("Battery is full! Consider reducing turbine output to prevent power overflow.");
        }
    }
    
    // Request exactly the needed power to meet demand
    public void RequestPower(float neededAmount)
    {
        if (!isOperational) return;
        
        // Account for efficiency - we need to withdraw more than what's needed due to efficiency loss
        dischargeAmount = neededAmount / dischargeEfficiency;
    }
    
    public float GetChargePercentage()
    {
        return (currentCharge / maxCapacity) * 100f;
    }
    
    public override void VisualiseConnections()
    {
        if (visualiser != null && outputGrid != null)
        {
            visualiser.CreateOrUpdateConnection(gameObject, outputGrid.gameObject, currentPower);
        }
    }
}