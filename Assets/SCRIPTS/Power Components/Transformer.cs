using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformer : PowerComponentBase
{
    // I/O
    public PowerGrid outputGrid;
    public Battery outputBattery;
    
    private float inputPower = 0f;
    private float currentDemand = 0f;
    
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
        
        // Get current power demand from grid
        if (outputGrid != null)
        {
            currentDemand = outputGrid.GetTotalDemand();
        }
        
        currentPower = inputPower;
        
        // Route power based on demand
        if (inputPower <= currentDemand)
        {
            // If we don't have enough power to meet demand, send all available power to grid
            if (outputGrid != null)
            {
                outputGrid.ReceivePower(this, inputPower);
            }
            
            // Request exactly the additional power needed from battery to meet demand
            if (outputBattery != null && inputPower < currentDemand)
            {
                float neededPower = currentDemand - inputPower;
                outputBattery.RequestPower(neededPower);
            }
        }
        else
        {
            // If we have excess power, send exactly what's needed to grid
            if (outputGrid != null)
            {
                outputGrid.ReceivePower(this, currentDemand);
            }
            
            // Send all excess power to battery
            if (outputBattery != null)
            {
                float excessPower = inputPower - currentDemand;
                outputBattery.StorePower(excessPower);
            }
        }
    }
    
    // Receive power from inverter
    public void ReceivePower(float power)
    {
        inputPower = power;
    }
    
    public override void VisualiseConnections()
    {
        if (visualiser == null) return;
        
        // Visualise connection to grid
        if (outputGrid != null)
        {
            float powerToGrid = Mathf.Min(inputPower, currentDemand);
            visualiser.CreateOrUpdateConnection(gameObject, outputGrid.gameObject, powerToGrid);
        }
        
        // Visualise connection to battery
        if (outputBattery != null)
        {
            float powerToBattery = inputPower > currentDemand ? inputPower - currentDemand : 0;
            visualiser.CreateOrUpdateConnection(gameObject, outputBattery.gameObject, powerToBattery);
        }
    }
}