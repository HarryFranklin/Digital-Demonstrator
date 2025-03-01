using UnityEngine;

public class Battery : MonoBehaviour
{    
    /**
    Turbine class is the source of the power system.  One or many turbines make up a wind farm.
    One or many wind farms send power to an inverter, then to a transformer.
    If power demand is too low, excess power is sent from the transformer to a battery, then to the grid as required.
    If demand is sufficient, power is sent from transformer to the grid.
    If the source power is too low, the battery is discharged to make up the difference.
    Power is sent from the grid to the consumer.

    Current stage: BATTERY
    Next stage: GRID
    **/

    // I/O
    public Transformer inputTransformer; // One transformer per battery (One transformer can serve multiple batteries)
    public PowerGrid outputGrid; // One grid per battery.
    // I/O

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}