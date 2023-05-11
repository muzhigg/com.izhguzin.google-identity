#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;

namespace Izhguzin.GoogleIdentity.Flows
{
    internal abstract class WebGLBaseFlow : IAuthorizationModel
    {
        [MonoPInvokeCallback(typeof(Action<string>))]
        protected static void OnInit(string error)
        {
            if (string.IsNullOrEmpty(error))
                onInitSuccessCallback?.Invoke();
            else
                onInitErrorCallback?.Invoke(error);

            onInitSuccessCallback = null;
            onInitErrorCallback   = null;
        }

        protected static Action         onInitSuccessCallback;
        protected static Action<string> onInitErrorCallback;

        protected WebGLBaseFlow(GoogleAuthOptions options,
            TaskCompletionSource<string>          completionSource)
        {
            onInitSuccessCallback = () => completionSource.SetResult(null);
            onInitErrorCallback = s => completionSource.SetException(
                new Exception(s));
        }

        public abstract Task Authorize();
    }

    internal class WebGLImplicitFlow : WebGLBaseFlow
    {
        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnRequestSuccess(string response) { }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnRequestError(string error) { }

        [DllImport("__Internal")]
        private static extern void InitializeGisImplicitClient(
            Action<string> initCallback, string         clientId, string scope,
            Action<string> callback,     Action<string> errorCallback);

        public WebGLImplicitFlow(GoogleAuthOptions options,
            TaskCompletionSource<string>           completionSource) : base(options, completionSource)
        {
            string clientId;

            try
            {
                clientId = options.ClientId.ThrowIfNullOrEmpty(
                    new NullReferenceException(
                        $"Client Id is not set in {nameof(GoogleAuthOptions)}."));
            }
            catch (Exception e)
            {
                completionSource.SetException(e);
                return;
            }

            InitializeGisImplicitClient(OnInit, clientId, string.Join(' ', options.Scopes),
                OnRequestSuccess, OnRequestError);
        }

        public override Task Authorize()
        {
            throw new NotImplementedException();
        }
    }
}
#endif