using UnityEngine;

public abstract class ElectricalComponent : MonoBehaviour
{
    [Header("Power Flow")]
    [SerializeField] protected float inputPower = 0f;
    [SerializeField] protected float outputPower = 0f;

    public float OutputPower => outputPower; // Public getter for access

    public virtual void ReceivePower(float power)
    {
        inputPower = power;
        SetOutputPower(power);
    }

    public virtual float ProvidePower(float requestedPower)
    {
        float providedPower = Mathf.Min(requestedPower, outputPower);
        outputPower -= providedPower;
        return providedPower;
    }

    public virtual float Charge(float power) { return 0f; }
    public virtual float Discharge(float requestedPower) { return 0f; }

    protected void SetOutputPower(float power)
    {
        outputPower = power;
    }
}