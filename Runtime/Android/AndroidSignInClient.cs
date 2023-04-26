using System;

namespace Izhguzin.GoogleIdentity
{
    public delegate void OnSuccessCallback(UserCredential credential);

    public delegate void OnFailureCallback(CommonStatus commonStatus, string errorMessage);

    internal class AndroidSignInClient : GoogleSignInClient
    {
        public AndroidSignInClient(SignInOptions options) : base(options) { }

        //public AndroidSignInClient(SignInOptions options, OnSuccessCallback onSuccessCallback,
        //    OnFailureCallback onFailureCallback)
        //{
        //    _proxy = GsiAppCompatActivity.ClientProxy;

        //    if (_proxy == null)
        //        // TODO : Handle error
        //        return;

        //    _proxy.SetListeners(new GoogleSignInClientProxy.OnSuccessListener(OnSuccessCallback),
        //        new GoogleSignInClientProxy.OnFailureListener(OnFailureCallback));
        //    //_proxy = new GoogleSignInClientProxy(UnityPlayer.CurrentActivity,
        //    //    new GoogleSignInClientProxy.OnSuccessListener(OnSuccessCallback),
        //    //    new GoogleSignInClientProxy.OnFailureListener(OnFailureCallback));
        //}

        protected override void BeginSignIn(GoogleRequestAsyncOperation operation)
        {
            throw new NotImplementedException();
        }

        private void OnFailureCallback(CommonStatus commonStatus, string errorMessage)
        {
            //InvokeOnFailureCallback(commonStatus);
            //Debug.LogException(new GoogleSignInException($"Error occurred in Sign In Client: {errorMessage}"));
        }

        private void OnSuccessCallback(UserCredential credential)
        {
            //Debug.Log(2);
            //_inProgress = false;
            //onSuccessCallback.Invoke(credential);
        }

        //private readonly GoogleSignInClientProxy _proxy;
        //private          bool                    _inProgress;
    }
}