using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class PowerLineConnector : MonoBehaviour
{
    public List<Transform> inputObjects = new List<Transform>();  // Multiple input sources
    public List<Transform> outputObjects = new List<Transform>(); // Multiple outputs

    public List<LineRenderer> inputLines = new List<LineRenderer>();  // Lines for inputs
    public List<LineRenderer> outputLines = new List<LineRenderer>(); // Lines for outputs

    public float yOffset = 0.5f; // Raise lines above objects for visibility

    public Color activeColor = Color.green;
    public Color inactiveColor = Color.red;
    public float powerFlow = 0f; // Updated externally based on actual power

    void Start()
    {
        InitialiseLines();
    }

    void Update()
    {
        AdjustLineCount(); // Ensure correct number of lines
        UpdateLines(); // Update positions & colors
    }

    void InitialiseLines()
    {
        while (inputLines.Count < inputObjects.Count)
            inputLines.Add(CreateNewLine());

        while (outputLines.Count < outputObjects.Count)
            outputLines.Add(CreateNewLine());
    }

    void AdjustLineCount()
    {
        // Add new input lines if needed
        while (inputLines.Count < inputObjects.Count)
            inputLines.Add(CreateNewLine());

        // Remove extra input lines
        while (inputLines.Count > inputObjects.Count)
        {
            Destroy(inputLines[inputLines.Count - 1].gameObject);
            inputLines.RemoveAt(inputLines.Count - 1);
        }

        // Add new output lines if needed
        while (outputLines.Count < outputObjects.Count)
            outputLines.Add(CreateNewLine());

        // Remove extra output lines
        while (outputLines.Count > outputObjects.Count)
        {
            Destroy(outputLines[outputLines.Count - 1].gameObject);
            outputLines.RemoveAt(outputLines.Count - 1);
        }
    }

    void UpdateLines()
    {
        // Ensure we have enough line renderers
        if (inputObjects.Count > inputLines.Count || outputObjects.Count > outputLines.Count) return;

        // Set positions & colors for input lines
        for (int i = 0; i < inputObjects.Count; i++)
        {
            if (inputObjects[i] == null) continue;

            Vector3 startPos = inputObjects[i].position + Vector3.up * yOffset;
            Vector3 endPos = transform.position + Vector3.up * yOffset;

            inputLines[i].SetPosition(0, startPos);
            inputLines[i].SetPosition(1, endPos);

            // Set the color of the input line based on power flow
            Color lineColor = powerFlow > 0 ? activeColor : inactiveColor;
            inputLines[i].startColor = lineColor;
            inputLines[i].endColor = lineColor;
        }

        // Set positions & colors for output lines
        for (int i = 0; i < outputObjects.Count; i++)
        {
            if (outputObjects[i] == null) continue;

            Vector3 startPos = transform.position + Vector3.up * yOffset;
            Vector3 endPos = outputObjects[i].position + Vector3.up * yOffset;

            outputLines[i].SetPosition(0, startPos);
            outputLines[i].SetPosition(1, endPos);

            // Set the color of the output line based on power flow
            Color lineColor = powerFlow > 0 ? activeColor : inactiveColor;
            outputLines[i].startColor = lineColor;
            outputLines[i].endColor = lineColor;
        }
    }

    LineRenderer CreateNewLine()
    {
        GameObject lineObj = new GameObject("PowerLine");
        lineObj.transform.parent = this.transform;
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.startWidth = 0.2f;
        lr.endWidth = 0.2f;
        lr.material = new Material(Shader.Find("Sprites/Default"));

        return lr;
    }
}