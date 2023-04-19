using System;
using Izhguzin.GoogleIdentity.Android;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    public delegate void OnSuccessCallback(UserCredential credential);

    public delegate void OnFailureCallback(ErrorCode errorCode, string errorMessage);

    internal class AndroidSignInClient : GoogleSignInClient
    {
        #region Fileds and Properties

        private readonly GoogleSignInClientProxy _proxy;
        private          bool                    _inProgress;

        #endregion

        public AndroidSignInClient(SignInOptions options, OnSuccessCallback onSuccessCallback,
            OnFailureCallback onFailureCallback) : base(options, onSuccessCallback, onFailureCallback)
        {
            _proxy = GsiAppCompatActivity.ClientProxy;

            if (_proxy == null)
                // TODO : Handle error
                return;

            _proxy.SetListeners(new GoogleSignInClientProxy.OnSuccessListener(OnSuccessCallback),
                new GoogleSignInClientProxy.OnFailureListener(OnFailureCallback));
            //_proxy = new GoogleSignInClientProxy(UnityPlayer.CurrentActivity,
            //    new GoogleSignInClientProxy.OnSuccessListener(OnSuccessCallback),
            //    new GoogleSignInClientProxy.OnFailureListener(OnFailureCallback));
        }

        public override void BeginSignInOld()
        {
            base.BeginSignInOld();

            if (_inProgress)
            {
                Debug.LogError("Sign-in process already in progress");
                return;
            }

            _inProgress = true;

            string clientId = options.GetAndroidCredentials();
            if (string.IsNullOrEmpty(clientId))
                throw new GoogleSignInException(
                    $"Client ID not set in {typeof(SignInOptions)}.");

            _proxy.BuildRequest(clientId, options.GetFilterByAuthorizedAccounts(), options.GetAutoSelectEnabled());
            _proxy.BeginSignIn();
        }

        public override void SignOutOld()
        {
            throw new NotImplementedException();
        }

        public override void RefreshTokenOld(UserCredential credential, OnSuccessCallback callback)
        {
            throw new NotImplementedException();
        }

        public override void RevokeAccessOld(UserCredential credential)
        {
            throw new NotImplementedException();
        }

        private void OnFailureCallback(ErrorCode errorCode, string errorMessage)
        {
            InvokeOnFailureCallback(errorCode);
            Debug.LogException(new GoogleSignInException($"Error occurred in Sign In Client: {errorMessage}"));
        }

        private void OnSuccessCallback(UserCredential credential)
        {
            Debug.Log(2);
            _inProgress = false;
            onSuccessCallback.Invoke(credential);
        }
    }
}