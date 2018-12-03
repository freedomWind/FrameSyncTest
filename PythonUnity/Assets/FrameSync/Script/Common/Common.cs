using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Common
{
    public static class Util
    {
        public static void Swap<T>(ref T t1, ref T t2)
        {
            T t = t2;
            t2 = t1;
            t1 = t;
        }

        public static T AddComponentIfNoExist<T>(this Transform trans) where T : Component
        {
            return trans.gameObject.AddComponentIfNoExsit<T>();
        }
        public static T AddComponentIfNoExsit<T>(this GameObject oo) where T : Component
        {
            T t = oo.GetComponent<T>();
            if (t == null)
                t = oo.AddComponent<T>();
            return t;
        }

    }

}
