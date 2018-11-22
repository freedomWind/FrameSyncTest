using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Common
{
    public abstract class IGameSystem 
    {
        public T GetSys<T>() where T : IGameSystem
        {
            return GameFacade.GetSys<T>();
        }
    }

    public sealed class GameFacade
    {
        static GameFacade _ins;
        Dictionary<string, IGameSystem> managerDic;
        private GameFacade()
        {
            //add system
        }
        public static void Init()
        {
            _ins = new GameFacade();
        }

        public static T GetSys<T>() where T : IGameSystem
        {
            return _ins.Get<T>();
        }

        T Get<T>() where T : IGameSystem
        {
            IGameSystem m = null;
            System.Type type = typeof(T);
            if (managerDic.TryGetValue(type.Name, out m))
                return m as T;
            Debug.LogError("null system :"+type.Name);
            return null;
        }
    }
}
