using System;
using System.Reflection;
using System.Collections.Generic;
using Common;

namespace Game.TempNet
{
    class Common
    {
        public static void Swap<T>(ref T t1, ref T t2)
        {
            T t = t2;
            t2 = t1;
            t1 = t;
        }
    }

    abstract class BaseProtocolHandler : IFrameLateUpdate
    {
        protected TcpProtocol _tcpProtocol;
        protected CircularBuffer<NetObj> _recvList = new CircularBuffer<NetObj>(1000);
        private CircularBuffer<NetObj> _doList = new CircularBuffer<NetObj>(1000);
        protected Dictionary<ushort, Handler> _packetHandleDic = new Dictionary<ushort, Handler>();

        internal BaseProtocolHandler(TcpAsyncClient client)
        {
            load();
            _tcpProtocol = new TcpProtocol(client);
            _tcpProtocol.SetProtocolHandler(this);
            HandlerManager.RegisterProtocolHandler(this);
            FrameUpdateManager.Instance.AddLateFrame(this);
        }

        void load()
        {
            var classes = Assembly.GetExecutingAssembly().GetTypes();
            Dictionary<ushort, Type> opcodeDic = new Dictionary<ushort, Type>();

            foreach (Type t in classes)
            {
                if (!t.FullName.Contains("Game.TempNet.Msg.")) continue;
                object[] attrs = t.GetCustomAttributes(typeof(MessageAttribute), false);
                if (attrs.Length != 0)
                {
                    opcodeDic.Add(((MessageAttribute)attrs[0]).ID, t);               
                }
                else
                {
                    attrs = t.GetCustomAttributes(typeof(OpcodeSyncAttribute), false);
                    if (attrs.Length != 0)
                    {
                        IOpcodeSync op = Activator.CreateInstance(t) as IOpcodeSync;
                        _packetHandleDic.Add(((OpcodeSyncAttribute)attrs[0]).ID, new Handler(op.HandleOpcode));                      
                    }
                }
            }
            foreach (KeyValuePair<ushort, Handler> kv in _packetHandleDic)
            {
                if (!opcodeDic.ContainsKey(kv.Key))
                {
                    UnityEngine.Debug.LogError("A IOpcodeSync don't have a MessageType");
                    continue;
                }
                kv.Value.type = opcodeDic[kv.Key];
            }
    }

        protected void Initialize(System.Action<bool> callback)
        {
            _tcpProtocol.Connect(callback);
        }

        protected void Send(ref byte[] msgByte)
        {
            _tcpProtocol.Send(ref msgByte);
        }

        public void DoRecvMsg()
        {
            lock(_recvList)
            {
                Common.Swap<CircularBuffer<NetObj>>(ref _recvList, ref _doList);
            }

            NetObj netObj;
            while (!_doList.IsEmpty)
            {
                netObj = _doList.Front();
                _doList.PopFront();
                Handler handler = null;
                _packetHandleDic.TryGetValue(netObj.id, out handler);
                if (handler == null)
                {
                    UnityEngine.Debug.Log("handler null!!");
                    continue;
                }
               
                HandlerMsg(handler,netObj);
            }
        }
        public void DoDelayHandler()
        {
            if (delayQue.Count == 0) return;
            for (int i = 0; i < delayQue.Count; i++)
            {
                DelayAction d = delayQue[i];
                if (d.delayT <= 20)
                {
                    d.action(d.msg);
                    delayQue.Remove(d);
                }
                d.delayT -= 20;
            }
            // delayQue.RemoveAll(DoAction);    //遍历的顺序不一致会有bug
        }

        bool DoAction(DelayAction action)
        {
            if (action.delayT <= 20)
            {
                action.action(action.msg);
                return true;
            }
            action.delayT -= 20; 
            return false;
        }
        //计算时间间隔
        //丢到一个驱动队列，放fixupdate里面跑
        void HandlerMsg(Handler handler, NetObj o)
        {
            handler.HandlerMsg(o.msg);
            #region  延迟执行模式
            /*
            int t1 = System.DateTime.UtcNow.Second;
            int t2 = System.DateTime.UtcNow.Millisecond;
            int t11 = o.beginTime / 1000;
            int t22 = o.beginTime % 1000;
            int span = 0;
            if (t11 == t1)
                span = t2 - t22;
            else
                span = 1000 + t2 - t22;
            DelayAction dd = new DelayAction(o.msg, 200 - span, handler.HandlerMsg);
            delayQue.Add(dd);
            */
            #endregion  
        }
        public void DisConnect()
        {
            _tcpProtocol.DisConnect();
        }
       
        List<DelayAction> delayQue = new List<DelayAction>(50);
        public bool Connected { get { return _tcpProtocol.Connected; } }

     //   public abstract void SendMsg(int msgno, object obj);
        public abstract bool SignalInputBuffer(ref byte[] recvBuff);

        void IFrameLateUpdate.LateUpdateDo(float time)
        {
            DoRecvMsg();     
        }
    }
    class DelayAction
    {
        public string msg;
        public int delayT;
        public System.Action<string> action;
        public DelayAction(string m, int d, System.Action<string> a)
        {
            msg = m; delayT = d; action = a;
        }
    }
}