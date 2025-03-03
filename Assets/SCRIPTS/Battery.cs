using UnityEngine;

public class Battery : MonoBehaviour
{
    /**
    Current stage: BATTERY
    Next stage: GRID
    **/

    public float capacity = 100f;
    public float storedPower = 0f;
    public Transformer inputTransformer;
    public PowerGrid outputGrid;

    public float Charge(float power)
    {
        float availableSpace = capacity - storedPower;
        float powerToStore = Mathf.Min(availableSpace, power);
        storedPower += powerToStore;

        if (power > powerToStore)
        {
            Debug.Log("Battery full, excess power available.");
        }

        return powerToStore;
    }

    public float Discharge(float neededPower)
    {
        float powerToProvide = Mathf.Min(storedPower, neededPower);
        storedPower -= powerToProvide;
        return powerToProvide;
    }

    void Update()
    {
        float gridDemand = outputGrid.GetPowerDemand();
        float availablePower = inputTransformer.inputPower;

        if (availablePower < gridDemand)
        {
            float shortfall = gridDemand - availablePower;
            float suppliedPower = Discharge(shortfall);
            outputGrid.ReceivePower(availablePower + suppliedPower);
        }
        else
        {
            outputGrid.ReceivePower(availablePower);
            float excessPower = availablePower - gridDemand;
            if (excessPower > 0)
            {
                Charge(excessPower);
            }
        }
    }
}