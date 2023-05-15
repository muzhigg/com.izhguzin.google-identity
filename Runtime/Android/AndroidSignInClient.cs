//using System;
//using Izhguzin.GoogleIdentity.Android;
//using Izhguzin.GoogleIdentity.JWTDecoder;

//namespace Izhguzin.GoogleIdentity
//{
//    internal class AndroidSignInClient : GoogleIdentityService
//    {
//        #region Fileds and Properties

//        private readonly GoogleSignInClientProxy _clientProxy;

//        #endregion

//        public AndroidSignInClient(GoogleAuthOptions options) : base(options)
//        {
//            _clientProxy = GsiAppCompatActivity.ClientProxy;

//            if (_clientProxy == null)
//                throw new NullReferenceException(
//                    "The current android activity must be GsiAppCompatActivity or inherited from it.");

//            AndroidAuthOptions androidSignInOptions = options.Android;
//            string clientId =
//                androidSignInOptions.ClientId.ThrowIfNullOrEmpty(
//                    new NullReferenceException($"Client ID not set in {typeof(GoogleAuthOptions)}."));
//            _clientProxy.ConfigureClient(clientId, true);
//        }

//        protected override void BeginSignOut(GoogleRequestAsyncOperation operation)
//        {
//            _clientProxy.SignOut(
//                new GoogleSignInClientProxy.OnTaskCompleteListener(listener =>
//                {
//                    InvokeOnSuccess(null, operation);
//                    listener.javaInterface.Dispose();
//                }));
//        }

//        protected override void BeginSignIn(GoogleRequestAsyncOperation operation)
//        {
//            _clientProxy.Authorize(new GoogleSignInClientProxy.OnTaskCompleteListener(listener =>
//            {
//                if (listener.StatusCode is CommonStatus.Success or CommonStatus.SuccessCache)
//                    PerformSignIn(operation, listener);
//                else
//                    OnExceptionCatch(operation, listener.StatusCode,
//                        new RequestFailedException(listener.StatusCode, $"Failed to Sign In: {listener.Error}"));
//            }));
//        }

//        private void PerformSignIn(GoogleRequestAsyncOperation operation,
//            GoogleSignInClientProxy.OnTaskCompleteListener     listener)
//        {
//            UserCredential credential = Decoder.DecodePayload<UserCredential>(listener.Value);
//            credential.Token = new TokenResponse { IdToken = listener.Value };
//            InvokeOnSuccess(credential, operation, listener.StatusCode != CommonStatus.Success);
//            _clientProxy.SignOut(new GoogleSignInClientProxy.OnTaskCompleteListener(completeListener =>
//                completeListener.javaInterface.Dispose()));

//            listener.javaInterface.Dispose();
//        }
//    }
//}

