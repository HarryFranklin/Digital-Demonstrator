using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumer : PowerComponentBase
{
    public PowerGrid inputGrid;
    public float powerDemand = 5f;
    private float receivedPower = 0f;
    private bool isPowerShortage = false;
    
    [HideInInspector] // Hide this in the inspector but keep it accessible via code
    public float currentPower { get; private set; }
    
    // Event that can be used to notify when power status changes
    public delegate void PowerStatusChanged(float ratio);
    public event PowerStatusChanged OnPowerStatusChanged;
    
    private float lastPowerRatio = 1.0f; // Track last power ratio to detect changes
    
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
            SetCurrentPower(0f);
            return;
        }
        
        // Set current power based on received power
        SetCurrentPower(receivedPower);
    }
    
    // Set current power and check for status changes
    private void SetCurrentPower(float power)
    {
        currentPower = power;
        
        // Calculate power ratio
        float powerRatio = (powerDemand > 0) ? currentPower / powerDemand : 1.0f;
        isPowerShortage = powerRatio < 0.99f;
        
        // Notify if power status changed significantly
        if (Mathf.Abs(powerRatio - lastPowerRatio) > 0.01f)
        {
            if (isPowerShortage)
            {
                Debug.Log($"Consumer {gameObject.name} power status changed: {powerRatio:P0} of needed power. Required: {powerDemand}, Received: {currentPower}");
            }
            
            // Fire event for status change
            OnPowerStatusChanged?.Invoke(powerRatio);
            lastPowerRatio = powerRatio;
        }
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
    
    // Override to return current power
    public override float GetCurrentPower()
    {
        return currentPower;
    }
    
    public override void VisualiseConnections()
    {
        // Consumer is an endpoint, so no outgoing connections to visualise
    }
}