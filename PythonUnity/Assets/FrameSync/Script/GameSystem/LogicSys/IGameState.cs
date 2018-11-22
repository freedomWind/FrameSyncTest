using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSys
{

    public enum gState
    {
        gameInit,
        gamePlay,
        gameLoading,
        gameReady,
        gameStart,
    }

    public abstract class IGameState
    {
        public abstract gState gState { get; }
        public static GameStateController controller
        {
            get { return GameStateController.GetIns(); }
        }
        public virtual void StateIn()
        {
        }
        public virtual void StateIng()
        { }
        public virtual void StateOut()
        {

        }
    }
}
