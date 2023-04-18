using System;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    public abstract class GoogleSignInClient : ISignInClient
    {
        #region Fileds and Properties

        protected readonly SignInOptions     options;
        protected          OnSuccessCallback onSuccessCallback;
        protected          OnFailureCallback onFailureCallback;

        private bool _inProgress;

        #endregion

        protected GoogleSignInClient(SignInOptions options, OnSuccessCallback onSuccessCallback,
            OnFailureCallback                      onFailureCallback)
        {
            this.options           = options;
            this.onSuccessCallback = (_ => _inProgress      = false) + onSuccessCallback;
            this.onFailureCallback = ((_, _) => _inProgress = false) + onFailureCallback;
        }

        public bool InProgress()
        {
            return _inProgress;
        }

        public virtual void BeginSignIn()
        {
            if (_inProgress)
            {
                Debug.LogWarning("Sign-in process already in progress");
                return;
            }

            _inProgress = true;
        }

        public abstract void SignOut();
        public abstract void RefreshToken(UserCredential credential, OnSuccessCallback callback);
        public abstract void RevokeAccess(UserCredential credential);

        public static GoogleSignInClient CreateInstance(SignInOptions options, OnSuccessCallback onSuccessCallback,
            OnFailureCallback                                         onFailureCallback)
        {
#if UNITY_STANDALONE
            return new StandaloneSignInClient(options, onSuccessCallback, onFailureCallback);
#endif

#if UNITY_ANDROID
            return new AndroidSignInClient(options, onSuccessCallback, onFailureCallback);
#endif

            throw new NotSupportedException(
                $"This platform ({Application.platform}) is not supported by the GoogleSignInClient");
        }

        protected void InvokeOnFailureCallback(ErrorCode errorCode)
        {
            string message = GenerateMessage(errorCode);

            onFailureCallback.Invoke(errorCode, message);
        }

        protected void InvokeOnSuccessCallback(UserCredential credential)
        {
            onSuccessCallback.Invoke(credential);
        }

        private static string GenerateMessage(ErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ErrorCode.Other:
                    goto case ErrorCode.Error;
                case ErrorCode.ResponseError:
                    return "Google server response error. Try again later.";
                case ErrorCode.InvalidAccount:
                    return "Invalid account.";
                case ErrorCode.NetworkError:
                    return "Connection error. Try again later.";
                case ErrorCode.DeveloperError:
                    return "Developer error. Please check application configuration.";
                case ErrorCode.Error:
                    return "Unexpected error occurred.";
                case ErrorCode.Timeout:
                    return "Timeout while awaiting response. Try again later.";
                case ErrorCode.Canceled:
                    return "Operation canceled. Please try again.";
                default:
                    throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null);
            }
        }
    }
}