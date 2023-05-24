#if UNITY_WEBGL && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;

#pragma warning disable CS4014

namespace Izhguzin.GoogleIdentity
{
    internal class WebGLIdentityService : GoogleIdentityService
    {
        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnRequestSuccess(string code)
        {
            _onRequestSuccessCallback?.Invoke(code);

            _onRequestSuccessCallback = null;
            _onRequestErrorCallback = null;
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnRequestError(string error)
        {
            _onRequestErrorCallback?.Invoke(error);
            _onRequestErrorCallback = null;
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
            _onInitErrorCallback = null;
        }

        private static Action         _onInitSuccessCallback;
        private static Action<string> _onInitErrorCallback;
        private static Action<string> _onRequestSuccessCallback;
        private static Action<string> _onRequestErrorCallback;

        public WebGLIdentityService(GoogleAuthOptions options) : base(options) { }

        public override Task<TokenResponse> AuthorizeAsync()
        {
            ValidateInProgress();
            InProgress = true;

            TaskCompletionSource<TokenResponse> taskCompletionSource = new();

            _onRequestSuccessCallback = code => PerformCodeExchangeAsync(code, taskCompletionSource);
            _onRequestErrorCallback =
                type => SetException(type, taskCompletionSource);

            AuthorizeGIS();

            return taskCompletionSource.Task;
        }

        internal override Task InitializeAsync()
        {
            ValidateInProgress();
            InProgress = true;

            TaskCompletionSource<string> tcs = new();

            _onInitSuccessCallback = () =>
            {
                InProgress = false;
                tcs.SetResult(null);
            };
            _onInitErrorCallback = s =>
            {
                InProgress = false;
                tcs.SetException(
                    new Exception(s));
            };

            InitializeGisCodeClient(OnInit, Options.ClientId,
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
                    taskCompletionSource.SetException(new AuthorizationFailedException(CommonErrorCodes.SignInCancelled,
                        "Popup window closed"));
                    break;
                //case "access_denied":
                //    taskCompletionSource.SetException(
                //        new AuthorizationFailedException(CommonErrorCodes.ResolutionRequired, ""));
                //    break;
                default:
                    taskCompletionSource.SetException(new AuthorizationFailedException(CommonErrorCodes.Error,
                        "There was an error during authorization. Details in the browser console."));
                    break;
            }

            InProgress = false;
        }

        private async Task PerformCodeExchangeAsync(string code,
            TaskCompletionSource<TokenResponse>            taskCompletionSource)
        {
            try
            {
                TokenResponse response = await SendCodeExchangeRequestAsync(code, null, "postmessage");
                taskCompletionSource.SetResult(response);
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
            finally
            {
                InProgress = false;
            }
        }
    }
}
#endif