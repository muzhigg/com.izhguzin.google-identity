using System;
using System.Runtime.CompilerServices;

namespace Izhguzin.GoogleIdentity
{
    public struct SignInAsyncOperationAwaiter : INotifyCompletion
    {
        public readonly bool IsCompleted => _asyncOp.IsDone;

        private readonly GoogleRequestAsyncOperation _asyncOp;
        private          Action                      _continuation;

        public SignInAsyncOperationAwaiter(GoogleRequestAsyncOperation asyncOp)
        {
            _asyncOp      = asyncOp;
            _continuation = null;
        }

        public readonly GoogleRequestAsyncOperation GetResult()
        {
            return _asyncOp;
        }

        public void OnCompleted(Action continuation)
        {
            _continuation       =  continuation;
            _asyncOp.OnComplete += OnOperationCompleted;
        }

        private readonly void OnOperationCompleted(GoogleRequestAsyncOperation obj)
        {
            _continuation?.Invoke();
        }
    }
}