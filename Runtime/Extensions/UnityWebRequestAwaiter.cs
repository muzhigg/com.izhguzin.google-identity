using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace Izhguzin.GoogleIdentity
{
    internal struct UnityWebRequestAwaiter : INotifyCompletion
    {
        private readonly UnityWebRequestAsyncOperation _asyncOp;
        private          Action                        _continuation;

        public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
        {
            _asyncOp      = asyncOp;
            _continuation = null;
        }

        public readonly bool IsCompleted => _asyncOp.isDone;

        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            _continuation      =  continuation;
            _asyncOp.completed += OnRequestCompleted;
        }

        private readonly void OnRequestCompleted(AsyncOperation obj)
        {
            _continuation?.Invoke();
        }
    }
}