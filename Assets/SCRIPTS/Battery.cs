using UnityEngine;

public class Battery : MonoBehaviour
{
    public Transformer inputTransformer;
    public PowerGrid outputGrid;
    public float capacity = 1000f;  
    public float storedPower = 0f;  
    public float powerInput;
    public float powerOutput; 

    private PowerLineConnector powerLine;

    void Start()
    {
        powerLine = GetComponent<PowerLineConnector>();

        // Ensure connections are correctly assigned
        if (powerLine != null)
        {
            if (inputTransformer != null) powerLine.inputObjects.Add(inputTransformer.transform);
            if (outputGrid != null) powerLine.outputObjects.Add(outputGrid.transform);
        }
    }

    void Update()
    {
        // Store incoming power
        if (powerInput > 0)
        {
            storedPower += powerInput;
            storedPower = Mathf.Min(storedPower, capacity);
        }

        // Discharge power to grid if needed
        float missingPower = outputGrid.totalConsumption - outputGrid.powerInput;
        powerOutput = (missingPower > 0 && storedPower > 0) ? Mathf.Min(missingPower, storedPower) : 0;

        storedPower -= powerOutput;
        storedPower = Mathf.Max(0, storedPower);

        // Send discharged power to the grid
        outputGrid.powerInput += powerOutput;

        // Set power flow values for input and output lines
        if (powerLine != null)
        {
            // Set the power flow for both input and output lines
            powerLine.powerFlow = powerInput > 0 ? powerInput : powerOutput > 0 ? powerOutput : 0;
        }
    }

    public bool isFull()
    {
        return storedPower >= capacity;
    }
}