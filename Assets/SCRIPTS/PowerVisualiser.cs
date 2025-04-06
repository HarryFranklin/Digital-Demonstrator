using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PowerVisualiser : MonoBehaviour
{
    [System.Serializable]
    public class VisualSettings
    {
        public Color powerFlowingColor = Color.green;
        public Color noPowerColor = Color.red;
        public Color insufficientPowerColor = Color.yellow;
        public float dotSize = 0.3f;
        public float smallDotSize = 0.15f;
        public float flowSpeed = 1.5f;
        public float slowFlowSpeed = 0.8f;
    }

    [SerializeField] private VisualSettings visualSettings = new VisualSettings();
    [SerializeField] private bool showDotsWhenNoPower = false;
    [SerializeField] private GameObject dotPrefab; // Prefab for sphere with no collider/shadows for better efficiency
    [SerializeField] private float pathElevation = 10.0f; // Height above terrain for dot paths
    public CyberAttackManager cyberAttack;

    // Public properties for attack modifications
    public Color powerFlowingColor { get => visualSettings.powerFlowingColor; set => visualSettings.powerFlowingColor = value; }
    public Color noPowerColor { get => visualSettings.noPowerColor; set => visualSettings.noPowerColor = value; }
    public Color insufficientPowerColor { get => visualSettings.insufficientPowerColor; set => visualSettings.insufficientPowerColor = value; }

    private Dictionary<(GameObject, GameObject), ConnectionData> connections = new Dictionary<(GameObject, GameObject), ConnectionData>();

    private class ConnectionData
    {
        public GameObject parentObject;
        public List<GameObject> dots = new List<GameObject>();
        public Vector3 start, end;
        public float length;
        public float offset;
        public Color color;
        public bool isSecondary;
        public float dotSpacing; // Spacing for this connection
    }

    public bool IsTurbineManipulated(Turbine turbine) => 
        cyberAttack != null && cyberAttack.IsTurbineManipulated(turbine);

    void Update()
    {
        if (connections.Count == 0)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        
        foreach (var conn in connections.Values)
        {
            // Skip animation for no power connections or inactive objects
            if (conn.color == visualSettings.noPowerColor || !conn.parentObject.activeSelf)
                continue;

            // Calculate speed based on connection status
            float speed = conn.color == visualSettings.powerFlowingColor ? 
                visualSettings.flowSpeed : visualSettings.slowFlowSpeed;
            
            // Update offset
            conn.offset = (conn.offset + speed * deltaTime) % conn.dotSpacing;

            // Update dot positions
            for (int i = 0; i < conn.dots.Count; i++)
            {
                float distanceAlongLine = (i * conn.dotSpacing + conn.offset) % conn.length;
                float normalisedPos = distanceAlongLine / conn.length;
                conn.dots[i].transform.position = Vector3.Lerp(conn.start, conn.end, normalisedPos);
            }
        }
    }

    private Vector3 FindWindFarmCenter(GameObject windFarm)
    {
        var turbines = windFarm.GetComponentsInChildren<Turbine>();
        if (turbines.Length == 0) return windFarm.transform.position;
        
        Vector3 sum = Vector3.zero;
        foreach (var turbine in turbines)
        {
            sum += turbine.transform.position;
        }
        return sum / turbines.Length;
    }

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

        // Elevate the start and end points to ensure dots are above terrain
        start.y += pathElevation;
        end.y += pathElevation;

        // Determine if this is a secondary connection
        bool isSecondary = (from.CompareTag("WindTurbine") && to.CompareTag("WindFarm")) || 
                           (from.CompareTag("PowerGrid") && to.CompareTag("PowerConsumer"));
        
        // Determine color
        Color color = power <= 0 ? visualSettings.noPowerColor :
            (isManipulated || (requiredPower > 0 && power < requiredPower)) 
                ? visualSettings.insufficientPowerColor 
                : visualSettings.powerFlowingColor;

        // Get current dot size
        float dotSize = isSecondary ? visualSettings.smallDotSize : visualSettings.dotSize;
        // Calculate dynamic spacing (5 * dotSize)
        float dotSpacing = dotSize * 7.5f;

        // Get or create connection
        if (!connections.TryGetValue(key, out ConnectionData conn))
        {
            conn = CreateNewConnection(from, to, start, end, color, isSecondary, dotSize, dotSpacing);
            connections[key] = conn;
        }
        else
        {
            UpdateExistingConnection(conn, start, end, color, dotSize, dotSpacing);
        }
    }

    private ConnectionData CreateNewConnection(GameObject from, GameObject to, Vector3 start, Vector3 end, Color color, bool isSecondary, float dotSize, float dotSpacing)
    {
        ConnectionData conn = new ConnectionData
        {
            start = start,
            end = end,
            length = Vector3.Distance(start, end),
            color = color,
            isSecondary = isSecondary,
            offset = 0f,
            dotSpacing = dotSpacing
        };

        // Create parent object
        conn.parentObject = new GameObject($"PowerConnection_{from.name}_to_{to.name}");
        conn.parentObject.transform.SetParent(transform);
        
        // Calculate number of dots needed
        int numDots = Mathf.Max(1, Mathf.CeilToInt(conn.length / dotSpacing));
        
        // Create dots at regular intervals
        for (int i = 0; i < numDots; i++)
        {
            GameObject dot = CreateDot(conn.parentObject.transform, dotSize, color);
            conn.dots.Add(dot);
            
            // Set initial position
            float initialPos = (i * dotSpacing) % conn.length;
            dot.transform.position = Vector3.Lerp(start, end, initialPos / conn.length);
            
            // Hide dots if no power
            dot.SetActive(color != visualSettings.noPowerColor || showDotsWhenNoPower);
        }

        return conn;
    }

    private void UpdateExistingConnection(ConnectionData conn, Vector3 start, Vector3 end, Color color, float dotSize, float dotSpacing)
    {
        // Update connection data
        conn.start = start;
        conn.end = end;
        float newLength = Vector3.Distance(start, end);
        
        // If length changed significantly or spacing changed, recreate dots
        if (Mathf.Abs(newLength - conn.length) > 0.5f || conn.dotSpacing != dotSpacing)
        {
            // Clean up existing dots
            foreach (var dot in conn.dots)
            {
                Destroy(dot);
            }
            conn.dots.Clear();
            
            // Update spacing
            conn.dotSpacing = dotSpacing;
            
            // Calculate new number of dots
            int numDots = Mathf.Max(1, Mathf.CeilToInt(newLength / dotSpacing));
            
            // Create new dots
            for (int i = 0; i < numDots; i++)
            {
                GameObject dot = CreateDot(conn.parentObject.transform, dotSize, color);
                conn.dots.Add(dot);
                
                // Set initial position
                float initialPos = (i * dotSpacing) % newLength;
                dot.transform.position = Vector3.Lerp(start, end, initialPos / newLength);
                
                // Set visibility based on power status
                dot.SetActive(color != visualSettings.noPowerColor || showDotsWhenNoPower);
            }
        }
        
        conn.length = newLength;

        // Update color if changed
        if (conn.color != color)
        {
            conn.color = color;
            
            // Update all dot colors and visibility
            foreach (var dot in conn.dots)
            {
                var renderer = dot.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = color;
                }
                
                // Update visibility based on power status
                dot.SetActive(color != visualSettings.noPowerColor || showDotsWhenNoPower);
            }
        }
    }

    private GameObject CreateDot(Transform parent, float size, Color color)
    {
        GameObject dot;
        
        // Use prefab (should have no collider and no shadows)
        dot = Instantiate(dotPrefab, parent);
        dot.transform.localScale = Vector3.one * size;
        
        // Set color
        dot.GetComponent<Renderer>().material.color = color;
        
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

    // Methods used in Monitoring attack
    public void HideAllConnections()
    {
        foreach (var conn in connections.Values)
        {
            conn.parentObject?.SetActive(false);
        }
    }

    public void ShowAllConnections()
    {
        foreach (var conn in connections.Values)
        {
            if (conn.parentObject != null)
            {
                conn.parentObject.SetActive(true);
                
                // Also update individual dot visibility
                foreach (var dot in conn.dots)
                {
                    dot?.SetActive(conn.color != visualSettings.noPowerColor || showDotsWhenNoPower);
                }
            }
        }
    }

    // Visualiser Manager functionality
    public void InitialiseConnectionsInStages(List<(GameObject from, GameObject to, float power, float requiredPower)> stage1,
                                          List<(GameObject from, GameObject to, float power, float requiredPower)> stage2,
                                          List<(GameObject from, GameObject to, float power, float requiredPower)> stage3)
    {
        StartCoroutine(StageConnectionCoroutine(stage1, stage2, stage3));
    }

    private IEnumerator StageConnectionCoroutine(List<(GameObject, GameObject, float, float)> stage1,
                                                List<(GameObject, GameObject, float, float)> stage2,
                                                List<(GameObject, GameObject, float, float)> stage3)
    {
        // Hide all at start
        HideAllConnections();

        yield return null; // Wait a frame to let objects initialise
        
        CreateStage(stage1);
        yield return new WaitForSeconds(0.1f); // Or 1 frame if you want instant

        CreateStage(stage2);
        yield return new WaitForSeconds(0.1f);

        CreateStage(stage3);
        ShowAllConnections();
    }

    private void CreateStage(List<(GameObject from, GameObject to, float power, float requiredPower)> stage)
    {
        foreach (var (from, to, power, requiredPower) in stage)
        {
            bool manipulated = false;
            if (from.CompareTag("WindTurbine"))
            {
                var turbine = from.GetComponent<Turbine>();
                manipulated = turbine != null && cyberAttack != null && cyberAttack.IsTurbineManipulated(turbine);
            }

            CreateOrUpdateConnection(from, to, power, requiredPower, manipulated);
        }
    }
}