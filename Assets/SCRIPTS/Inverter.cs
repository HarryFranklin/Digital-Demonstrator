using UnityEngine;

public class Inverter : MonoBehaviour
{
    /**
    Current stage: INVERTER
    Next stage: TRANSFORMER
    **/

    // I/O
    public WindFarm inputFarm;
    public Transformer outputTransformer;
    // I/O

    private float inputPower;
    private float outputPower;

    public void ReceivePower(float power)
    {
        inputPower = power;
    }

    void Update()
    {
        if (outputTransformer != null)
        {
            outputPower = inputPower; // No conversion loss for now
            outputTransformer.ReceivePower(outputPower);
        }
    }
}
