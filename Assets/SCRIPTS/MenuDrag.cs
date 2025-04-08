using UnityEngine;

public class MenuDrag : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;

    private Camera mainCamera;
    private ItemRotateOnHover rotateScript;

    void Start()
    {
        mainCamera = Camera.main;
        rotateScript = GetComponent<ItemRotateOnHover>();
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPosition();
            transform.position = mousePos + offset;
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();

        if (rotateScript != null)
        {
            rotateScript.OnDragStart();
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        if (rotateScript != null)
        {
            rotateScript.OnDragEnd();
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = Mathf.Abs(mainCamera.WorldToScreenPoint(transform.position).z);
        return mainCamera.ScreenToWorldPoint(screenMousePos);
    }
}