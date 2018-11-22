using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameUpdateManager : MonoBehaviour
{
    private List<IFrameUpdate> _list;
    private List<IFrameLateUpdate> _llist;
    private List<IFrameFixUpdate> _lllist;
    private static FrameUpdateManager _ins;
    public static FrameUpdateManager Instance
    {
        get
        {
            try
            {
                if (_ins == null)
                {
                  //  _ins = AppFacade.Instance.GetComponent<FrameUpdateManager>();
                }
                return _ins;
            }
            catch
            {
                return null;
            }
        }
    }
    private FrameUpdateManager()
    {
        _list = new List<IFrameUpdate>();
        _llist = new List<IFrameLateUpdate>();
        _lllist = new List<IFrameFixUpdate>();
    }
    public void AddFrame(IFrameUpdate frame)
    {
        _list.Add(frame);
    }
    public void AddLateFrame(IFrameLateUpdate frame)
    {
        _llist.Add(frame);
    }
    public void AddFixFrmae(IFrameFixUpdate frame)
    {
        _lllist.Add(frame);
    }
    public void RemoveFrame(IFrameUpdate frame)
    {
        _list.Remove(frame);
    }
    public void RemoveFrame(IFrameLateUpdate frame)
    {
        _llist.Remove(frame);
    }
    public void RemoveFrame(IFrameFixUpdate frame)
    {
        _lllist.Remove(frame);
    }
    void Update()
    {
        for (int i = 0; i < _list.Count; i++)
        {
            _list[i].UpdateDo(Time.deltaTime);
        }
    }
    private void LateUpdate()
    {
        for (int i = 0; i < _llist.Count; i++)
        {
            _llist[i].LateUpdateDo(Time.deltaTime);
        }
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < _lllist.Count; i++)
        {
            _lllist[i].FixUpdateDo();
        }
    }
    private void OnDestroy()
    {
        _ins = null;
    }
}