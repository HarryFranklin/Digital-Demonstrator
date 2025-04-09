using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : PowerComponentBase
{
    // I/O
    public Transformer outputTransformer;
    
    private float inputPower = 0f;
    
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(UpdateVisualisationRoutine(0.1f));
    }
    
    void Update()
    {
        if (isOperational)
        {
            currentPower = inputPower; // Pass through all power
            
            // Send power to transformer
            if (outputTransformer != null)
            {
                outputTransformer.ReceivePower(currentPower);
            }
        }
        else
        {
            currentPower = 0f;
        }
    }
    
    // Receive power from wind farm
    public void ReceivePower(float power)
    {
        inputPower = power;
    }
    
    public override void VisualiseConnections()
    {
        if (visualiser != null && outputTransformer != null)
        {
            visualiser.CreateOrUpdateConnection(gameObject, outputTransformer.gameObject, currentPower);
        }
    }
}