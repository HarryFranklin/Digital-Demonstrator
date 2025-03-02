using UnityEngine;

public abstract class ElectricalComponent : MonoBehaviour
{
    [Header("Power Flow")]
    [SerializeField] public float inputPower = 0f;  // Serialised to show in Inspector
    [SerializeField] public float outputPower = 0f;

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

    public virtual float Charge(float power) { return 0f; }  // Default for Battery override
    public virtual float Discharge(float requestedPower) { return 0f; }  // Default for Battery override

    // Debugging function to manually update in Unity Editor
    [ContextMenu("Refresh Power Data")]
    public void RefreshPowerData()
    {
        Debug.Log($"{gameObject.name} - Input: {inputPower}, Output: {outputPower}");
    }

    // Draw power flow in the scene
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * (inputPower / 10f));
    }
}
