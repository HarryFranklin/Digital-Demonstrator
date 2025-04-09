using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindFarm : PowerComponentBase
{
    // I/O
    public Inverter outputInverter;
    
    private Dictionary<Turbine, float> turbinePower = new Dictionary<Turbine, float>();
    private List<Turbine> connectedTurbines = new List<Turbine>();
    
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(UpdateVisualisationRoutine(0.1f));
        StartCoroutine(UpdatePositionRoutine());
    }
    
    void Update()
    {
        // Sum up power from all turbines
        currentPower = 0f;
        foreach (var power in turbinePower.Values)
        {
            currentPower += power;
        }
        
        // Send power to inverter
        if (outputInverter != null && isOperational)
        {
            outputInverter.ReceivePower(currentPower);
        }
    }
    
    // Receive power from a turbine
    public void ReceivePower(Turbine turbine, float power)
    {
        if (!connectedTurbines.Contains(turbine))
        {
            connectedTurbines.Add(turbine);
        }
        
        turbinePower[turbine] = power;
    }
    
    public override void VisualiseConnections()
    {
        if (visualiser == null) return;
        
        // Visualise connections from turbines to wind farm
        foreach (var turbine in connectedTurbines)
        {
            if (turbine != null)
            {
                visualiser.CreateOrUpdateConnection(turbine.gameObject, gameObject, turbinePower.ContainsKey(turbine) ? turbinePower[turbine] : 0);
            }
        }
        
        // Visualise connection to inverter
        if (outputInverter != null)
        {
            visualiser.CreateOrUpdateConnection(gameObject, outputInverter.gameObject, currentPower);
        }
    }
    
    // Coroutine to update position to centre of turbines
    private IEnumerator UpdatePositionRoutine()
    {
        while (true)
        {
            UpdatePosition();
            yield return new WaitForSeconds(0.5f); // Run UpdatePosition() every given time
        }
    }
    
    private void UpdatePosition()
    {
        if (connectedTurbines.Count == 0) return;
        
        Vector3 centre = Vector3.zero;
        int count = 0;
        
        foreach (var turbine in connectedTurbines)
        {
            if (turbine != null)
            {
                centre += turbine.transform.position;
                count++;
            }
        }
        
        if (count > 0)
        {
            transform.position = centre / count;
        }
    }
}