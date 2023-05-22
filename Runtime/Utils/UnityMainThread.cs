using System;
using System.Threading;
using Izhguzin.GoogleIdentity.UnityMainThreadDispatcher;
using UnityEngine;
using UnityEngine.Scripting;

namespace Izhguzin.GoogleIdentity.Utils
{
    /// <summary>
    ///     Auxiliary class to work with the main thread.
    /// </summary>
    public static class UnityMainThread
    {
        /// <summary>
        ///     Checks the current thread.
        /// </summary>
        public static bool IsRunningOnMainThread()
        {
            return Thread.CurrentThread == _mainThread;
        }

        /// <summary>
        ///     Starts the passed method in the next frame.
        /// </summary>
        public static void RunOnMainThread(Action action)
        {
            Dispatcher.Enqueue(action);
        }

        [RuntimeInitializeOnLoadMethod, Preserve]
        private static void Init()
        {
            _mainThread = Thread.CurrentThread;
        }

        private static Thread _mainThread;
    }
}