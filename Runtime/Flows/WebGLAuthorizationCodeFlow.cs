#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;

namespace Izhguzin.GoogleIdentity.Flows
{
    internal class WebGLAuthorizationCodeFlow : WebGLBaseFlow
    {
        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnRequestSuccess(string response) { }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnRequestError(string error) { }


        [DllImport("__Internal")]
        private static extern void InitializeGisCodeClient(
            Action<string> initCallback, string         clientId, string scope,
            Action<string> callback,     Action<string> errorCallback);

        public WebGLAuthorizationCodeFlow(GoogleAuthOptions options,
            TaskCompletionSource<string>                    completionSource) : base(options, completionSource)
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

            InitializeGisCodeClient(OnInit, clientId,
                string.Join(' ', options.Scopes),
                OnRequestSuccess, OnRequestError);
        }

        public override Task Authorize()
        {
            throw new NotImplementedException();
        }
    }
}
#endif