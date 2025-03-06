using UnityEngine;

public class Consumer : MonoBehaviour
{
    public PowerGrid inputGrid;
    public float powerInput;
    public float powerConsumption = 10f;

    private PowerLineConnector powerLine;

    void Start()
    {
        powerLine = GetComponent<PowerLineConnector>();
    }

    void Update()
    {
        if (powerLine != null)
        {
            powerLine.powerFlow = powerInput;
        }
    }
}