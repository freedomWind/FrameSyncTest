using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace GameSys
{
    public class GameLogic : IGameSystem
    {
        List<IGameState> gamestateList = new List<IGameState>();
        public GameLogic()
        {
            //gamestateList.Add(new GameSelect());
            //gamestateList.Add(new GameLoading());
            //gamestateList.Add(new GameReady());
            //gamestateList.Add(new GamePlaying());
            //gamestateList.Add(new GameOver());
        }
        public void ChangeGameState(gState s)
        {
            IGameState ss = GetGameState(s);
            if (ss != null)
                IGameState.controller.StateSwitch(ss);
        }

        public IGameState GetGameState(gState s)
        {
            return gamestateList.Find(x => x.gState == s);
        }

        public gState GetCurrentState()
        {
            return IGameState.controller.Cur.gState;
        }
    }
}
