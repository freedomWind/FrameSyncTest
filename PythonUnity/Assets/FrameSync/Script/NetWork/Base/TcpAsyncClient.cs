using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Game.TempNet
{
    class TcpAsyncClient : IDisposable
    {
        private const Int32 BuffSize = 100*1024;

        private Socket clientSocket;
        private Boolean connected = false;
        private IPEndPoint hostEndPoint;
        private Int32 _iTryNum = 0;
        private byte[] m_recvBuffer;
        private List<byte> m_buffer;
        private SocketAsyncEventArgs receiveEventArgs = new SocketAsyncEventArgs();
        private SocketAsyncEventArgs sendEventArgs = new SocketAsyncEventArgs();
        private IBaseProtocol _protocolObj = null;

        private AutoResetEvent _ConnectEvent;

        /// 当前连接状态  
        public bool Connected { get { return clientSocket != null && clientSocket.Connected; } }

        internal TcpAsyncClient(String ip, Int32 port)
        {
            //   TcpClient
            ip = Dns.GetHostAddresses(ip)[0].ToString();
            hostEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            clientSocket = new Socket(hostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_recvBuffer = new byte[BuffSize];
            m_buffer = new List<byte>();
            _ConnectEvent = new AutoResetEvent(false);
        }

        public void SetProtocol(IBaseProtocol protocolObj)
        {
            _protocolObj = protocolObj;
        }

        /// <summary>  
        /// 连接到主机  
        /// </summary>  
        /// <returns>0.连接成功, 其他值失败,参考SocketError的值列表</returns>  
        internal SocketError Connect()
        {
            SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
            connectArgs.UserToken = clientSocket;
            connectArgs.RemoteEndPoint = hostEndPoint;
            connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);

            clientSocket.ConnectAsync(connectArgs);
            Console.WriteLine("Connect thread id " + Thread.CurrentThread.ManagedThreadId);
            _ConnectEvent.WaitOne();
            return connectArgs.SocketError;
        }

        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        /// Disconnect from the host.  
        internal void Disconnect()
        {
            clientSocket.Disconnect(false);
        }

        /// <summary>  
        /// 初始化收发参数  
        /// </summary>  
        /// <param name="e"></param>  
        private void initArgs(SocketAsyncEventArgs e)
        {
            //发送参数  
            sendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            sendEventArgs.UserToken = clientSocket;

            //接收参数  
            receiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            receiveEventArgs.UserToken = e.UserToken;
            receiveEventArgs.SetBuffer(m_recvBuffer, 0, BuffSize);

            Socket socket = (Socket)e.UserToken;
            //启动接收,不管有没有,一定得启动.否则有数据来了也不知道.  
            if (!socket.ReceiveAsync(receiveEventArgs))
                ProcessReceive(receiveEventArgs);
        }

        // Calback for connect operation  
        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            // Set the flag for socket connected.  
            connected = (e.SocketError == SocketError.Success);

            //如果连接成功,则初始化socketAsyncEventArgs  
            if (connected)
            {
                Console.WriteLine("ProcessConnect thread id " + Thread.CurrentThread.ManagedThreadId);
                _ConnectEvent.Set();
                if (_protocolObj != null)
                    _protocolObj.SignalConnectResult(true);
                initArgs(e);
            }
            else
            {
                if (_iTryNum++ < 3)
                {
                    Connect();
                }
                else
                {
                    _ConnectEvent.Set();
                    if (_protocolObj != null)
                        _protocolObj.SignalConnectResult(false);
                }
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                // check if the remote host closed the connection  
                Socket token = (Socket)e.UserToken;
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    //读取数据  
                    byte[] data = new byte[e.BytesTransferred];
                    Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
                    m_buffer.AddRange(data);

                  // byte[] rev = m_buffer.GetRange(0, m_buffer.Count).ToArray();
                    _protocolObj.SignalInputBuffer(ref m_buffer);
                    //继续接收
                    if (!token.ReceiveAsync(e))
                        this.ProcessReceive(e);
                }
                else
                {
                    ProcessError(e);
                }
            }
            catch (Exception xe)
            {
                Console.WriteLine(xe.Message);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                _protocolObj.SignalSendCompleted();
            }
            else
            {
                ProcessError(e);
            }
        }


        // Close socket in case of failure and throws  
        // a SockeException according to the SocketError.  
        private void ProcessError(SocketAsyncEventArgs e)
        {
            Socket s = (Socket)e.UserToken;
            if (s.Connected)
            {
                // close the socket associated with the client  
                try
                {
                    s.Shutdown(SocketShutdown.Both);
                }
                catch (Exception)
                {
                    // throws if client process has already closed  
                }
                finally
                {
                    if (s.Connected)
                    {
                        s.Close();
                    }
                    connected = false;
                }
            }

            //这里一定要记得把事件移走,如果不移走,当断开服务器后再次连接上,会造成多次事件触发.  
            sendEventArgs.Completed -= IO_Completed;
            receiveEventArgs.Completed -= IO_Completed;
        }

        internal void Send(ref byte[] sendBuffer)
        {
            if (connected)
            {
                sendEventArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);
                clientSocket.SendAsync(sendEventArgs);
            }
            else
            {
                throw new SocketException((Int32)SocketError.NotConnected);
            }
        }

        #region IDisposable Members  
        // Disposes the instance of SocketClient.  
        public void Dispose()
        {
            if (clientSocket.Connected)
            {
                clientSocket.Close();
            }
        }
        #endregion
    }
    

}
