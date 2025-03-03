using UnityEngine;

public class Turbine : ElectricalComponent
{
    public WindFarm outputWindFarm;
    public float windSpeed = 10f;
    public float maxSpeed = 20f;
    public bool isOperational = true;

    public GameObject rotationPoint;
    public GameObject pivot;

    void Update()
    {
        float adjustedSpeed = windSpeed;

        if (outputWindFarm != null && outputWindFarm.inverter.outputTransformer.outputBattery.IsFull())
        {
            adjustedSpeed *= 0.5f; // Slow down when battery is full
        }

        if (isOperational && rotationPoint != null)
        {
            float rotationSpeed = Mathf.Clamp(adjustedSpeed, 0, maxSpeed);
            rotationPoint.transform.RotateAround(pivot.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime * 10);
            SetOutputPower(rotationSpeed);  // Assign power based on speed
        }
        else
        {
            SetOutputPower(0);
        }
    }
}
