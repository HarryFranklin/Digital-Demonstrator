using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WireConnector : MonoBehaviour
{
    public WindFarm windFarm;
    public LineRenderer circleRenderer;
    public int circleSegments = 36;
    public float circleRadius = 0.5f;
    public Color activeColour = Color.green;
    public Color inactiveColour = Color.red;
    public Color futureColour = Color.yellow;
    private float yOffset = 0.5f; // Offset to raise the lines

    void Start()
    {
        if (windFarm == null)
        {
            windFarm = GetComponent<WindFarm>();
        }

        if (circleRenderer == null)
        {
            GameObject circleObject = new GameObject("CircleRenderer");
            circleRenderer = circleObject.AddComponent<LineRenderer>();
            circleRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        circleRenderer.startWidth = 0.05f * 5;
        circleRenderer.endWidth = 0.05f * 5;
        circleRenderer.loop = true;
        circleRenderer.positionCount = circleSegments;

        foreach (Turbine turbine in windFarm.inputTurbines)
        {
            Transform lineHolder = turbine.transform.Find("LineRendererHolder");
            if (lineHolder == null)
            {
                GameObject holderObj = new GameObject("LineRendererHolder");
                holderObj.transform.parent = turbine.transform;
                lineHolder = holderObj.transform;
            }

            if (lineHolder.GetComponent<LineRenderer>() == null)
            {
                LineRenderer line = lineHolder.gameObject.AddComponent<LineRenderer>();
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.startWidth = 0.05f;
                line.endWidth = 0.05f;
            }
        }
    }

    void Update()
    {
        DrawConnections();
        DrawCircle();
    }

    void DrawConnections()
    {
        if (windFarm.inputTurbines.Count == 0) return;

        Vector3 centerPoint = windFarm.GetCenterPoint();
        centerPoint.y += yOffset;

        foreach (Turbine turbine in windFarm.inputTurbines)
        {
            if (turbine == null) continue;

            turbine.ReceivePower(turbine.windSpeed); // Input power comes from wind speed

            if (turbine.isOperational && turbine.rotationPoint != null)
            {
                float rotationSpeed = Mathf.Clamp(turbine.windSpeed, 0, turbine.maxSpeed);
                turbine.rotationPoint.transform.RotateAround(turbine.pivot.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime * 10);
                turbine.outputPower = turbine.GetPowerOutput();
            }
            else 
            {
                turbine.outputPower = 0;
            }

            Transform lineHolder = turbine.transform.Find("LineRendererHolder");
            if (lineHolder == null) continue;

            LineRenderer line = lineHolder.GetComponent<LineRenderer>();
            line.positionCount = 2;

            Vector3 start = turbine.transform.position;
            start.y += yOffset;

            line.SetPosition(0, start);
            line.SetPosition(1, centerPoint);

            float powerRatio = turbine.GetPowerOutput() / Mathf.Max(1, windFarm.totalPowerOutput);
            line.startColor = Color.Lerp(inactiveColour, activeColour, powerRatio);
            line.endColor = Color.Lerp(inactiveColour, activeColour, powerRatio);
        }
    }

    void DrawCircle()
    {
        Vector3 centerPoint = windFarm.GetCenterPoint();
        centerPoint.y += yOffset;
        Vector3[] circlePoints = new Vector3[circleSegments];

        for (int i = 0; i < circleSegments; i++)
        {
            float angle = i * 2 * Mathf.PI / circleSegments;
            float x = Mathf.Cos(angle) * circleRadius;
            float z = Mathf.Sin(angle) * circleRadius;
            circlePoints[i] = new Vector3(centerPoint.x + x, centerPoint.y, centerPoint.z + z);
        }

        circleRenderer.SetPositions(circlePoints);
    }
}