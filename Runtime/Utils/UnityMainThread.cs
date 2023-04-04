using System.Threading;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    internal static class UnityMainThread
    {
        private static Thread _mainThread;

        public static bool IsRunningOnMainThread()
        {
#if UNITY_STANDALONE
            return Thread.CurrentThread == _mainThread;
#else
            throw new NotSupportedException();
#endif
        }

#if UNITY_STANDALONE
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            _mainThread = Thread.CurrentThread;
        }
#endif
    }
}