using UnityEngine;

public class MenuDrag : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    private bool isDragging = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseEnter()
    {
        // Trigger rotation on hover
        ItemRotateOnHover rotateScript = GetComponent<ItemRotateOnHover>();
        if (rotateScript != null)
        {
            rotateScript.enabled = true;
        }
    }

    void OnMouseExit()
    {
        // Stop rotation on exit
        ItemRotateOnHover rotateScript = GetComponent<ItemRotateOnHover>();
        if (rotateScript != null)
        {
            rotateScript.enabled = false;
        }
    }

    void OnMouseDown()
    {
        // Start dragging
        isDragging = true;
        // Calculate the offset between the mouse position and the object position
        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseUp()
    {
        // Stop dragging
        isDragging = false;
    }

    void Update()
    {
        if (isDragging)
        {
            // Update the object's position based on mouse position
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        // Convert the mouse position to world coordinates
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
