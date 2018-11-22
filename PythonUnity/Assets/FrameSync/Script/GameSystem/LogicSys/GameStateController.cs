using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSys
{
    public sealed class GameStateController : IFrameUpdate
    {
        private IGameState state = null;
        static GameStateController _ins;
        public static GameStateController GetIns()
        {
            if (_ins == null)
                _ins = new GameStateController();
            return _ins;
        }
        GameStateController()
        {
            FrameUpdateManager.Instance.AddFrame(this);
        }
        public IGameState Cur { get { return state; } }
        public void StateSwitch(IGameState state)
        {
            if (this.state != null && this.state.gState == state.gState)
                return;
            if (this.state != null)
                this.state.StateOut();
            this.state = state;
            this.state.StateIn();
        }
        void IFrameUpdate.UpdateDo(float time)
        {
            if (state != null)
                state.StateIng();
        }
    }
}
