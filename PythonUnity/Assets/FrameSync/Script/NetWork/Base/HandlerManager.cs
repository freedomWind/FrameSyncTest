using System.Collections.Generic;

namespace Game.TempNet
{
    class HandlerManager
    {
        static List<BaseProtocolHandler> _lstHandlers = new List<BaseProtocolHandler>();

        public static void RegisterProtocolHandler(BaseProtocolHandler protocolHandler)
        {
            _lstHandlers.Add(protocolHandler);
        }

        public static void RemoveProtocolHandler(BaseProtocolHandler protocolHandler)
        {
            _lstHandlers.Remove(protocolHandler);
        }

        public static void DoAllRecvMsgs()
        {
            foreach (BaseProtocolHandler protcolHandler in _lstHandlers)
            {
                protcolHandler.DoRecvMsg();
            }
        }

        public static void Disconnect()
        {
            foreach (BaseProtocolHandler protcolHandler in _lstHandlers)
            {
                protcolHandler.DisConnect();
            }
        }
    }

}