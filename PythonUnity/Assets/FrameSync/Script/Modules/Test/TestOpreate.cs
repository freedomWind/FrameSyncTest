using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameSync;

public class TestOpreate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (!FGame.Instance.isReady)
            return;
        if (Input.anyKey)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    CommandSystem.GetInstance().NewCommand(CommandDefine.testKeyPress, keyCode);
                }
            }

        }
    }
}
