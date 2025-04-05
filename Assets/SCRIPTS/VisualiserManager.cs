using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualiserManager : MonoBehaviour
{
    [SerializeField] private PowerVisualiser powerVisualiser;

    [Header("Scene References")]
    public GameObject windFarm;
    public GameObject inverter;
    public GameObject transformer;
    public GameObject battery;
    public GameObject powerGrid;

    public List<GameObject> turbines = new List<GameObject>();

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
        var stage2 = new List<(GameObject from, GameObject to, float power, float requiredPower)>
        {
            (windFarm, inverter, GetPowerFromFarm(), 0),
            (inverter, transformer, GetPowerFromFarm(), 0) // Assuming inverter passes same amount
        };

        // Stage 3: Transformer to Grid / Battery and Battery to Grid
        var transformerPower = GetPowerFromFarm(); // You can replace this with actual transformer output

        var stage3 = new List<(GameObject from, GameObject to, float power, float requiredPower)>
        {
            (transformer, powerGrid, transformerPower, 0),
            (transformer, battery, transformerPower, 0),
            (battery, powerGrid, GetBatteryOutput(), 0)
        };

        powerVisualiser.InitialiseConnectionsInStages(stage1, stage2, stage3);
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
        // Replace with actual battery output logic
        return 0f;
    }
}
