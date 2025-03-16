using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerVisualiser : MonoBehaviour
{
    public Color powerFlowingColor = Color.green;
    public Color noPowerColor = Color.red;
    public Color insufficientPowerColor = Color.yellow; // For when the consumer isn't powered enough.
    public float thinLineWidth = 0.1f; // Turbine -> Farm, Grid -> Consumer
    public float thickLineWidth = 0.4f; // All other connections
    public CyberAttack cyberAttack;
    
    // Dict of 2 GO's (from and to) as key, lr as value.
    private Dictionary<(GameObject, GameObject), LineRenderer> connectionLines = new Dictionary<(GameObject, GameObject), LineRenderer>();
    
    // Method to check if a turbine is being manipulated
    public bool IsTurbineManipulated(Turbine turbine)
    {
        if (cyberAttack != null)
        {
            return cyberAttack.IsTurbineManipulated(turbine);
        }
        return false;
    }
    
    public LineRenderer CreateOrUpdateConnection(GameObject from, GameObject to, float power, float requiredPower = 0, bool isManipulated = false)
    {
        var key = (from, to);
        LineRenderer line;
        
        // If connection doesn't exist, create it and add to dict
        if (!connectionLines.TryGetValue(key, out line))
        {
            GameObject lineObj = new GameObject($"PowerLine_{from.name}_to_{to.name}");
            lineObj.transform.SetParent(transform);
            
            line = lineObj.AddComponent<LineRenderer>();
            line.positionCount = 2;
            
            connectionLines[key] = line;
        }
        
        // Determine line thickness
        bool isThinLine = (from.CompareTag("WindTurbine") && to.CompareTag("WindFarm")) || (from.CompareTag("PowerGrid") && to.CompareTag("PowerConsumer"));
        line.startWidth = isThinLine ? thinLineWidth : thickLineWidth;
        line.endWidth = isThinLine ? thinLineWidth : thickLineWidth;
        
        // Create variables and forcing Y value
        Vector3 fromPos = from.transform.position;
        Vector3 toPos = to.transform.position;
        fromPos.y = 0.5f;
        toPos.y = 0.5f;

        // Update positions
        line.SetPosition(0, fromPos);
        line.SetPosition(1, toPos);

        // Set color based on power flow and manipulation status
        if (power <= 0)
        {
            line.material.color = noPowerColor;
        }
        else if (isManipulated)
        {
            // Use yellow for manipulated turbines
            line.material.color = insufficientPowerColor;
        }
        else if (requiredPower > 0 && power < requiredPower)
        {
            line.material.color = insufficientPowerColor;
        }
        else
        {
            line.material.color = powerFlowingColor;
        }
        
        return line;
    }

    public void ForceUpdateAllConnections()
    {
        foreach (var connectionEntry in connectionLines)
        {
            var connection = connectionEntry.Key;
            GameObject from = connection.Item1;
            GameObject to = connection.Item2;
            
            // Default values
            float currentPower = 0;
            float requiredPower = 0;
            bool isManipulated = false;
            
            // Check if "from" is a turbine
            if (from.CompareTag("WindTurbine"))
            {
                Turbine turbine = from.GetComponent<Turbine>();
                if (turbine != null && turbine.isOperational)
                {
                    currentPower = turbine.GetCurrentPower();
                    
                    // Check if this turbine is being manipulated during power generation attack
                    // ONLY set isManipulated for turbine-to-windfarm connections
                    if (cyberAttack != null && cyberAttack.IsTurbineManipulated(turbine) && to.CompareTag("WindFarm"))
                    {
                        isManipulated = true;
                    }
                }
            }
            
            // Update the line
            CreateOrUpdateConnection(from, to, currentPower, requiredPower, isManipulated);
        }
    }
}