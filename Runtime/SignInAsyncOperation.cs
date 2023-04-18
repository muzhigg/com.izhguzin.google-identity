using System;
using System.Collections;

namespace Izhguzin.GoogleIdentity
{
    public class SignInAsyncOperation
    {
        private event Action Completed;

        #region Fileds and Properties

        public bool IsDone
        {
            get
            {
                lock (_lockObject)
                {
                    return _isDone;
                }
            }
            internal set
            {
                lock (_lockObject)
                {
                    _isDone = value;
                    if (_isDone) Completed?.Invoke();
                }
            }
        }

        private          bool   _isDone;
        private readonly object _lockObject = new();

        #endregion

        public IEnumerator WaitForCompletion()
        {
            while (!IsDone) yield return null;
        }

        public void OnComplete(Action callback)
        {
            bool shouldInvoke = false;
            lock (_lockObject)
            {
                if (_isDone)
                    shouldInvoke = true;
                else
                    Completed += callback;
            }

            if (shouldInvoke) callback?.Invoke();
        }
    }
}