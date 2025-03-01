using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WireConnector : MonoBehaviour
{
    public WindFarm windFarm;
    public LineRenderer circleRenderer;
    public List<LineRenderer> turbineLines = new List<LineRenderer>();
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
            GameObject lineObj = new GameObject("TurbineLine");
            LineRenderer line = lineObj.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default")); // Fix for purple lines
            line.startWidth = 0.05f;
            line.endWidth = 0.05f;
            turbineLines.Add(line);
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
        centerPoint.y += yOffset; // Raise center point by yOffset

        for (int i = 0; i < windFarm.inputTurbines.Count; i++)
        {
            Turbine turbine = windFarm.inputTurbines[i];
            if (turbine == null) continue;

            LineRenderer line = turbineLines[i];
            line.positionCount = 2;

            Vector3 start = turbine.transform.position;
            start.y += yOffset; // Raise turbine connection point

            line.SetPosition(0, start);
            line.SetPosition(1, centerPoint);

            Color turbineColour = turbine.isOperational ? activeColour : inactiveColour;
            line.startColor = turbineColour;
            line.endColor = turbineColour;
        }

        UpdateCircleColour();
    }

    void DrawCircle()
    {
        Vector3 centerPoint = windFarm.GetCenterPoint();
        centerPoint.y += yOffset; // Raise circle position by yOffset
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

    void UpdateCircleColour()
    {
        if (windFarm.inputTurbines.Count == 0) return;

        float activeCount = windFarm.inputTurbines.Count(t => t.isOperational);
        float percentageActive = activeCount / windFarm.inputTurbines.Count;

        Color mixedColour = Color.Lerp(inactiveColour, activeColour, percentageActive);
        circleRenderer.startColor = mixedColour;
        circleRenderer.endColor = mixedColour;
    }
}