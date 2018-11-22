//using ProtoBuf;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;

namespace Game.TempNet
{/*
    class BaseProtoBufProtocolHandler : BaseProtocolHandler
    {
        internal BaseProtoBufProtocolHandler(TcpAsyncClient client) : base(client)
        {
        }

        public override void SendMsg(int msgno, object obj)
        {
            MemoryStream stream = new MemoryStream();
            Serializer.NonGeneric.Serialize(stream, obj);

            byte[] msgBody = stream.ToArray();
            ushort msgBodySize = (ushort)(msgBody.Length);//Convert.ToInt16(msgBody.Length);
            byte[] msgByte = new byte[4 + msgBodySize];

            Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(msgno)), msgByte, 4);
            Buffer.BlockCopy(msgBody, 0, msgByte, 4, msgBody.Length);

            Send(ref msgByte);
        }

        public override bool SignalInputBuffer(ref byte[] recvBuff)
        {
            int msgID = (int)(recvBuff[0] << 24 | recvBuff[1] << 16 | recvBuff[2] << 8 | recvBuff[3]);

            Handler handler = null;
            _packetHandleDic.TryGetValue(msgID, out handler);
            if (handler == null)
            {
                return false;
            }

            MemoryStream stream = new MemoryStream(recvBuff, 4, recvBuff.Length - 4);
            NetObject stObj = new NetObject();
            stObj.msgID = msgID;
            stObj.obj = Serializer.NonGeneric.Deserialize(handler.t, stream);

            lock (_recvList)
            {
                _recvList.PushBack(stObj);
            }

            return true;
        }
    }
*/}