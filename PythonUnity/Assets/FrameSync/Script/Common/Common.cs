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
    }
}
