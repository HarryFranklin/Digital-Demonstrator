using UnityEngine;

public class Inverter : MonoBehaviour
{
    /**
    Current stage: INVERTER
    Next stage: TRANSFORMER
    **/

    // I/O
    public WindFarm inputFarm;  // Reference to the input wind farm
    public Transformer outputTransformer;  // Reference to the next stage (transformer)
    // I/O

    public float powerInput;

    void Update()
    {
        //outputTransformer.powerInput = powerInput;
    }
}