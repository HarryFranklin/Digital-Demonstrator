using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PowerVisualiser : MonoBehaviour
{
    [System.Serializable]
    private class DotSettings
    {
        public Color powerFlowingColor = Color.green;
        public Color noPowerColor = Color.red;
        public Color insufficientPowerColor = Color.yellow;
        public float size = 0.3f;
        public float spacing = 1f;
        public float greenSpeed = 1.5f;
        public float yellowSpeed = 0.8f;
        public Material material;
    }

    [System.Serializable]
    private class LineSettings
    {
        public float thinWidth = 0.1f;
        public float thickWidth = 0.4f;
    }

    [SerializeField] private DotSettings dotSettings = new DotSettings();
    [SerializeField] private LineSettings lineSettings = new LineSettings();
    [SerializeField] private bool showStationaryDotsOnNoPower = true;
    public CyberAttackManager cyberAttack;

    // Public structures to allow external modification by attacks
    public Color powerFlowingColor 
    { 
        get => dotSettings.powerFlowingColor; 
        set => dotSettings.powerFlowingColor = value; 
    }
    public Color noPowerColor 
    { 
        get => dotSettings.noPowerColor; 
        set => dotSettings.noPowerColor = value; 
    }
    public Color insufficientPowerColor 
    { 
        get => dotSettings.insufficientPowerColor; 
        set => dotSettings.insufficientPowerColor = value; 
    }

    private Dictionary<(GameObject, GameObject), ConnectionData> connections = new Dictionary<(GameObject, GameObject), ConnectionData>();

    private class ConnectionData
    {
        public List<GameObject> dots = new List<GameObject>();
        public Vector3 start, end;
        public float length;
        public float offset;
        public Color color;
    }

    public bool IsTurbineManipulated(Turbine turbine) => 
        cyberAttack != null && cyberAttack.IsTurbineManipulated(turbine);

    void Update()
    {
        foreach (var conn in connections.Values)
        {
            // Skip entirely if no power and configured to hide dots
            if (!showStationaryDotsOnNoPower && conn.color == dotSettings.noPowerColor)
                continue;

            // Skip animation if no power, but keep dots visible
            if (conn.color == dotSettings.noPowerColor)
                continue;

            float speed = conn.color == dotSettings.powerFlowingColor ? dotSettings.greenSpeed : dotSettings.yellowSpeed;
            float totalCycleLength = dotSettings.size + dotSettings.spacing;
            conn.offset = (conn.offset + speed * Time.deltaTime) % totalCycleLength;

            for (int i = 0; i < conn.dots.Count; i++)
            {
                if (conn.dots[i] == null) continue;
                float distanceAlongLine = (i * totalCycleLength + conn.offset) % conn.length;
                conn.dots[i].transform.position = Vector3.Lerp(conn.start, conn.end, distanceAlongLine / conn.length);
            }
        }
    }

    private Vector3 FindWindFarmCenter(GameObject windFarm) =>
        windFarm.GetComponentsInChildren<Turbine>() is var turbines && turbines.Length > 0
            ? turbines.Select(t => t.transform.position).Aggregate((a, b) => a + b) / turbines.Length
            : windFarm.transform.position;

    public void CreateOrUpdateConnection(GameObject from, GameObject to, float power, float requiredPower = 0, bool isManipulated = false)
    {
        var key = (from, to);
        Vector3 start = from.transform.position;
        Vector3 end = to.transform.position;

        // Special handling for turbine to wind farm
        if (from.CompareTag("WindTurbine") && to.CompareTag("WindFarm"))
        {
            start.y = 0.5f;
            end = FindWindFarmCenter(to);
        }
        else
        {
            start.y = end.y = 0.5f;
        }

        // Determine line properties
        bool isThin = (from.CompareTag("WindTurbine") && to.CompareTag("WindFarm")) || 
                      (from.CompareTag("PowerGrid") && to.CompareTag("PowerConsumer"));
        float width = isThin ? lineSettings.thinWidth : lineSettings.thickWidth;

        // Determine color
        Color color = power <= 0 ? dotSettings.noPowerColor :
            (isManipulated || (requiredPower > 0 && power < requiredPower)) 
                ? dotSettings.insufficientPowerColor 
                : dotSettings.powerFlowingColor;

        // Get or create connection
        if (!connections.TryGetValue(key, out ConnectionData conn))
        {
            conn = new ConnectionData {
                start = start,
                end = end,
                length = Vector3.Distance(start, end),
                color = color
            };

            GameObject parent = new GameObject($"PowerLine_{from.name}_to_{to.name}");
            parent.transform.SetParent(transform);

            // Only create dots if not configured to hide on no power
            if (!(color == dotSettings.noPowerColor && !showStationaryDotsOnNoPower))
            {
                int numDots = Mathf.Max(1, Mathf.CeilToInt(conn.length / (dotSettings.size + dotSettings.spacing)));
                float actualSpacing = (conn.length - (numDots * dotSettings.size)) / (numDots - 1);

                for (int i = 0; i < numDots; i++)
                {
                    GameObject dot = CreateDot(parent.transform, width);
                    conn.dots.Add(dot);

                    float distanceAlongLine = i * (dotSettings.size + actualSpacing);
                    dot.transform.position = Vector3.Lerp(start, end, distanceAlongLine / conn.length);
                    dot.GetComponent<Renderer>().material.color = color;
                }
            }

            connections[key] = conn;
        }
        else
        {
            UpdateExistingConnection(conn, start, end, color);
        }
    }

    private void UpdateExistingConnection(ConnectionData conn, Vector3 start, Vector3 end, Color color)
    {
        conn.start = start;
        conn.end = end;
        conn.length = Vector3.Distance(start, end);

        // Handle dot creation/removal based on no power setting
        if (color == dotSettings.noPowerColor && !showStationaryDotsOnNoPower)
        {
            // Destroy existing dots
            foreach (var dot in conn.dots)
                if (dot != null) Destroy(dot);
            conn.dots.Clear();
        }
        else if (conn.dots.Count == 0 && color == dotSettings.noPowerColor)
        {
            // Recreate dots if they were removed and now showing stationary dots
            int numDots = Mathf.Max(1, Mathf.CeilToInt(conn.length / (dotSettings.size + dotSettings.spacing)));
            float actualSpacing = (conn.length - (numDots * dotSettings.size)) / (numDots - 1);

            GameObject parent = new GameObject($"PowerLine_Restored");
            parent.transform.SetParent(transform);

            for (int i = 0; i < numDots; i++)
            {
                GameObject dot = CreateDot(parent.transform, lineSettings.thinWidth);
                conn.dots.Add(dot);

                float distanceAlongLine = i * (dotSettings.size + actualSpacing);
                dot.transform.position = Vector3.Lerp(start, end, distanceAlongLine / conn.length);
                dot.GetComponent<Renderer>().material.color = color;
            }
        }
        else if (conn.color != color)
        {
            conn.color = color;
            foreach (var dot in conn.dots.Where(d => d != null))
                dot.GetComponent<Renderer>().material.color = color;
        }
    }

    private GameObject CreateDot(Transform parent, float size)
    {
        GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dot.transform.SetParent(parent);
        dot.transform.localScale = Vector3.one * size;

        if (dotSettings.material != null)
        {
            Renderer renderer = dot.GetComponent<Renderer>();
            renderer.material = new Material(dotSettings.material);
        }

        return dot;
    }
    
    public void ForceUpdateAllConnections()
    {
        foreach (var keyValuePair in connections)
        {
            var key = keyValuePair.Key;
            GameObject from = key.Item1;
            GameObject to = key.Item2;
            
            float power = 0f;
            bool isManipulated = false;
            
            // Different process for turbine to farm
            if (from.CompareTag("WindTurbine"))
            {
                Turbine turbine = from.GetComponent<Turbine>();
                if (turbine != null && turbine.isOperational)
                {
                    power = turbine.GetCurrentPower();
                    isManipulated = cyberAttack != null && 
                        cyberAttack.IsTurbineManipulated(turbine) && 
                        to.CompareTag("WindFarm");
                }
            }
            
            CreateOrUpdateConnection(from, to, power, 0, isManipulated);
        }
    }

    // Below methods used in Monitoring attack
    public void HideAllConnections()
    {
        foreach (var conn in connections.Values)
            foreach (var dot in conn.dots)
                if (dot != null) dot.SetActive(false);
    }

    public void ShowAllConnections()
    {
        foreach (var conn in connections.Values)
            foreach (var dot in conn.dots)
                if (dot != null) dot.SetActive(true);
    }
}