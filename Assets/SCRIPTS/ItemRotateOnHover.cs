using UnityEngine;

public class ItemRotateOnHover : MonoBehaviour
{
    private bool isHovered = false;

    void Update()
    {
        // Rotate around the Y-axis when hovered
        if (isHovered)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 50f); // Rotate at 50 degrees per second
        }
    }

    // Start rotating when mouse enters the item
    void OnMouseEnter()
    {
        isHovered = true;
    }

    // Stop rotating when mouse leaves the item
    void OnMouseExit()
    {
        isHovered = false;
    }
}
