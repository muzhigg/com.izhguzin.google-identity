using System;
using System.Text;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Android;
using Izhguzin.GoogleIdentity.Standalone;
using UnityEngine;
using UnityEngine.Networking;
using Decoder = Izhguzin.GoogleIdentity.JWTDecoder.Decoder;

namespace Izhguzin.GoogleIdentity
{
    internal class AndroidSignInClient : GoogleSignInClient
    {
        private static UserCredential GetCredentialFromResponse(string tokenResponseJson)
        {
            TokenResponse response = TokenResponse.FromJson(tokenResponseJson);

            if (string.IsNullOrEmpty(response.RefreshToken)) Debug.LogWarning("No Refresh token");

            UserCredential credential = Decoder.DecodePayload<UserCredential>(response.IdToken);
            credential.Token = response;
            return credential;
        }

        private static void SaveRefreshToken(string token)
        {
            string encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            PlayerPrefs.SetString(PrefsKey, encodedToken);
        }

        #region Fileds and Properties

        private const    string                  PrefsKey = "gsi.user";
        private readonly AndroidSignInOptions    _androidSignInOptions;
        private readonly GoogleSignInClientProxy _clientProxy;

        #endregion

        public AndroidSignInClient(SignInOptions options) : base(options)
        {
            _clientProxy = GsiAppCompatActivity.ClientProxy;

            if (_clientProxy == null)
                throw new NullReferenceException(
                    "The current android activity must be GsiAppCompatActivity or inherited from it.");

            _androidSignInOptions = options.Android;
            string clientId =
                _androidSignInOptions.WebClientId.ThrowIfNullOrEmpty(
                    new NullReferenceException($"Client ID not set in {typeof(SignInOptions)}."));
            _clientProxy.ConfigureClient(clientId, _androidSignInOptions.UseOneTimeToken);
        }

        protected override void BeginSignIn(GoogleRequestAsyncOperation operation)
        {
            _clientProxy.SignIn(new GoogleSignInClientProxy.OnTaskCompleteListener(listener =>
            {
                if (listener.StatusCode is CommonStatus.Success or CommonStatus.SuccessCache)
                    PerformSignIn(operation, listener);
                else
                    OnExceptionCatch(operation, listener.StatusCode,
                        new GoogleSignInException(listener.StatusCode, $"Failed to Sign In: {listener.Error}"));
            }));
        }

        protected override void BeginRefreshToken(GoogleRequestAsyncOperation operation)
        {
            throw new NotImplementedException();
        }

        protected override void BeginRevokeAccess(GoogleRequestAsyncOperation operation)
        {
            _clientProxy.RevokeAccess(new GoogleSignInClientProxy.OnTaskCompleteListener(listener =>
            {
                if (listener.StatusCode == CommonStatus.Success)
                {
                    PlayerPrefs.DeleteKey(PrefsKey);
                    InvokeOnSuccess(null, operation);
                }
                else
                {
                    OnExceptionCatch(operation, listener.StatusCode,
                        new GoogleSignInException(listener.StatusCode, $"Failed to Revoke Access: {listener.Error}"));
                }

                listener.javaInterface.Dispose();
            }));
        }

        private async Task PerformSignIn(GoogleRequestAsyncOperation operation,
            GoogleSignInClientProxy.OnTaskCompleteListener           listener)
        {
            if (_androidSignInOptions.UseOneTimeToken)
            {
                UserCredential credential = Decoder.DecodePayload<UserCredential>(listener.Value);
                InvokeOnSuccess(credential, operation, true);
            }
            else
            {
                try
                {
                    UserCredential credential = await SendCodeExchangeRequestAsync(listener.Value);
                    SaveRefreshToken(credential.Token.RefreshToken);
                    InvokeOnSuccess(credential, operation, false);
                }
                catch (GoogleSignInException exception)
                {
                    OnExceptionCatch(operation, exception.CommonStatus, exception);
                }
                catch (NullReferenceException exception)
                {
                    OnExceptionCatch(operation, CommonStatus.DeveloperError, exception);
                }
            }

            listener.javaInterface.Dispose();
        }

        private async Task<UserCredential> SendCodeExchangeRequestAsync(string code)
        {
            TokenRequestUrl requestUrl = new(_androidSignInOptions)
            {
                Code        = code,
                RedirectUri = ""
            };
            using UnityWebRequest tokenRequest = CreatePostRequest(requestUrl);
            await tokenRequest.SendWebRequest();
            Debug.Log(tokenRequest.downloadHandler.text);
            CheckResponseForErrors(tokenRequest, "Token exchange");

            try
            {
                return GetCredentialFromResponse(tokenRequest.downloadHandler.text);
            }
            catch (Exception exception)
            {
                throw new GoogleSignInException(CommonStatus.ResponseError,
                    $"Failed to exchange authorization code for access token: {exception.Message}");
            }
        }

        private UnityWebRequest CreatePostRequest(RequestUrl url)
        {
            UnityWebRequest request = new(url.EndPointUrl, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(url.BuildBody()))
                {
                    contentType = "application/x-www-form-urlencoded"
                },
                downloadHandler = new DownloadHandlerBuffer()
            };

            return request;
        }

        private void CheckResponseForErrors(UnityWebRequest tokenRequest, string method)
        {
            if (tokenRequest.result != UnityWebRequest.Result.Success)
                throw new GoogleSignInException(CommonStatus.ResponseError,
                    $"{method} request failed with error: {tokenRequest.error}");

            if (tokenRequest.responseCode != 200)
                throw new GoogleSignInException(CommonStatus.ResponseError,
                    $"{method} request failed with status code {tokenRequest.responseCode} and message: {tokenRequest.downloadHandler.text}");
        }
    }
}