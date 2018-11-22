using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameSyncEx;

public class Main : MonoBehaviour {
    IServer ser;
	// Use this for initialization
	void Start () {
        ser = GameServer.GetInstance();
        ser.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        ser.Stop();
    }
}
