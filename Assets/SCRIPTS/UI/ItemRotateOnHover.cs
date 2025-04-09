using UnityEngine;

public class ItemRotateOnHover : MonoBehaviour
{
    private bool isHovered = false;
    private bool isDragging = false;
    private bool hasBeenDragged = false;

    private Quaternion originalRotation;
    private Vector3 originalPosition;

    private Vector3 lastSettledPosition;
    private float returnSpeed = 5f;

    void Start()
    {
        originalRotation = transform.rotation;
        originalPosition = transform.position;
        lastSettledPosition = originalPosition;
    }

    void Update()
    {
        if (isDragging)
            return;

        if (isHovered)
        {
            // Rotate in-place (simulate center rotation)
            transform.rotation *= Quaternion.Euler(0, 50f * Time.deltaTime, 0);
        }
        else
        {
            // Smoothly return to original rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * returnSpeed);

            // Smoothly return to last known settled position (if not dragged, this is the start pos)
            transform.position = Vector3.Lerp(transform.position, lastSettledPosition, Time.deltaTime * returnSpeed);
        }
    }

    public void OnDragStart()
    {
        isDragging = true;
        isHovered = false;

        // Snap to original rotation (but keep position)
        transform.rotation = originalRotation;
    }

    public void OnDragEnd()
    {
        isDragging = false;
        hasBeenDragged = true;

        // Record this as the new "settled" position
        lastSettledPosition = transform.position;
    }

    void OnMouseEnter()
    {
        if (!isDragging)
        {
            isHovered = true;

            if (!hasBeenDragged)
            {
                // Record position before hover starts (if not dragged yet)
                lastSettledPosition = transform.position;
            }
        }
    }

    void OnMouseExit()
    {
        isHovered = false;
    }
}
