using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.TempNet
{
    public enum NetModel
    {
        PVP,
        PVE,
        NONE,
    }

    class NetMgr : BaseJsonProtocolHandler
    {
        public static int selfid;
        private static NetMgr ins;

        public static NetMgr getIns()
        {
            if (ins == null)
            {
                if (NetModel != NetModel.PVP)
                {
                    ins = new NetMgr(null);
                }
            }
            return ins;
        }
        public static void SetIp(string ip)
        {
            NetMgr.ip = ip;
            NetMgr.port = 13909;
        }
        private static NetModel _model = NetModel.NONE;
        public static NetModel NetModel
        {
            set { if (NetModel.NONE == _model) _model = value; }
            get { return _model; }
        }
        public NetMgr(TcpAsyncClient client) : base(client)
        { }
        static string ip;
        static int port;
        public static void Connect(System.Action<bool> result)
        {
            Connect(ip, port, result);
        }
        public static void Connect(string ip, int port, System.Action<bool> result)
        {
            if (ins == null)
            {
                NetMgr.ip = ip;
                NetMgr.port = port;
                ins = new NetMgr(new TcpAsyncClient(ip, port));
                // 这connect是阻塞的，会卡很久
                ins.Initialize(result);
            }
        }
        static int reconnectTimes = 0;
       
        public void SyncMsg(IMessage msg, Action<IMessage> callback = null)
        {
            ushort id = msg.ID;
            if (id == 0)
            {
                Debug.LogError("error msg:" + msg.GetType());
                return;
            }
            if (callback != null)
            {
                _packetHandleDic[id].selfCallback = callback;
            }
            int sendTime = System.DateTime.UtcNow.Second * 1000 + System.DateTime.UtcNow.Millisecond;
            byte[] msgBody = System.Text.Encoding.UTF8.GetBytes(msg.ToString());
            ushort msgBodySize = (ushort)(msgBody.Length);
            byte[] msgByte = new byte[msgBodySize];
            Buffer.BlockCopy(msgBody, 0, msgByte, 0, msgBody.Length);
            if (NetMgr.NetModel == NetModel.PVP)   //有网络
            {
                Send(ref msgByte);
            }
            else  //没网络
            {
                ForNoNet(msgByte);
            }
        }
    }
}
