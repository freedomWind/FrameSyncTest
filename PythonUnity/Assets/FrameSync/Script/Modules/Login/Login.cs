using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.TempNet;
using Common;

public class Login : MonoBehaviour {

	// Use this for initialization
	void Start () {
        NetMgr.NetModel = NetModel.PVP;
        DoLogin();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DoLogin()
    {
        NetMgr.SetIp("10.22.12.32");
        NetMgr.Connect(result => {
            Debug.LogError("result :"+result);
        });
    }

    private void OnDestroy()
    {
        NetMgr.getIns().DisConnect();
    }
}
