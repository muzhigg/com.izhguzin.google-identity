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
        private static void SaveCredential(UserCredential credential)
        {
            string tokenJson    = credential.Token.ToJson();
            string encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenJson));
            PlayerPrefs.SetString(PrefsKey, encodedToken);
        }

        /// <exception cref="JsonDeserializationException"></exception>
        private static bool TryLoadCredential([CanBeNull] out UserCredential credential)
        {
            credential = null;

            if (!PlayerPrefs.HasKey(PrefsKey)) return false;

            credential = LoadCredential();
            return true;
        }

        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="JsonDeserializationException"></exception>
        private static UserCredential LoadCredential()
        {
            string encodedString = PlayerPrefs.GetString(PrefsKey, null);

            if (string.IsNullOrEmpty(encodedString))
                throw new NullReferenceException("No User Credential found in PlayerPrefs.");

            string decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));
            return GetCredentialFromResponse(decodedToken);
        }

        /// <exception cref="JsonDeserializationException"></exception>
        private static UserCredential GetCredentialFromResponse(string tokenResponseJson)
        {
            TokenResponse  response   = TokenResponse.FromJson(tokenResponseJson);
            UserCredential credential = Decoder.DecodePayload<UserCredential>(response.IdToken);
            credential.Token = response;
            return credential;
        }

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
            if (HasCachedUser())
                PerformOfflineSignInAsync(operation);
            else
                PerformSignInAsync(operation);
        }

        protected override void BeginRefreshToken(GoogleRequestAsyncOperation operation)
        {
            if (!CanRefreshToken(out string message))
            {
                OnExceptionCatch(operation, CommonStatus.DeveloperError, new NullReferenceException(message));
                return;
            }

            PerformRefreshTokenAsync(operation);
        }

        protected override void BeginRevokeAccess(GoogleRequestAsyncOperation operation)
        {
            if (CurrentUser == null)
            {
                OnExceptionCatch(operation, CommonStatus.DeveloperError,
                    new NullReferenceException("Unable to revoke access: User is not signed in."));
                return;
            }

            PerformRevokeAccessAsync(operation);
        }

        private async Task PerformOfflineSignInAsync(GoogleRequestAsyncOperation operation)
        {
            try
            {
                UserCredential credential = LoadCredential();

                if (credential.IsExpired())
                {
                    credential = await SendRefreshTokenRequestAsync(credential);
                    SaveCredential(credential);
                }

                InvokeOnSuccess(credential, operation);
            }
            catch (GoogleSignInException exception)
            {
                OnExceptionCatch(operation, exception.CommonStatus, exception);
            }
            catch (Exception exception)
            {
                OnExceptionCatch(operation, CommonStatus.LoadingCachedUserError, exception);
            }
        }

        private async Task PerformSignInAsync(GoogleRequestAsyncOperation operation)
        {
            try
            {
                AuthorizationRequestUrl requestUrl = new(_standaloneOptions)
                {
                    ResponseType = "code",
                    Scope        = "email profile",
                    Prompt       = "consent",
                    AccessType   = "offline"
                };
                HttpCodeListener listener = new(requestUrl.RedirectUri,
                    _standaloneOptions.ResponseHtml);
                Application.OpenURL(requestUrl.BuildUrl());

                string code = await listener.WaitForCodeAsync(requestUrl.State);

                UserCredential credential = await SendCodeExchangeRequestAsync(code,
                    requestUrl.ProofCodeKey.codeVerifier, requestUrl.RedirectUri);

                SaveCredential(credential);
                InvokeOnSuccess(credential, operation);
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

        private async Task PerformRevokeAccessAsync(GoogleRequestAsyncOperation operation)
        {
            try
            {
                if (CurrentUser.Token.IsEffectivelyExpired() && !string.IsNullOrEmpty(CurrentUser.Token.RefreshToken))
                    CurrentUser = await SendRefreshTokenRequestAsync(CurrentUser);

                await SendRevokeAccessRequestAsync();

                CurrentUser = null;
                PlayerPrefs.DeleteKey(PrefsKey);
                InvokeOnComplete(operation, CommonStatus.Success);
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

        private async Task PerformRefreshTokenAsync(GoogleRequestAsyncOperation operation)
        {
            try
            {
                UserCredential credential = await SendRefreshTokenRequestAsync(CurrentUser);
                SaveCredential(credential);
                InvokeOnSuccess(credential, operation);
            }
            catch (NullReferenceException exception)
            {
                OnExceptionCatch(operation, CommonStatus.DeveloperError, exception);
            }
            catch (GoogleSignInException exception)
            {
                OnExceptionCatch(operation, exception.CommonStatus, exception);
            }
        }

        /// <exception cref="GoogleSignInException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        private async Task<UserCredential> SendCodeExchangeRequestAsync(string code, string codeVerifier,
            string                                                             redirectUri)
        {
            TokenRequestUrl requestUrl = new(_standaloneOptions)
            {
                Code         = code,
                RedirectUri  = redirectUri,
                CodeVerifier = codeVerifier
            };
            using UnityWebRequest tokenRequest = CreatePostRequest(requestUrl);
            await tokenRequest.SendWebRequest();
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

        /// <exception cref="GoogleSignInException"></exception>
        private async Task SendRevokeAccessRequestAsync()
        {
            TokenResponse          token      = CurrentUser.Token;
            RevokeAccessRequestUrl requestUrl = new(token.AccessToken);

            using UnityWebRequest webRequest = CreatePostRequest(requestUrl);
            await webRequest.SendWebRequest();

            CheckResponseForErrors(webRequest, "Revoke access");
        }

        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="GoogleSignInException"></exception>
        private async Task<UserCredential> SendRefreshTokenRequestAsync(UserCredential expiredCredential)
        {
            TokenResponse          oldToken   = expiredCredential.Token;
            RefreshTokenRequestUrl requestUrl = new(_standaloneOptions, oldToken.RefreshToken);

            using UnityWebRequest refreshRequest = CreatePostRequest(requestUrl);
            await refreshRequest.SendWebRequest();
            CheckResponseForErrors(refreshRequest, "Refresh token");

            try
            {
                UserCredential credential = GetCredentialFromResponse(refreshRequest.downloadHandler.text);
                credential.Token.RefreshToken = oldToken.RefreshToken;
                return credential;
            }
            catch (Exception exception)
            {
                throw new GoogleSignInException(CommonStatus.ResponseError,
                    $"Failed to refresh token: {exception.Message}");
            }
        }

        private bool HasCachedUser()
        {
            return PlayerPrefs.HasKey(PrefsKey);
        }

        private bool CanRefreshToken(out string message)
        {
            message = null;

            if (CurrentUser == null)
            {
                message = "Unable to refresh token. User must sign in.";
                return false;
            }

            if (!string.IsNullOrEmpty(CurrentUser.Token.RefreshToken)) return true;

            message =
                "Unable to refresh token. UserCredential has no refresh token. Call the sign out method with revoke access, and then sign in again.";
            return false;
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

        /// <exception cref="GoogleSignInException"></exception>
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