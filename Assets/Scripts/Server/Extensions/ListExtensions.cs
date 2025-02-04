using System.Collections.Generic;
using UnityEngine;

namespace CCGP.Server
{
    public static class ListExtensions
    {
        public static T Bottom<T>(this List<T> list)
        {
            return list[0];
        }

        public static T Top<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }
    }
}