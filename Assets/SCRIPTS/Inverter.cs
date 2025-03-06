using UnityEngine;

public class Inverter : MonoBehaviour
{
    public WindFarm inputWindFarm;
    public Transformer outputTransformer;
    public float powerInput;
    
    private PowerLineConnector powerLine;

    void Start()
    {
        powerLine = GetComponent<PowerLineConnector>();

        // Ensure the wind farm's center is treated as the input
        if (powerLine != null && inputWindFarm != null)
        {
            powerLine.inputObjects.Clear();
            GameObject windFarmCenter = new GameObject("WindFarmCenter");
            windFarmCenter.transform.position = inputWindFarm.GetCenterPoint();
            powerLine.inputObjects.Add(windFarmCenter.transform);
        }

        // Ensure the output transformer is correctly assigned
        if (powerLine != null && outputTransformer != null)
        {
            powerLine.outputObjects.Add(outputTransformer.transform);
        }
    }

    void Update()
    {
        powerInput = inputWindFarm.totalPowerInput;

        if (powerLine != null)
        {
            // Update the wind farm center position dynamically
            if (powerLine.inputObjects.Count > 0 && powerLine.inputObjects[0] != null)
            {
                powerLine.inputObjects[0].position = inputWindFarm.GetCenterPoint();
            }

            powerLine.powerFlow = powerInput;
        }

        if (outputTransformer != null)
        {
            outputTransformer.powerInput = powerInput;
        }
    }
}