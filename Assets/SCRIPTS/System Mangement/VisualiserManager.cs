using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualiserManager : MonoBehaviour
{
    [SerializeField] private PowerVisualiser powerVisualiser;

    [Header("Scene References")]
    // Change object types
    public GameObject windFarm;
    public GameObject inverter;
    public GameObject transformer;
    public GameObject battery;
    public GameObject powerGrid;

    public List<GameObject> turbines = new List<GameObject>();
    
    // Add a reference to check if battery is active/charging
    [Header("Battery State")]
    [SerializeField] private bool isBatteryActive = false;
    [SerializeField] private float batteryPower = 0f;

    void Start()
    {
        // Wait a frame to ensure scene objects are initialised
        StartCoroutine(InitialiseVisualisation());
    }

    private IEnumerator InitialiseVisualisation()
    {
        yield return null; // Wait one frame

        // Stage 1: Turbines to Wind Farm
        var stage1 = new List<(GameObject from, GameObject to, float power, float requiredPower)>();
        foreach (var turbine in turbines)
        {
            var tScript = turbine.GetComponent<Turbine>();
            float power = tScript != null && tScript.isOperational ? tScript.GetCurrentPower() : 0f;
            stage1.Add((turbine, windFarm, power, 0));
        }

        // Stage 2: Wind Farm to Inverter to Transformer
        var farmPower = GetPowerFromFarm();

        // Use power from farm to pass to inverter and transformer
        var stage2 = new List<(GameObject from, GameObject to, float power, float requiredPower)>
        {
            (windFarm, inverter, farmPower, 0),
            (inverter, transformer, farmPower, 0) // Assuming inverter passes same amount
        };

        // Stage 3: Transformer to Grid / Battery and Battery to Grid
        var transformerPower = farmPower; // Replace later with actual transformer output
        var batteryPower = GetBatteryOutput();

        var stage3 = new List<(GameObject from, GameObject to, float power, float requiredPower)>
        {
            (transformer, powerGrid, transformerPower, 0),
            // Only show power flowing to battery if it's active
            (transformer, battery, isBatteryActive ? transformerPower : 0, 0),
            (battery, powerGrid, batteryPower, 0)
        };

        powerVisualiser.InitialiseConnectionsInStages(stage1, stage2, stage3);
    }

    // Update connections when battery state changes
    public void UpdateBatteryState(bool active, float power)
    {
        isBatteryActive = active;
        batteryPower = power;
        
        // Update connections from transformer to battery and battery to grid
        powerVisualiser.CreateOrUpdateConnection(transformer, battery, 
            isBatteryActive ? GetPowerFromFarm() : 0, 0, false);
            
        powerVisualiser.CreateOrUpdateConnection(battery, powerGrid, 
            GetBatteryOutput(), 0, false);
    }

    // Placeholder: You can connect these to actual logic later
    private float GetPowerFromFarm()
    {
        float total = 0f;
        foreach (var turbine in turbines)
        {
            var t = turbine.GetComponent<Turbine>();
            if (t != null && t.isOperational)
                total += t.GetCurrentPower();
        }
        return total;
    }

    private float GetBatteryOutput()
    {
        // Return the battery power only if the battery is active
        return isBatteryActive ? batteryPower : 0f;
    }
    
    // Method to call from other scripts to refresh visuals
    public void RefreshPowerVisuals()
    {
        StartCoroutine(InitialiseVisualisation());
    }
}