using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFrameUpdate
{
    void UpdateDo(float time);
}
public interface IFrameLateUpdate
{
    void LateUpdateDo(float time);
}
public interface IFrameFixUpdate
{
    void FixUpdateDo();
}
