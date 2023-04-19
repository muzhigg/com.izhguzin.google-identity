using System;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    public class SignInAsyncOperation
    {
        public event Action<SignInAsyncOperation> OnSuccess
        {
            add
            {
                bool shouldInvoke = false;

                lock (_lockObject)
                {
                    if (IsDone)
                    {
                        if (Status == ErrorCode.Success) shouldInvoke = true;
                    }
                    else
                    {
                        _onSuccess += value;
                    }
                }

                if (shouldInvoke) value(this);
            }
            remove => _onSuccess -= value;
        }

        public event Action<SignInAsyncOperation> OnFailure
        {
            add
            {
                bool shouldInvoke = false;

                lock (_lockObject)
                {
                    if (IsDone)
                    {
                        if (Status != ErrorCode.Success) shouldInvoke = true;
                    }
                    else
                    {
                        _onFailure += value;
                    }
                }

                if (shouldInvoke) value(this);
            }
            remove => _onFailure -= value;
        }

        public event Action<SignInAsyncOperation> OnComplete
        {
            add
            {
                bool shouldInvoke = false;

                lock (_lockObject)
                {
                    if (IsDone)
                        shouldInvoke = true;
                    else
                        _onComplete += value;
                }

                if (shouldInvoke) value(this);
            }
            remove => _onComplete -= value;
        }

        #region Fileds and Properties

        public GoogleSignInClient SignInClient { get; private set; }

        public ErrorCode Status { get; private set; }

        /// <summary>
        ///     Has the operation finished? (Read Only)
        /// </summary>
        public bool IsDone
        {
            get
            {
                lock (_lockObject)
                {
                    return _isDone;
                }
            }
            /*internal*/
            set
            {
                lock (_lockObject)
                {
                    _isDone = value;

                    if (_isDone) InvokeCompletionEvent();
                }
            }
        }

        private Action<SignInAsyncOperation> _onComplete;
        private Action<SignInAsyncOperation> _onSuccess;
        private Action<SignInAsyncOperation> _onFailure;

        private          bool   _isDone;
        private readonly object _lockObject = new();

        #endregion

        internal void InvokeCompletionEvent()
        {
            if (!IsDone)
            {
                Debug.LogError(
                    "Internal AsyncOperation error. Attempt to call completion event when operation is incomplete.");
                return;
            }

            if (Status == ErrorCode.Success)
            {
                _onSuccess?.Invoke(this);
                _onSuccess = null;
            }
            else
            {
                _onFailure?.Invoke(this);
                _onFailure = null;
            }

            _onComplete?.Invoke(this);
            _onComplete = null;
        }
    }
}