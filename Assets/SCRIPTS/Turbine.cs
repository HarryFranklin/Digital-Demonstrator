using UnityEngine;

public class Turbine : MonoBehaviour
{
    public float windSpeed = 10f; // Controls rotation speed
    public float maxSpeed = 20f; // Max speed before efficiency drops off, and side effects can happen
    public bool isOperational = true; // Is the turbine on? Set to True by default

    public float powerOutput;
    public GameObject rotationPoint; // The part that should rotate (blades)
    public GameObject pivot; // The part that the blades should rotate around

    void Start()
    {
        
    }

    void Update()
    {
        if (isOperational && rotationPoint != null)
        {
            float rotationSpeed = Mathf.Clamp(windSpeed, 0, maxSpeed);
            rotationPoint.transform.RotateAround(pivot.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime * 10);
            powerOutput = CalculatePower(rotationSpeed);
        }
        else 
        { 
            powerOutput = 0; 
        } 
    }

    private float CalculatePower(float speed)
    {
        return speed; // Placeholder logic
    }

    public float GetPowerOutput()
    {
        return powerOutput;
    }
}
