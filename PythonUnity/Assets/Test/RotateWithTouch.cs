using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 更随鼠标旋转
/// </summary>
public class RotateWithTouch : MonoBehaviour
{
    public System.Action<GameObject> OnRotating;
    public System.Action OnBeginDrag;
    public System.Action OnEndDrag;
    protected Vector3 world;  //鼠标的位置
    private void Awake()
    {
        isDragging = false;
    }
    bool isDragging;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            if (OnBeginDrag != null)
                OnBeginDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (OnEndDrag != null)
                OnEndDrag();
            isDragging = false;
        }
        if (isDragging)
        {
            Vector3 mouseposition = Input.mousePosition;
            Vector3 targetposition = Camera.main.WorldToScreenPoint(transform.position);
            mouseposition.z = targetposition.z;
            world = Camera.main.ScreenToWorldPoint(mouseposition);
            world.z = Camera.main.ScreenToWorldPoint(mouseposition).z;
            world.y = this.transform.position.y;
            transform.LookAt(world);
            if (OnRotating != null)
                OnRotating(gameObject);
        }
    }
}
