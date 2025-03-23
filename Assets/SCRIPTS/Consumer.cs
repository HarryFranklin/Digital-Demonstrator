using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumer : PowerComponentBase
{
    public PowerGrid inputGrid;
    public float powerDemand = 5f;
    private float receivedPower = 0f;
    private bool isPowerShortage = false;
    
    [HideInInspector] // Hide this in the but keep it accessible via code
    public float currentPower { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(UpdateVisualisationRoutine(0.1f));
        
        // Register with grid
        if (inputGrid != null)
        {
            inputGrid.RegisterConsumer(this);
        }
    }
    
    void OnDestroy()
    {
        // Unregister from grid
        if (inputGrid != null)
        {
            inputGrid.UnregisterConsumer(this);
        }
    }
    
    void Update()
    {
        if (!isOperational)
        {
            currentPower = 0f;
            return;
        }
        
        // Check if we have enough power
        currentPower = receivedPower;
        isPowerShortage = currentPower < powerDemand * 0.99f;
        
        if (isPowerShortage)
        {
            Debug.Log($"Consumer {gameObject.name} is not receiving enough power. Required: {powerDemand}, Received: {currentPower}");
        }
        
        // Reset received power for next frame
        receivedPower = 0f;
    }
    
    // Receive power from grid
    public void ReceivePower(float power)
    {
        receivedPower = power;
    }
    
    // Get power demand
    public float GetPowerDemand()
    {
        return isOperational ? powerDemand : 0f;
    }
    
    public override void VisualiseConnections()
    {
        // Consumer is an endpoint, so no outgoing connections to visualise
    }
}