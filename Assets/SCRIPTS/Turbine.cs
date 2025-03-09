using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbine : PowerComponentBase
{
    // I/O
    public WindFarm outputWindFarm;
    
    public float windSpeed;
    public float maxSpeed = 20f;
    
    // Rotation functionality
    public GameObject rotationPoint;
    public GameObject pivot;
    
    // For manual control
    public bool manualControl = false;
    public float manualWindSpeed = 0f;
    
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(UpdateVisualisationRoutine(0.1f));
    }
    
    void Update()
    {
        // Automatically toggle operational status based on wind speed
        float effectiveWindSpeed = manualControl ? manualWindSpeed : windSpeed;
        isOperational = effectiveWindSpeed > 0;
        
        if (isOperational)
        {
            RotateBlades(effectiveWindSpeed);
            currentPower = effectiveWindSpeed; // Directly use wind speed as power output
            
            // Send power to wind farm
            if (outputWindFarm != null)
            {
                outputWindFarm.ReceivePower(this, currentPower);
            }
        }
        else
        {
            currentPower = 0f; // No power if turbine is offline
            RotateBlades(0f);
        }
    }
    
    private void RotateBlades(float speed)
    {
        if (rotationPoint != null && pivot != null)
        {
            float rotationSpeed = Mathf.Clamp(speed, 0, maxSpeed);
            rotationPoint.transform.RotateAround(pivot.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime * 10);
        }
    }
    
    public override void VisualiseConnections()
    {
        if (outputWindFarm != null && visualiser != null)
        {
            visualiser.CreateOrUpdateConnection(gameObject, outputWindFarm.gameObject, currentPower);
        }
    }
    
    // Method to manually set wind speed
    public void SetWindSpeed(float speed)
    {
        if (manualControl)
        {
            manualWindSpeed = speed;
        }
    }
    
    // Method to toggle manual control
    public void ToggleManualControl(bool enabled, float initialSpeed = 0f)
    {
        manualControl = enabled;
        if (enabled)
        {
            manualWindSpeed = initialSpeed;
        }
    }
}