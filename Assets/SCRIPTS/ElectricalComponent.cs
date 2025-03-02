using UnityEngine;
using System.Collections.Generic;

public abstract class ElectricalComponent : MonoBehaviour
{
    protected float inputPower = 0f;
    protected float outputPower = 0f;

    public virtual void ReceivePower(float power)
    {
        inputPower = power;
    }

    public virtual float ProvidePower(float requestedPower)
    {
        float providedPower = Mathf.Min(requestedPower, outputPower);
        outputPower -= providedPower;
        return providedPower;
    }

    public virtual float Charge(float power) { return 0f; }  // Default, overridden by Battery
    public virtual float Discharge(float requestedPower) { return 0f; }  // Default, overridden by Battery
}