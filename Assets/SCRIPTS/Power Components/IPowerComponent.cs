using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base interface for all power components
public interface IPowerComponent
{
    float GetCurrentPower();
    bool IsOperational();
    void VisualiseConnections();
}