using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Game.TempNet
{
    class BaseJsonProtocolHandler : BaseProtocolHandler
    {
        internal BaseJsonProtocolHandler(TcpAsyncClient client) : base(client)
        {
        }

        public override bool SignalInputBuffer(ref byte[] recvBuff)
        {
            ushort msgID = BitConverter.ToUInt16(recvBuff, 0);
            int st = BitConverter.ToInt32(recvBuff, 2);
            string js = System.Text.Encoding.UTF8.GetString(recvBuff, 6, recvBuff.Length - 6);
            NetObj stObj = new  NetObj(msgID,js,st);
            //UnityEngine.Debug.LogError("recvddd :"+msgID);
            lock (_recvList)
            {
                _recvList.PushBack(stObj);
            }

            return true;
        }

        /// <summary>
        /// !!!单击模式模拟调用
        /// </summary>
        /// <param name="buffer"></param>
        public void ForNoNet(byte[] buffer)
        {
            SignalInputBuffer(ref buffer);
        }
    }
}