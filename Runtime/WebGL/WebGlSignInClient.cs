//using System;
//using System.Runtime.InteropServices;
//using AOT;
//using Izhguzin.GoogleIdentity.JWTDecoder;

//namespace Izhguzin.GoogleIdentity
//{
//    public class WebGlSignInClient : GoogleIdentityService
//    {
//        [DllImport("__Internal")]
//        internal static extern void InitGsiClient(string webClientId);

//        [DllImport("__Internal")]
//        internal static extern void SignIn(Action<string> onSuccess, Action<string> onFailure);

//        [MonoPInvokeCallback(typeof(Action<string>))]
//        private static void OnSuccess(string idToken)
//        {
//            UserCredential credential = Decoder.DecodePayload<UserCredential>(idToken);
//            _instance.InvokeOnSuccess(credential, _operation);
//        }

//        [MonoPInvokeCallback(typeof(Action<string>))]
//        private static void OnFailure(string error) { }

//        private static WebGlSignInClient           _instance;
//        private static GoogleRequestAsyncOperation _operation;

//        public WebGlSignInClient(GoogleAuthOptions options) : base(options)
//        {
//            _instance = this;
//            InitGsiClient(
//                options.WebGL.ClientId.ThrowIfNullOrEmpty(new NullReferenceException("Client Id is not set")));
//        }

//        protected override void BeginSignOut(GoogleRequestAsyncOperation operation)
//        {
//            throw new NotImplementedException();
//        }

//        protected override void BeginSignIn(GoogleRequestAsyncOperation operation)
//        {
//            _operation = operation;
//            SignIn(OnSuccess, OnFailure);
//        }
//    }
//}

