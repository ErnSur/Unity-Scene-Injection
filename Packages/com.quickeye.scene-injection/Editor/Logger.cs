using System;
using UnityEngine;

namespace QuickEye.SceneInjection
{
    internal static class Debug
    {
        public static void Log(object message)
        {
            message = $"[HSI] {message}";
            if (Application.isBatchMode)
                Console.WriteLine(message);
            else
                UnityEngine.Debug.Log(message);
        }
    }
}