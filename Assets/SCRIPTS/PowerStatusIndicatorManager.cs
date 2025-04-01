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
    public Sprite recoveredSprite;  // Green icon

    [Header("Power Status Thresholds")]
    [Range(0f, 1f)] public float criticalPowerThreshold = 0.15f;  // 15% of required power
    [Range(0f, 1f)] public float warningPowerThreshold = 0.65f;    // 65% of required power
    
    [Header("Icon Settings")]
    public IconSettings iconSettings = new IconSettings();
    
    [Header("General Settings")]
    public float checkInterval = 0.5f;

    // Reference to Canvas that will hold the icons
    public Canvas overlayCanvas;
    
    private Dictionary<Consumer, GameObject> consumerIcons = new Dictionary<Consumer, GameObject>();

    private void Awake()
    {
        if (overlayCanvas == null)
        {
            overlayCanvas = transform.Find("Canvas")?.GetComponent<Canvas>(); // Find the child canvas if not set
        }

        // Ensure overlayCanvas is assigned
        if (overlayCanvas == null)
        {
            Debug.LogError("No Canvas found for PowerStatusIndicatorManager.");
            return;
        }

        StartCoroutine(CheckAllConsumersPowerStatus());
    }
    
    private IEnumerator CheckAllConsumersPowerStatus()
    {
        while (true)
        {
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
            return;
        }

        float powerRatio = consumer.currentPower / consumer.GetPowerDemand();
        PowerStatus status = GetStatusFromPowerRatio(powerRatio);

        if (!consumerIcons.ContainsKey(consumer) || status != consumerIcons[consumer].GetComponent<IconStatus>().currentStatus)
        {
            UpdateStatusIcon(consumer, status);
        }
    }

    private PowerStatus GetStatusFromPowerRatio(float powerRatio)
    {
        if (powerRatio < criticalPowerThreshold) return PowerStatus.Critical;
        if (powerRatio < warningPowerThreshold) return PowerStatus.Warning;
        return PowerStatus.Normal;
    }

    private void UpdateStatusIcon(Consumer consumer, PowerStatus status)
    {
        // Remove the previous icon if it exists
        RemoveIcon(consumer);

        Sprite iconSprite = GetIconSprite(status);
        if (iconSprite != null)
        {
            GameObject icon = new GameObject($"StatusIcon_{consumer.name}");
            icon.transform.SetParent(overlayCanvas.transform, false);

            var image = icon.AddComponent<UnityEngine.UI.Image>();
            image.sprite = iconSprite;
            image.rectTransform.sizeDelta = new Vector2(iconSettings.size * 100, iconSettings.size * 100);
            image.raycastTarget = false; // Don't need raycast for icons
            
            icon.AddComponent<IconStatus>().currentStatus = status;  // Attach status to manage it

            consumerIcons[consumer] = icon;

            if (status == PowerStatus.Recovered)
            {
                StartCoroutine(RemoveRecoveredIconAfterDelay(consumer, 3f));
            }
        }
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
            case PowerStatus.Recovered: return recoveredSprite;
            default: return null; // No icon for normal state
        }
    }

    private IEnumerator RemoveRecoveredIconAfterDelay(Consumer consumer, float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveIcon(consumer);
    }

    private void LateUpdate()
    {
        foreach (var consumer in consumerIcons)
        {
            if (consumer.Value != null)
            {
                Vector3 worldPos = consumer.Key.transform.position + iconSettings.offset;
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                RectTransform rectTransform = consumer.Value.GetComponent<RectTransform>();
                rectTransform.position = new Vector3(screenPos.x, screenPos.y, 0);
            }
        }
    }

    // Helper class to store the status of each icon
    private class IconStatus : MonoBehaviour
    {
        public PowerStatus currentStatus;
    }

    private enum PowerStatus
    {
        Normal,
        Warning,
        Critical,
        Recovered
    }
}
