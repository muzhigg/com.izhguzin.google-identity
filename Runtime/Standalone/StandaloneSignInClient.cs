using System;
using System.Text;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Standalone;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using Decoder = Izhguzin.GoogleIdentity.JWTDecoder.Decoder;

#pragma warning disable CS4014

namespace Izhguzin.GoogleIdentity
{
    internal sealed class StandaloneSignInClient : GoogleSignInClient
    {
        #region Fileds and Properties

        private const string PrefsKey = "gsi.user";

        private readonly StandaloneSignInOptions _standaloneOptions;

        #endregion

        internal StandaloneSignInClient(SignInOptions options) : base(options)
        {
            _standaloneOptions = options.Standalone;
        }

        protected override void BeginSignIn(GoogleRequestAsyncOperation operation)
        {
            try
            {
                if (TryLoadCredential(out UserCredential credential))
                    InvokeOnComplete(credential, operation, CommonStatus.Success);
                else
                    PerformSignIn(operation);
            }
            // Here we catch an exception from the TryLoadCredential method
            catch (Exception exception) when (exception is not GoogleSignInException)
            {
                OnExceptionCatch(operation, CommonStatus.LoadingCachedUserError, exception);
            }
        }

        private async Task PerformSignIn(GoogleRequestAsyncOperation operation)
        {
            try
            {
                AuthorizationRequestUrl authorizationRequestUrl = new(_standaloneOptions)
                {
                    ResponseType = "code",
                    Scope        = "email profile"
                };

                HttpCodeListener listener = new(authorizationRequestUrl.RedirectUri,
                    _standaloneOptions.ResponseHtml);
                Application.OpenURL(authorizationRequestUrl.BuildUrl());

                string code = await listener.WaitForCodeAsync(authorizationRequestUrl.State);
                await PerformCodeExchangeAsync(operation, code, authorizationRequestUrl);
            }
            catch (GoogleSignInException exception)
            {
                OnExceptionCatch(operation, exception.CommonStatus, exception);
            }
        }

        private async Task PerformCodeExchangeAsync(GoogleRequestAsyncOperation asyncOperation, string code,
            AuthorizationRequestUrl                                             requestUrl)
        {
            await PerformCodeExchangeAsync(code, requestUrl.ProofCodeKey.codeVerifier, requestUrl.RedirectUri);
            InvokeOnComplete(asyncOperation, CommonStatus.Success);
        }

        private async Task PerformCodeExchangeAsync(string code, string codeVerifier,
            string                                         redirectUri)
        {
            TokenRequestUrl requestUrl = new(_standaloneOptions)
            {
                Code         = code,
                RedirectUri  = redirectUri,
                CodeVerifier = codeVerifier
            };

            try
            {
                using UnityWebRequest tokenRequest = new(GoogleAuthConstants.TokenUrl, "POST")
                {
                    uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(requestUrl.BuildBody()))
                    {
                        contentType = "application/x-www-form-urlencoded"
                    },
                    downloadHandler = new DownloadHandlerBuffer()
                };

                await tokenRequest.SendWebRequest();
                HandleTokenResponse(tokenRequest);

                CurrentUser = GetCredentialFromResponse(tokenRequest.downloadHandler.text);
                SaveCredential(CurrentUser);
            }
            catch (Exception exception)
            {
                throw new GoogleSignInException(CommonStatus.ResponseError,
                    $"Failed to exchange authorization code for access token: {exception.Message}");
            }
        }

        private static void SaveCredential(UserCredential credential)
        {
            string tokenJson    = credential.Token.ToJson();
            string encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenJson));
            PlayerPrefs.SetString(PrefsKey, encodedToken);
        }

        private static bool TryLoadCredential([CanBeNull] out UserCredential credential)
        {
            credential = null;

            if (!PlayerPrefs.HasKey(PrefsKey)) return false;

            string encodedString = PlayerPrefs.GetString(PrefsKey);
            string decodedToken  = Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));

            credential = GetCredentialFromResponse(decodedToken);

            return true;
        }

        private static UserCredential GetCredentialFromResponse(string tokenResponseJson)
        {
            TokenResponse  response   = TokenResponse.FromJson(tokenResponseJson);
            UserCredential credential = Decoder.DecodePayload<UserCredential>(response.IdToken);
            credential.Token = response;
            return credential;
        }

        private void HandleTokenResponse(UnityWebRequest tokenRequest)
        {
            if (tokenRequest.result != UnityWebRequest.Result.Success)
                throw new GoogleSignInException(CommonStatus.ResponseError,
                    $"Token request failed with error: {tokenRequest.error}");

            if (tokenRequest.responseCode != 200)
                throw new GoogleSignInException(CommonStatus.ResponseError,
                    $"Token request failed with status code {tokenRequest.responseCode} and message: {tokenRequest.downloadHandler.text}");
        }
    }
}