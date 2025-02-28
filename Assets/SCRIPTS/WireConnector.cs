using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WireConnector : MonoBehaviour
{
    public List<Turbine> connectedTurbines; // Assign turbines in the Inspector
    public LineRenderer lineRenderer;
    public LineRenderer circleRenderer;
    private Vector3 centerPoint;
    public int circleSegments = 36;
    public float circleRadius = 0.5f;
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.red;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        
        if (circleRenderer == null)
        {
            GameObject circleObject = new GameObject("CircleRenderer");
            circleRenderer = circleObject.AddComponent<LineRenderer>();
        }

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = connectedTurbines.Count * 2;

        circleRenderer.startWidth = 0.05f * 5;
        circleRenderer.endWidth = 0.05f * 5;
        circleRenderer.loop = true;
        circleRenderer.positionCount = circleSegments;
    }

    void Update()
    {
        UpdateWireConnections();
        DrawCircle();
    }

    void UpdateWireConnections()
    {
        if (connectedTurbines == null || connectedTurbines.Count == 0) return;

        // Calculate the center point
        centerPoint = Vector3.zero;
        foreach (Turbine turbine in connectedTurbines)
        {
            centerPoint += turbine.transform.position;
        }
        centerPoint /= connectedTurbines.Count;

        // Update LineRenderer
        lineRenderer.positionCount = connectedTurbines.Count * 2;
        for (int i = 0; i < connectedTurbines.Count; i++)
        {
            lineRenderer.SetPosition(i * 2, connectedTurbines[i].transform.position);
            lineRenderer.SetPosition(i * 2 + 1, centerPoint);
            
            Color turbineColor = connectedTurbines[i].isOperational ? activeColor : inactiveColor;
            lineRenderer.startColor = turbineColor;
            lineRenderer.endColor = turbineColor;
        }

        // Update Circle Color Based on Average Turbine Status
        float activeCount = connectedTurbines.Count(t => t.isOperational);
        float percentageActive = activeCount / connectedTurbines.Count;
        circleRenderer.startColor = Color.Lerp(inactiveColor, activeColor, percentageActive);
        circleRenderer.endColor = Color.Lerp(inactiveColor, activeColor, percentageActive);
    }

    void DrawCircle()
    {
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