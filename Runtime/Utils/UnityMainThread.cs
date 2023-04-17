using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Izhguzin.GoogleIdentity
{
    internal static class PlayerLoopSystemExtensions
    {
        public static ref PlayerLoopSystem Find<T>(this PlayerLoopSystem root)
        {
            for (int i = 0; i < root.subSystemList.Length; i++)
                if (root.subSystemList[i].type == typeof(T))
                    return ref root.subSystemList[i];

            throw new Exception($"System of type '{typeof(T).Name}' not found inside system '{root.type.Name}'.");
        }
    }

    internal class PlayerLoopSystemSubscription<T> : IDisposable
    {
        #region Fileds and Properties

        private readonly Action _callback;

        #endregion

        public PlayerLoopSystemSubscription(Action callback)
        {
            _callback = callback;
            Subscribe();
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        private void Invoke()
        {
            _callback.Invoke();
        }

        private void Subscribe()
        {
            PlayerLoopSystem     loop   = PlayerLoop.GetCurrentPlayerLoop();
            ref PlayerLoopSystem system = ref loop.Find<T>();
            system.updateDelegate += Invoke;
            PlayerLoop.SetPlayerLoop(loop);
        }

        private void Unsubscribe()
        {
            PlayerLoopSystem     loop   = PlayerLoop.GetCurrentPlayerLoop();
            ref PlayerLoopSystem system = ref loop.Find<T>();
            system.updateDelegate -= Invoke;
            PlayerLoop.SetPlayerLoop(loop);
        }
    }

    public static class UnityMainThread
    {
        private static          Thread                  _mainThread;
        private static readonly ConcurrentQueue<Action> _actionQueue = new();

        public static bool IsRunningOnMainThread()
        {
            return Thread.CurrentThread == _mainThread;
        }

        public static void RunOnMainThread(Action action)
        {
            _actionQueue.Enqueue(action);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            _mainThread = Thread.CurrentThread;
            PlayerLoopSystemSubscription<Update> subscription = new PlayerLoopSystemSubscription<Update>(Update);
            Application.quitting += subscription.Dispose;
        }

        private static void Update()
        {
            while (_actionQueue.TryDequeue(out Action action)) action?.Invoke();
        }
    }
}