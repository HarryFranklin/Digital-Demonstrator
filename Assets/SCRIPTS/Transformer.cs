using UnityEngine;

public class Transformer : MonoBehaviour
{
    public Inverter inputInverter;
    public PowerGrid outputGrid;
    public Battery outputBattery;
    public float powerInput;

    private PowerLineConnector powerLine;

    void Start()
    {
        powerLine = GetComponent<PowerLineConnector>();
    }

    void Update()
    {
        powerInput = inputInverter.powerInput;
        float gridDemand = outputGrid.totalConsumption;

        float powerToGrid = Mathf.Min(powerInput, gridDemand);
        float powerToBattery = Mathf.Max(0, powerInput - gridDemand);

        if (powerLine != null)
        {
            powerLine.powerFlow = powerToGrid + powerToBattery;
        }

        outputGrid.powerInput = powerToGrid;
        outputBattery.powerInput = powerToBattery;
    }
}