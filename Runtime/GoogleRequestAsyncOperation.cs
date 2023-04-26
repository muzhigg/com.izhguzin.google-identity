using System;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     Asynchronous operation object requests received from GoogleSignInClient.
    ///     <para>
    ///         You can yield until it continues,
    ///         or await until it's complete,
    ///         or register an event handler with GoogleRequestAsyncOperation.OnComplete,
    ///         or manually check whether it's done.
    ///     </para>
    /// </summary>
    public sealed class GoogleRequestAsyncOperation
    {
        public event Action<GoogleRequestAsyncOperation> OnComplete
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

        /// <summary>
        ///     The client who created this operation.
        /// </summary>
        public GoogleSignInClient SignInClient { get; }

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
        }

        /// <summary>
        ///     Error message. If the operation was successful the value will be null.
        /// </summary>
        public string Error { get; internal set; }

        private Action<GoogleRequestAsyncOperation> _onComplete;

        private          bool   _isDone;
        private readonly object _lockObject = new();

        #endregion

        internal GoogleRequestAsyncOperation(GoogleSignInClient client)
        {
            SignInClient = client;
        }

        internal void InvokeCompletionEvent(CommonStatus code)
        {
            lock (_lockObject)
            {
                _isDone = true;

                _onComplete?.Invoke(this);
                _onComplete = null;
            }
        }
    }
}