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
    private float yOffset = 0.5f; 

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
            if (turbine.GetComponent<LineRenderer>() == null)
            {
                LineRenderer line = turbine.gameObject.AddComponent<LineRenderer>();
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.startWidth = 0.05f;
                line.endWidth = 0.05f;
            }
        }
    }

    void Update()
    {
        DrawConnections();
        UpdateCircleColour();
    }

    void DrawConnections()
    {
        if (windFarm.inputTurbines.Count == 0) return;

        Vector3 centerPoint = windFarm.GetCenterPoint();
        centerPoint.y += yOffset;

        float maxPower = windFarm.totalPowerOutput > 0 ? windFarm.totalPowerOutput : 1; // Prevent division errors

        foreach (Turbine turbine in windFarm.inputTurbines)
        {
            if (turbine == null) continue;

            LineRenderer line = turbine.GetComponent<LineRenderer>();
            line.positionCount = 2;

            Vector3 start = turbine.transform.position;
            start.y += yOffset;

            line.SetPosition(0, start);
            line.SetPosition(1, centerPoint);

            float powerRatio = Mathf.Clamp01(turbine.GetPowerOutput() / maxPower); // Normalize power ratio
            line.startColor = Color.Lerp(inactiveColour, activeColour, powerRatio);
            line.endColor = Color.Lerp(inactiveColour, activeColour, powerRatio);
        }
    }

    void UpdateCircleColour()
    {
        if (windFarm.inputTurbines.Count == 0) return;

        float totalPower = 0f;
        float activePower = 0f;

        foreach (Turbine turbine in windFarm.inputTurbines)
        {
            if (turbine.isOperational)
            {
                activePower += turbine.GetPowerOutput();
            }
            totalPower += turbine.GetPowerOutput();
        }

        float powerRatio = totalPower > 0 ? activePower / totalPower : 0f;
        Color mixedColour = Color.Lerp(inactiveColour, activeColour, powerRatio);
        circleRenderer.startColor = mixedColour;
        circleRenderer.endColor = mixedColour;
    }
}