using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerVisualiser : MonoBehaviour
{
    public Color powerFlowingColor = Color.green;
    public Color noPowerColor = Color.red;
    public Color insufficientPowerColor = Color.yellow;
    public float thinLineWidth = 0.1f;
    public float thickLineWidth = 0.4f;
    public CyberAttackManager cyberAttack;
    
    // Dot animation settings
    public float greenDotSpeed = 1.5f;
    public float yellowDotSpeed = 0.8f;
    public float dotSize = 0.3f;
    public float dotGap = 0.7f;
    public Material dotMaterial;
    
    // Dictionary to track connections
    private Dictionary<(GameObject, GameObject), PowerLineConnection> connections = 
        new Dictionary<(GameObject, GameObject), PowerLineConnection>();
    
    // Simplified connection class
    private class PowerLineConnection
    {
        public List<GameObject> dots = new List<GameObject>();
        public Vector3 start, end;
        public float length;
        public float offset = 0f;
        public Color color;
    }
    
    public bool IsTurbineManipulated(Turbine turbine) => 
        cyberAttack != null && cyberAttack.IsTurbineManipulated(turbine);
        // If cyberAttack script is there, and the cyber attack says the turbine is manipulated, it's manipulated
    
    void Update()
    {
        foreach (var conn in connections.Values)
        {
            if (conn.color == noPowerColor || conn.dots.Count == 0) continue;
            
            // Update animation based on color
            float speed = conn.color == powerFlowingColor ? greenDotSpeed : yellowDotSpeed;
            conn.offset = (conn.offset + speed * Time.deltaTime) % (dotSize + dotGap);
            
            // Update dot positions based on post found online
            float cycle = dotSize + dotGap;
            for (int i = 0; i < conn.dots.Count; i++)
            {
                float position = (i * cycle + conn.offset) % conn.length;
                if (conn.dots[i] != null)
                {
                    conn.dots[i].transform.position = Vector3.Lerp(
                        conn.start, conn.end, position / conn.length);
                }
            }
        }
    }
    
    // Used to create or update a given connection, key into it using the from and to GO's.
    public void CreateOrUpdateConnection(GameObject from, GameObject to, float power, float requiredPower = 0, bool isManipulated = false)
    {
        // Create key and prepare positions
        var key = (from, to);
        Vector3 start = from.transform.position;
        Vector3 end = to.transform.position;
        start.y = end.y = 0.5f;
        
        // Determine width and color
        bool isThin = (from.CompareTag("WindTurbine") && to.CompareTag("WindFarm")) || 
                      (from.CompareTag("PowerGrid") && to.CompareTag("PowerConsumer"));
        float width = isThin ? thinLineWidth : thickLineWidth;
        
        Color color = noPowerColor;
        if (power > 0)
        {
            if (isManipulated || (requiredPower > 0 && power < requiredPower))
                color = insufficientPowerColor;
            else
                color = powerFlowingColor;
        }
        
        // Get or create connection
        if (!connections.TryGetValue(key, out PowerLineConnection conn))
        {
            // Create new connection
            conn = new PowerLineConnection {
                start = start,
                end = end,
                length = Vector3.Distance(start, end),
                color = color
            };
            
            // Create parent object
            GameObject parent = new GameObject($"PowerLine_{from.name}_to_{to.name}");
            parent.transform.SetParent(transform);
            
            // Create dots
            int numDots = Mathf.CeilToInt(conn.length / (dotSize + dotGap));
            for (int i = 0; i < numDots; i++)
            {
                GameObject dot = CreateDot(parent.transform, width);
                conn.dots.Add(dot);
                
                // Set initial position and color
                float pos = (i * (dotSize + dotGap)) % conn.length;
                dot.transform.position = Vector3.Lerp(start, end, pos / conn.length);
                dot.GetComponent<Renderer>().material.color = color;
            }
            
            connections[key] = conn;
        }
        else
        {
            // Update existing connection
            bool wasRed = conn.color == noPowerColor;
            conn.start = start;
            conn.end = end;
            conn.length = Vector3.Distance(start, end);
            
            // Update colors and reset if needed
            if (conn.color != color)
            {
                conn.color = color;
                foreach (var dot in conn.dots)
                {
                    if (dot != null)
                        dot.GetComponent<Renderer>().material.color = color;
                }
                
                // Reset positions if changing from red to another color
                if (wasRed && color != noPowerColor)
                {
                    conn.offset = 0f;
                    float cycle = dotSize + dotGap;
                    for (int i = 0; i < conn.dots.Count; i++)
                    {
                        float pos = (i * cycle) % conn.length;
                        if (conn.dots[i] != null)
                            conn.dots[i].transform.position = Vector3.Lerp(start, end, pos / conn.length);
                    }
                }
            }
        }
    }
    
    private GameObject CreateDot(Transform parent, float size)
    {
        GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dot.transform.SetParent(parent);
        dot.transform.localScale = Vector3.one * size;
        
        if (dotMaterial != null)
        {
            Renderer renderer = dot.GetComponent<Renderer>();
            renderer.material = new Material(dotMaterial);
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