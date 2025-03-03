using UnityEngine;

public class Turbine : MonoBehaviour
{
    // I/O
    public WindFarm outputWindFarm;
    // I/O

    public float windSpeed = 10f; // Wind speed for simulation
    public float maxSpeed = 20f; // Max rotation speed
    public bool isOperational = true; // Can be toggled (e.g., for attacks)

    // Rotation functionality
    public GameObject rotationPoint;
    public GameObject pivot;

    // Power
    public float powerOutput;

    void Update()
    {
        if (isOperational)
        {
            RotateBlades();
            powerOutput = windSpeed; // Directly use wind speed as power output
        }
        else
        {
            powerOutput = 0f; // No power if turbine is offline
        }

        // Send updated power to wind farm
        if (outputWindFarm != null)
        {
            outputWindFarm.ReceivePower(powerOutput);
        }
    }

    private void RotateBlades()
    {
        if (rotationPoint != null && pivot != null)
        {
            float rotationSpeed = Mathf.Clamp(windSpeed, 0, maxSpeed);
            rotationPoint.transform.RotateAround(pivot.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime * 10);
        }
    }
}