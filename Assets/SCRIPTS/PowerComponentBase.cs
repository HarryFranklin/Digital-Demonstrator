using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerComponentBase : MonoBehaviour, IPowerComponent
{
    public bool isOperational = true;
    protected float currentPower = 0f;
    public PowerVisualiser visualiser;
    public bool visualisationEnabled = true;
    
    protected virtual void Awake()
    {
        // Find or create visualiser
        if (visualiser == null)
        {
            GameObject visObj = new GameObject("PowerVisualiser");
            visualiser = visObj.AddComponent<PowerVisualiser>();
        }
    }
    
    public virtual float GetCurrentPower()
    {
        return currentPower;
    }
    
    public virtual bool IsOperational()
    {
        return isOperational;
    }
    
    public abstract void VisualiseConnections();

    // Coroutine for updating visualisation every given time period
    protected IEnumerator UpdateVisualisationRoutine(float interval)
    {
        while (true)
        {
            if (visualisationEnabled)
            {
                VisualiseConnections();
            }
            yield return new WaitForSeconds(interval);
        }
    }
}