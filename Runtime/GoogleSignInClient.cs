using System;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    public abstract class GoogleSignInClient : ISignInClient
    {
        #region Fileds and Properties

        protected readonly SignInOptions          options;
        protected          Action<UserCredential> onSuccessCallback;
        protected          Action                 onFailureCallback;

        #endregion

        protected GoogleSignInClient(SignInOptions options, Action<UserCredential> onSuccessCallback,
            Action                                 onFailureCallback)
        {
            this.options = options;
            //this.onSuccessCallback =  _ => InProgress  = false;
            //this.onFailureCallback =  () => InProgress = false;
            this.onSuccessCallback = onSuccessCallback;
            this.onFailureCallback = onFailureCallback;
        }

        public abstract bool InProgress();

        public abstract void BeginSignIn();

        public static GoogleSignInClient CreateInstance(SignInOptions options, Action<UserCredential> onSuccessCallback,
            Action                                                    onFailureCallback)
        {
#if UNITY_STANDALONE
            return new StandaloneSignInClient(options, onSuccessCallback, onFailureCallback);
#endif

            throw new NotSupportedException(
                $"This platform ({Application.platform}) is not supported by the GoogleSignInClient");
        }
    }
}