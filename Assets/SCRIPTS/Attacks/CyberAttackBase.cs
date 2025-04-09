using System.Collections;
using UnityEngine;

public abstract class CyberAttackBase : MonoBehaviour
{
    protected PowerVisualiser powerVisualiser;
    protected PowerSystemManager powerSystemManager;
    protected bool isActive = false;
    
    // Name of the attack, to be overridden by derived classes
    public abstract string AttackName { get; }
    
    public virtual void Initialise(PowerVisualiser visualiser, PowerSystemManager systemManager)
    {
        powerVisualiser = visualiser;
        powerSystemManager = systemManager;
    }
    
    public virtual bool IsActive()
    {
        return isActive;
    }
    
    public virtual void ToggleAttack()
    {
        isActive = !isActive;
        
        if (isActive)
        {
            StartAttack();
        }
        else
        {
            StopAttack();
        }
    }
    
    protected abstract void StartAttack();
    protected abstract void StopAttack();
    
    protected void OnDestroy()
    {
        // Ensure attack is stopped when object is destroyed
        if (isActive)
        {
            StopAttack();
        }
    }
}