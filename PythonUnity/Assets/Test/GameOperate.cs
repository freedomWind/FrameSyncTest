using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 对物体的操作
/// </summary>
public class GameOperate : MonoBehaviour
{
    [Header("绑定你要操作的物体")]
    public GameObject Target;
    [Header("移动快慢控制")]
    [Range(0,10)]
    public float qMove;
    [Header("缩放快慢控制")]
    [Range(0,10)]
    public float qScale;
    [Header("旋转快慢控制")]
    [Range(0,10)]
    public float qRotate;
    System.Action curOperate;
    private void Update()
    {
        curOperate = null;
        if (Input.GetMouseButton(0))
        {
            curOperate = DoMove;
        }
        else if (Input.GetMouseButton(1))
        {
            curOperate = DoRotate;
        }
        else if(Input.GetAxis("Mouse ScrollWheel")!=0)
        {
            curOperate = DoScale;
        }
        if (curOperate != null)
            curOperate();
    }
    void DoScale()
    {
        Debug.Log("正在缩放");
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if (Target != null)
        {
            //设置阙值
            Target.transform.localScale += Vector3.one * zoom * qScale;
        }
        lastOperate = OperatType.scale;
    }
    Vector3 origMousePos;
    void DoRotate()
    {
        Debug.Log("正在旋转");
        if (Target != null)
        {
            //把对应的鼠标位置改变映射成要旋转的量
            //Target.transform.localEulerAngles += Vector3.one * 0.5f;  
        }
        lastOperate = OperatType.rotate;
        lastMousePosition = Input.mousePosition;
    }
    void DoMove()
    {
        Debug.Log("正在移动");
        if (Target != null)
        {
            //Time.deltaTime*(Input.mousePosition - origMousePos)*
            //对应鼠标位置改变映射成要平移的量
            //Target.transform.localPosition += Vector3.one * 0.05f;
        }
        lastOperate = OperatType.move;
        lastMousePosition = Input.mousePosition;
    }

    OperatType lastOperate;  //上一次操作
    Vector3 lastMousePosition;

    enum OperatType
    {
        scale,
        rotate,
        move,
        none,
    }
}
