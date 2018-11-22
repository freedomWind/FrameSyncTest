using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace Game.TempNet
{
    
    public struct NetObj
    {
        public ushort id;
        public int beginTime;
        public string msg;
        public NetObj(ushort id, string js,int time)
        {
            this.id = id;
            msg = js;
            beginTime = time;
        }
    }

    public abstract class IMessage
    {
        //[SerializeField]
        public int _playerid;
        public int playerid { get { return _playerid; } }
        public bool isSelf
        {
            get
            {
                return (_playerid == NetMgr.selfid);
            }
        }
        public sealed override string ToString()
        {
            _playerid = NetMgr.selfid;
            return JsonConvert.SerializeObject(this);//JsonUtility.ToJson(this);
        }
        public ushort ID
        {
            get
            {
                object[] oo = this.GetType().GetCustomAttributes(typeof(MessageAttribute), false);
                if (oo.Length == 0) { Debug.LogError("sendRspMsg error"); return 0; }
                return ((MessageAttribute)oo[0]).ID;
            }
        }
    }

    /// <summary>
    /// 同步过来的指令
    /// </summary>
    public interface IOpcodeSync
    {
        void HandleOpcode(IMessage msg);
    }
    
    public class Handler
    {
        public Type type;
        public Action<IMessage> selfCallback;
        public Action<IMessage> syncCallback;
        public Handler()
        {
        }
        public Handler(Action<IMessage> syncAction)
        {
            this.syncCallback = syncAction;
            this.selfCallback = syncAction;
        }
        public void HandlerMsg(string m)
        {
            IMessage msg = (IMessage)FromJson(m);         
            if (msg.isSelf && selfCallback != null)
                selfCallback(msg);
            else if (!msg.isSelf && syncCallback != null)
                syncCallback(msg);
        }
        public IMessage FromJson(string json)
        {
            return JsonConvert.DeserializeObject(json, type) as IMessage;//JsonUtility.FromJson(json,type) as IMessage;
        }
    }
}