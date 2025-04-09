using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum AccordionDirection { Up, Down }
public enum AccordionResourceType {Toggle, Button }

public class AccordionButton : MonoBehaviour
{
    [Header("Accordion Settings")]
    public AccordionDirection expandDirection = AccordionDirection.Up;
    public bool openOnHover = false;
    public bool onlyOneOpen = true;
    public int numberOfItems = 3;
    public AccordionResourceType[] resourceTypes;

    [Header("UI Elements")]
    public RectTransform contentContainer;
    public GameObject togglePrefab;
    public GameObject buttonPrefab;
    public TextMeshProUGUI label;
    public float itemSpacing = 30f;
    public float expandSpeed = 5f;

    [Header("Accordion References")]
    public List<AccordionButton> allAccordions;

    private bool isOpen = false;
    private float targetHeight;
    private float originalY;

    private void Start()
    {
        originalY = ((RectTransform)transform).anchoredPosition.y;
        SetupContent();
        CollapseImmediate();
    }

    private void SetupContent()
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < numberOfItems; i++)
        {
            GameObject item;
            if (resourceTypes.Length > i && resourceTypes[i] == AccordionResourceType.Toggle)
            {
                item = Instantiate(togglePrefab, contentContainer);
            }
            else
            {
                item = Instantiate(buttonPrefab, contentContainer);
            }

            item.GetComponentInChildren<TextMeshProUGUI>().text = "Item " + (i + 1);
        }

        float totalHeight = numberOfItems * itemSpacing;
        contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, totalHeight);
    }

    private void Update()
    {
        if (isOpen)
        {
            contentContainer.sizeDelta = Vector2.Lerp(contentContainer.sizeDelta, new Vector2(contentContainer.sizeDelta.x, targetHeight), Time.deltaTime * expandSpeed);
        }
        else
        {
            contentContainer.sizeDelta = Vector2.Lerp(contentContainer.sizeDelta, new Vector2(contentContainer.sizeDelta.x, 0), Time.deltaTime * expandSpeed);
        }

        AdjustStackedPositions();
    }

    public void ToggleAccordion()
    {
        if (onlyOneOpen)
        {
            foreach (var acc in allAccordions)
            {
                if (acc != this)
                    acc.CloseAccordion();
            }
        }

        isOpen = !isOpen;
    }

    public void CloseAccordion() => isOpen = false;
    public void OpenAccordion() => isOpen = true;

    public void CollapseImmediate()
    {
        isOpen = false;
        contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, 0);
    }

    private void OnMouseEnter()
    {
        if (openOnHover)
            ToggleAccordion();
    }

    private void AdjustStackedPositions()
    {
        if (expandDirection == AccordionDirection.Up)
        {
            float offset = isOpen ? targetHeight : 0;
            foreach (var acc in allAccordions)
            {
                if (acc == this) continue;
                if (((RectTransform)acc.transform).anchoredPosition.y > originalY)
                {
                    Vector2 pos = ((RectTransform)acc.transform).anchoredPosition;
                    pos.y = originalY + offset + 50f; // 50f is spacing between stacked buttons
                    ((RectTransform)acc.transform).anchoredPosition = Vector2.Lerp(((RectTransform)acc.transform).anchoredPosition, pos, Time.deltaTime * expandSpeed);
                }
            }
        }

        // Add future logic for Down expansion if needed
    }
}