using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System;

public class AppFacade : MonoBehaviour
{
    static AppFacade _ins;
    public static AppFacade Instance
    {
        get
        {
            return _ins;
        }
    }

    private void Awake()
    {

        _ins = this;
        transform.AddComponentIfNoExist<FrameUpdateManager>();
        GameObject.DontDestroyOnLoad(this);        
    }

    private void Start()
    { 
    }
}

