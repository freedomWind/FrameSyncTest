using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Game.TempNet
{
    class TcpProtocol : IBaseProtocol
    {
        private BaseProtocolHandler _baseHandler = null;
        private TcpAsyncClient _tcpAsyncClient = null;
        private CircularBuffer<byte[]> _SendList = new CircularBuffer<byte[]>(1000);
        private Boolean _bSendAsync = false;
        private System.Action<bool> _ConnectCallback = null;

        internal TcpProtocol(TcpAsyncClient client)
        {
            try
            {
                _tcpAsyncClient = client;
                _tcpAsyncClient.SetProtocol(this);
            }
            catch
            {
                
            }
        }

        public void Connect(System.Action<bool> callback)
        {
            _ConnectCallback = callback;
            _tcpAsyncClient.Connect();
        }

        public void DisConnect()
        {
            _tcpAsyncClient.Disconnect();
        }

        public void SetProtocolHandler(BaseProtocolHandler handler)
        {
            _baseHandler = handler;
        }

        /// 判断是否已连接  
        public bool Connected
        {
            get { return _tcpAsyncClient != null && _tcpAsyncClient.Connected; }
        }

        public bool SignalConnectResult(bool bResult)
        {
            Console.WriteLine("OnConnectResult: " + (bResult ? "success" : "fail"));
            if (_ConnectCallback != null)
            {
                _ConnectCallback(bResult);
            }

            return true;
        }

        public bool SignalInputBuffer(ref List<byte> data)
        {
            while (data.Count > 4)
            {
                byte[] header = data.GetRange(0, 4).ToArray();
                int packageLen = BitConverter.ToInt32(header, 0);
                if (packageLen <= data.Count - 4)
                {
                    byte[] rev = data.GetRange(4, packageLen).ToArray();
                    data.RemoveRange(0, packageLen + 4);

                    if (_baseHandler != null)
                        _baseHandler.SignalInputBuffer(ref rev);
                }
                else
                {
                    break;
                }
            }

            Console.WriteLine("SignalInputBuffer thread id " + Thread.CurrentThread.ManagedThreadId);
            return true;
        }

        public bool SignalSendCompleted()
        {
            Console.WriteLine("SignalSendCompleted thread id " + Thread.CurrentThread.ManagedThreadId);

            byte[] buf = null;
            lock (_SendList)
            {
                if (_SendList.IsEmpty)
                {
                    _bSendAsync = false;
                    return true;
                }

                buf = _SendList.Front();
                _SendList.PopFront();
            }

            _tcpAsyncClient.Send(ref buf);

            return true;
        }

        public bool Send(string message)
        {
            if (!Connected) return false;

            Console.WriteLine("Send: " + message);
            byte[] buff = Encoding.UTF8.GetBytes(message);

            Send(ref buff);
            return true;
        }

        public bool Send(ref byte[] buff)
        {
            if (!Connected) return false;

            byte[] sendbuff = new byte[buff.Length + 4];
            int len = IPAddress.HostToNetworkOrder(buff.Length);
            Array.Copy(BitConverter.GetBytes(len), sendbuff, 4);
            Array.Copy(buff, 0, sendbuff, 4, buff.Length);

            byte[] buf = null;
            lock(_SendList)
            {
                _SendList.PushBack(sendbuff);
                if (!_bSendAsync)
                {
                    _bSendAsync = true;
                    buf = _SendList.Front();
                    _SendList.PopFront();
                }
            }

            if (buf != null)
            {
                _tcpAsyncClient.Send(ref buf);
            }

            return true;
        }

    }
}