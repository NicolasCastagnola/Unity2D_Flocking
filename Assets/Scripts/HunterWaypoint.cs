using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterWaypoint : MonoBehaviour
{
    private Camera _mainCamera;
    private void Awake() => _mainCamera = Camera.main;
    private bool isDragging = false;
    private void OnMouseDrag()
    {
        isDragging = true;

        var mousePosition = Input.mousePosition;
        var worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
        var transform1 = transform;
        transform1.position = new Vector3(worldPosition.x, worldPosition.y, transform1.position.z);
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void Update()
    {
        if (!isDragging) return;
            
        var mousePosition = Input.mousePosition;
        var worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
        var transform1 = transform;
        transform1.position = new Vector3(worldPosition.x, worldPosition.y, transform1.position.z);
    }
}
