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

        public static bool onPhoton;
        public static int account;
        public static string password;

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
            if(!NetMgr.onPhoton)
            {
                NetMgr.port = 13909;
            }
            else
            {
                NetMgr.port = 5055;
            }
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
                if (!onPhoton)
                {
                    ins.Initialize(result);
                }
            }
        }
        static int reconnectTimes = 0;
        public static void ReConnect(System.Action<bool> result)
        {
            //reconnectTimes++;
            //if (reconnectTimes == 5)
            //    result(false);
            //if(onPhoton)
            //{
            //    if(reconnectTimes >= 5)
            //    {
            //        return;
            //    }
            //    if(!PhotonNetwork.connected)
            //    {
            //        ConnectAndJoinRandom.Instance.PhotonReconnect();
            //    }
            //    result(true);
            //    return;
            //}
            //ins.DisConnect();
            //ins._tcpProtocol.Connect(_ => {
            //    if (_) result(true);
            //    else  //失败则间歇重连
            //    {
            //        AppFacade.Instance.DelayAction(5, () => {
            //            ReConnect(result);
            //        });
            //    }
            //});
        }
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
            byte[] msgByte = new byte[6 + msgBodySize];
            //Debug.Log("发送的数据包大小：" + msgBody.Length + " sent time:" + sendTime);
            Array.Copy(BitConverter.GetBytes(id), msgByte, 2);
            Array.Copy(BitConverter.GetBytes(sendTime), 0, msgByte, 2, 4);
            Buffer.BlockCopy(msgBody, 0, msgByte, 6, msgBody.Length);
            if (NetMgr.NetModel == NetModel.PVP)   //有网络
            {
                //NetworkDelay.GetInstance().OnSendMsg(msg._serialid);
                if(!onPhoton)
                {
                    Send(ref msgByte);
                }
                //else
                //{
                //    int type = id / 1000;
                //    if (type == 2)
                //    {
                //        var punEvent = new Hashtable();
                //        punEvent.Add((byte)245, msgByte);
                //        PhotonNetwork.networkingPeer.OpRaiseEvent(PunEvent.JDSync, punEvent, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All });
                //    }
                //    else
                //    {
                //        Dictionary<byte, object> op = new Dictionary<byte, object>();
                //        op[ParameterCode.JDNetData] = msgByte;
                //        PhotonNetwork.networkingPeer.OpCustom(OperationCode.JDNet, op, true);
                //    }
                //}
            }
            else  //没网络
            {
                ForNoNet(msgByte);
            }
        }
    }
}
