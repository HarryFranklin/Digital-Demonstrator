using UnityEngine;

public class Battery : ElectricalComponent
{
    public Transformer inputTransformer;
    public PowerGrid outputGrid;

    [Header("Battery Status")]
    [SerializeField] private float currentCharge = 50f;
    [SerializeField] private bool isCharging = false;
    public float capacity = 100f;
    private float chargeRate = 10f;
    private float dischargeRate = 10f;

    public override float Charge(float power)
    {
        float storedPower = Mathf.Min(power * chargeRate * Time.deltaTime, capacity - currentCharge);
        currentCharge += storedPower;
        isCharging = storedPower > 0;
        return storedPower;
    }

    public override float Discharge(float requestedPower)
    {
        float availablePower = Mathf.Min(requestedPower, currentCharge * dischargeRate * Time.deltaTime);
        currentCharge -= availablePower;
        isCharging = false;
        return availablePower;
    }

    public bool IsFull()
    {
        return currentCharge >= capacity;
    }

    void Update()
    {
        if (outputGrid != null)
        {
            float neededPower = outputGrid.ProvidePower(0);
            if (neededPower > 0)
            {
                float dischargedPower = Discharge(neededPower);
                outputGrid.ReceivePower(dischargedPower);
            }
        }
    }
}