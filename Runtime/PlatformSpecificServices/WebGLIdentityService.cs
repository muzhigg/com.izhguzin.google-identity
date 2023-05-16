#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    internal class WebGLIdentityService : GoogleIdentityService
    {
        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnRequestSuccess(string code)
        {
            Debug.Log(1);
            _onRequestSuccessCallback?.Invoke(code);

            _onRequestSuccessCallback = null;
            _onRequestErrorCallback   = null;
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnRequestError(string error)
        {
            _onRequestErrorCallback?.Invoke(error);
            _onRequestErrorCallback   = null;
            _onRequestSuccessCallback = null;
        }

        [DllImport("__Internal")]
        private static extern void InitializeGisCodeClient(
            Action<string> initCallback, string         clientIdStr, string scopeStr,
            Action<string> callback,     Action<string> errorCallback);

        [DllImport("__Internal")]
        private static extern void AuthorizeGIS();

        [DllImport("__Internal")]
        private static extern string GetURLFromPage();

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnInit(string error)
        {
            if (string.IsNullOrEmpty(error))
                _onInitSuccessCallback?.Invoke();
            else
                _onInitErrorCallback?.Invoke(error);

            _onInitSuccessCallback = null;
            _onInitErrorCallback   = null;
        }

        private static Action         _onInitSuccessCallback;
        private static Action<string> _onInitErrorCallback;
        private static Action<string> _onRequestSuccessCallback;
        private static Action<string> _onRequestErrorCallback;

        public WebGLIdentityService(GoogleAuthOptions options) : base(options) { }

        public override Task<TokenResponse> Authorize()
        {
            TaskCompletionSource<TokenResponse> taskCompletionSource = new();

            _onRequestSuccessCallback = code => PerformCodeExchange(code, taskCompletionSource);
            _onRequestErrorCallback =
                type => SetException(type, taskCompletionSource);

            AuthorizeGIS();

            return taskCompletionSource.Task;
        }

        internal override Task InitializeAsync()
        {
            TaskCompletionSource<string> tcs = new();

            _onInitSuccessCallback = () => tcs.SetResult(null);
            _onInitErrorCallback = s => tcs.SetException(
                new Exception(s));

            Debug.LogWarning(Options.ClientId);

            if (string.IsNullOrEmpty(Options.ClientId))
            {
                tcs.SetException(new NullReferenceException(
                    $"Client Id is not set in {nameof(GoogleAuthOptions)}."));
                return tcs.Task;
            }

            string clientId = Options.ClientId;

            InitializeGisCodeClient(OnInit, clientId,
                string.Join(' ', Options.Scopes),
                OnRequestSuccess, OnRequestError);

            return tcs.Task;
        }

        private void SetException(string type, TaskCompletionSource<TokenResponse> taskCompletionSource)
        {
            switch (type)
            {
                case "popup_failed_to_open":
                    taskCompletionSource.SetException(new AuthorizationFailedException(CommonErrorCodes.Error,
                        "Failed to open popup window."));
                    break;
                case "popup_closed":
                    taskCompletionSource.SetException(new AuthorizationFailedException(CommonErrorCodes.Canceled,
                        "Popup window closed"));
                    break;
                default:
                    taskCompletionSource.SetException(new AuthorizationFailedException(CommonErrorCodes.Error,
                        "There was an error during authorization. Details in the browser console."));
                    break;
            }
        }

        private async Task PerformCodeExchange(string code, TaskCompletionSource<TokenResponse> taskCompletionSource)
        {
            try
            {
                Debug.Log(2);
                Debug.Log(GetURLFromPage());
                TokenResponse response = await SendCodeExchangeRequestAsync(code, null, "postmessage");
                taskCompletionSource.SetResult(response);
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
        }
    }
}
#endif