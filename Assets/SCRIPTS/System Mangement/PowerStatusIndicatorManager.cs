using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerStatusIndicatorManager : MonoBehaviour
{
    [System.Serializable]
    public class IconSettings
    {
        public Vector3 offset = new Vector3(0, 1.5f, 0);
        public float size = 0.5f;
    }

    [Header("Consumers to Monitor")]
    public List<Consumer> consumers = new List<Consumer>();
    
    [Header("Icon Sprites")]
    public Sprite warningSprite;    // Yellow "!" icon
    public Sprite criticalSprite;   // Red icon
    public Sprite goodSprite;       // Green icon

    [Header("Power Status Thresholds")]
    [Range(0f, 1f)] public float criticalPowerThreshold = 0.25f;  // 0-24% of required power
    [Range(0f, 1f)] public float warningPowerThreshold = 0.85f;    // 25-84% of required power
    
    [Header("Icon Settings")]
    public IconSettings iconSettings = new IconSettings();
    
    [Header("Animation Settings")]
    public float goodIconDuration = 1.5f;    // How long the green icon stays
    public float fadeOutDuration = 1.5f;   // How long it takes to fade out
    public float slideDistance = 75f;      // How far the icon slides up
    
    [Header("General Settings")]
    public float checkInterval = 0.25f;
    public bool debugStatusChanges = true;

    // Reference to Canvas that will hold the icons
    public Canvas overlayCanvas;
    
    private Dictionary<Consumer, GameObject> consumerIcons = new Dictionary<Consumer, GameObject>();
    private Dictionary<Consumer, PowerStatus> lastStatus = new Dictionary<Consumer, PowerStatus>();

    private void Awake()
    {
        // Register with existing consumers
        foreach (var consumer in consumers)
        {
            if (consumer != null)
            {
                RegisterConsumer(consumer);
            }
        }
        
        StartCoroutine(CheckAllConsumersPowerStatus());
    }
    
    private void RegisterConsumer(Consumer consumer)
    {
        // Subscribe to power status changes
        consumer.OnPowerStatusChanged += (ratio) => OnConsumerPowerChanged(consumer, ratio);
        
        // Initialize status
        UpdateConsumerStatus(consumer);
    }
    
    private void OnConsumerPowerChanged(Consumer consumer, float powerRatio)
    {
        if (debugStatusChanges)
        {
            Debug.Log($"Power changed for {consumer.gameObject.name}: {powerRatio:P0} of required power");
        }
        
        UpdateConsumerStatus(consumer);
    }
    
    private IEnumerator CheckAllConsumersPowerStatus()
    {
        while (true)
        {
            // This is now a backup check in case the event system missed something
            foreach (var consumer in consumers)
            {
                if (consumer != null)
                {
                    UpdateConsumerStatus(consumer);
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void UpdateConsumerStatus(Consumer consumer)
    {
        if (!consumer.isOperational) 
        {
            RemoveIcon(consumer);
            lastStatus.Remove(consumer);
            return;
        }

        float powerRatio = consumer.currentPower / consumer.GetPowerDemand();
        PowerStatus currentStatus = GetStatusFromPowerRatio(powerRatio);
        
        // Check if status has changed
        if (!lastStatus.ContainsKey(consumer))
        {
            // First time seeing this consumer
            lastStatus[consumer] = currentStatus;
            UpdateStatusIcon(consumer, currentStatus);
        }
        else if (lastStatus[consumer] != currentStatus)
        {
            // Status has changed
            PowerStatus previousStatus = lastStatus[consumer];
            lastStatus[consumer] = currentStatus;
            
            if (debugStatusChanges)
            {
                Debug.Log($"Status changed for {consumer.gameObject.name}: {previousStatus} -> {currentStatus}");
            }
            
            // If we're changing to "Good" from a worse state, show green recovery icon
            if (currentStatus == PowerStatus.Good && 
                (previousStatus == PowerStatus.Warning || previousStatus == PowerStatus.Critical))
            {
                UpdateStatusIcon(consumer, PowerStatus.Good, true);
            }
            else
            {
                UpdateStatusIcon(consumer, currentStatus);
            }
        }
    }

    private PowerStatus GetStatusFromPowerRatio(float powerRatio)
    {
        if (powerRatio < criticalPowerThreshold) return PowerStatus.Critical;
        if (powerRatio < warningPowerThreshold) return PowerStatus.Warning;
        return PowerStatus.Good;
    }

    private void UpdateStatusIcon(Consumer consumer, PowerStatus status, bool animate = false)
    {
        // Remove the previous icon if it exists
        RemoveIcon(consumer);

        Sprite iconSprite = GetIconSprite(status);
        if (iconSprite != null)
        {
            // Create a unique name for this icon
            string iconName = $"StatusIcon_{consumer.gameObject.name}_{consumer.GetInstanceID()}";
            GameObject icon = new GameObject(iconName);
            icon.transform.SetParent(overlayCanvas.transform, false);

            var image = icon.AddComponent<UnityEngine.UI.Image>();
            image.sprite = iconSprite;
            image.rectTransform.sizeDelta = new Vector2(iconSettings.size * 100, iconSettings.size * 100);
            image.raycastTarget = false; // Don't need raycast for icons
            
            var iconStatus = icon.AddComponent<IconStatus>();
            iconStatus.currentStatus = status;

            consumerIcons[consumer] = icon;

            // If this is a "Good" status and we want to animate it
            if (status == PowerStatus.Good && animate)
            {
                StartCoroutine(AnimateGoodIcon(consumer, icon));
            }
            // If it's a "Good" status but not animated, just show temporarily
            else if (status == PowerStatus.Good)
            {
                StartCoroutine(RemoveIconAfterDelay(consumer, goodIconDuration));
            }
        }
    }

    private IEnumerator AnimateGoodIcon(Consumer consumer, GameObject icon)
    {
        // Wait for the display duration
        yield return new WaitForSeconds(goodIconDuration);
        
        // Get components for animation
        var image = icon.GetComponent<UnityEngine.UI.Image>();
        var rectTransform = icon.GetComponent<RectTransform>();
        Vector3 startPosition = rectTransform.position;
        Vector3 endPosition = startPosition + new Vector3(0, slideDistance, 0);
        Color startColor = image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        
        // Animate fade out and slide up
        float elapsedTime = 0;
        while (elapsedTime < fadeOutDuration)
        {
            float t = elapsedTime / fadeOutDuration;
            
            // Lerp color and position
            image.color = Color.Lerp(startColor, endColor, t);
            rectTransform.position = Vector3.Lerp(startPosition, endPosition, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure we reach the final state
        image.color = endColor;
        rectTransform.position = endPosition;
        
        // Remove the icon
        RemoveIcon(consumer);
    }

    private void RemoveIcon(Consumer consumer)
    {
        if (consumerIcons.ContainsKey(consumer))
        {
            Destroy(consumerIcons[consumer]);
            consumerIcons.Remove(consumer);
        }
    }

    private Sprite GetIconSprite(PowerStatus status)
    {
        switch (status)
        {
            case PowerStatus.Warning: return warningSprite;
            case PowerStatus.Critical: return criticalSprite;
            case PowerStatus.Good: return goodSprite;
            default: return null;
        }
    }

    private IEnumerator RemoveIconAfterDelay(Consumer consumer, float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveIcon(consumer);
    }

    private void LateUpdate()
    {
        foreach (var pair in consumerIcons)
        {
            Consumer consumer = pair.Key;
            GameObject icon = pair.Value;
            
            if (consumer != null && icon != null)
            {
                Vector3 worldPos = consumer.transform.position + iconSettings.offset;
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                RectTransform rectTransform = icon.GetComponent<RectTransform>();
                rectTransform.position = new Vector3(screenPos.x, screenPos.y, 0);
            }
        }
    }

    // Public method to force refresh all icons - used for attacks
    public void ForceRefreshAllIcons()
    {
        // Clear all existing icons
        foreach (var pair in consumerIcons)
        {
            if (pair.Value != null)
            {
                Destroy(pair.Value);
            }
        }
        consumerIcons.Clear();
        
        // Reset last status cache to force re-evaluation
        lastStatus.Clear();
        
        // Re-evaluate all consumers
        foreach (var consumer in consumers)
        {
            if (consumer != null && consumer.isOperational)
            {
                float powerRatio = consumer.currentPower / consumer.GetPowerDemand();
                PowerStatus status = GetStatusFromPowerRatio(powerRatio);
                UpdateStatusIcon(consumer, status);
                lastStatus[consumer] = status;
                
                if (debugStatusChanges)
                {
                    Debug.Log($"Force refreshed icon for {consumer.gameObject.name}: {status}");
                }
            }
        }
        
        Debug.Log("All power status icons have been forcibly refreshed");
    }

    public void RestartMonitoring() // Fixes issue with monitoring attack breaking the power status indicator
    {
        // Clear any existing state
        foreach (var consumer in consumers)
        {
            if (consumerIcons.ContainsKey(consumer))
            {
                Destroy(consumerIcons[consumer]);
            }
        }
        consumerIcons.Clear();
        lastStatus.Clear();
        
        // Restart the monitoring coroutine
        StopAllCoroutines();
        StartCoroutine(CheckAllConsumersPowerStatus());
    }

    // Helper class to store the status of each icon
    private class IconStatus : MonoBehaviour
    {
        public PowerStatus currentStatus;
    }

    private enum PowerStatus
    {
        Good,
        Warning,
        Critical
    }
}