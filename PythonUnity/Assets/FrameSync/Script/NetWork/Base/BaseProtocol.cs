using System;
using System.Collections.Generic;

namespace Game.TempNet
{
    public interface IBaseProtocol
    {
        bool SignalConnectResult(bool bResult);
        bool SignalInputBuffer(ref List<byte> data);
        bool SignalSendCompleted();
    }
}
