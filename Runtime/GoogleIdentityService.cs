using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Izhguzin.GoogleIdentity
{
    public abstract class GoogleIdentityService : IIdentityService
    {
        public static async Task InitializeAsync(GoogleAuthOptions options)
        {
            if (UnityMainThread.IsRunningOnMainThread() == false)
                throw new InitializationException(
                    "You are attempting to initialize Google Identity Service from a non-Unity Main Thread. Google Identity Service can only be initialized from Main Thread");

            if (Application.isPlaying == false)
                throw new InitializationException(
                    "You are attempting to initialize Google Identity Service in Edit Mode. Google Identity Service can only be initialized in Play Mode");

            if (_instance != null)
                throw new InitializationException(
                    "You are attempting to initialize a Google Identity Service that has already been initialized.");

            try
            {
                CheckOptions(options);
                GoogleIdentityService instance = CreateInstance(options);
                await instance.InitializeAsync();
                _instance = instance;
            }
            catch (Exception ex)
            {
                throw new InitializationException(
                    $"An error occurred during initialization: {ex.Message}");
            }
        }

        private static void CheckOptions(GoogleAuthOptions options)
        {
            if (options.ClientId.IsNullOrEmpty())
                throw new NullReferenceException("GoogleAuthOptions is missing a required parameter (client_id)");

            if (options.ClientSecret.IsNullOrEmpty())
                throw new NullReferenceException(
                    "GoogleAuthOptions is missing a required parameter (client_secret)");

            if (options.Scopes.Count == 0)
                throw new NullReferenceException("GoogleAuthOptions is missing a required parameter (scopes)");
        }

        private static GoogleIdentityService CreateInstance(GoogleAuthOptions options)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            return new StandaloneIdentityService(options);
#elif UNITY_ANDROID
            return new AndroidIdentityService(options);
#elif UNITY_WEBGL
            return new WebGLIdentityService(options);
#else
            throw new InitializationException(
                $"This platform ({Application.platform}) is not supported by " +
                "the GoogleIdentityService");
#endif
        }

        private static GoogleIdentityService _instance;

        #region Fileds and Properties

        public static GoogleIdentityService Instance
        {
            get
            {
                if (_instance == null)
                    throw new InitializationException(
                        "Google Identity Service is not initialized.");

                return _instance;
            }
        }

        protected GoogleAuthOptions Options { get; }

        protected bool InProgress { get; set; }

        #endregion

        protected GoogleIdentityService(GoogleAuthOptions options)
        {
            Options = options;
        }

        public abstract Task<TokenResponse> AuthorizeAsync();

        public async Task<TokenResponse> LoadStoredTokenAsync(string userId)
        {
            ValidateInProgress();
            InProgress = true;

            ITokenStorage tokenStorage = Options.TokenStorage;
            if (tokenStorage == null)
                throw new NullReferenceException(
                    "To get a cached token, you must first set the storage to GoogleAuthOptions.");

            string tokenJson = await tokenStorage.LoadTokenAsync(userId);

            if (string.IsNullOrEmpty(tokenJson)) return null;

            TokenResponse response = TokenResponse.FromJson(tokenJson);

            InProgress = false;
            return response;
        }

        protected async Task<TokenResponse> SendCodeExchangeRequestAsync(string code, string codeVerifier,
            string                                                              redirectUri)
        {
            TokenRequestUrl tokenRequestUrl = new()
            {
                Code         = code,
                RedirectUri  = redirectUri,
                ClientId     = Options.ClientId,
                ClientSecret = Options.ClientSecret,
                CodeVerifier = codeVerifier
            };

            using UnityWebRequest webRequest = tokenRequestUrl.CreatePostRequest();
            await webRequest.SendWebRequest();

            try
            {
                CheckResponseForErrors(webRequest, "Token exchange");
            }
            catch (RequestFailedException exception)
            {
                throw new AuthorizationFailedException(exception.ErrorCode, exception.Message);
            }

            try
            {
                return TokenResponse.FromJson(webRequest.downloadHandler.text);
            }
            catch (Exception exception)
            {
                throw new AuthorizationFailedException(CommonErrorCodes.DeserializationError,
                    $"Deserialization error: {exception.Message}", exception);
            }
        }

        protected void ValidateInProgress()
        {
            if (InProgress)
                throw new InvalidOperationException("GoogleIdentityService is already executing the request.");
        }

        internal abstract Task InitializeAsync();

        internal async Task RefreshTokenAsync(TokenResponse token)
        {
            ValidateInProgress();
            InProgress = true;

            RefreshTokenRequestUrl requestUrl = new()
            {
                RefreshToken = token.RefreshToken,
                ClientId     = Options.ClientId,
                ClientSecret = Options.ClientSecret
            };

            using UnityWebRequest webRequest = requestUrl.CreatePostRequest();
            await webRequest.SendWebRequest();
            CheckResponseForErrors(webRequest, "Refresh token");

            try
            {
                RefreshTokenProperties(token, webRequest.downloadHandler.text);
            }
            catch (Exception exception)
            {
                throw new RequestFailedException(CommonErrorCodes.DeserializationError,
                    $"Deserialization error: {exception.Message}");
            }
            finally
            {
                InProgress = false;
            }
        }

        internal async Task RevokeAccessAsync(TokenResponse token)
        {
            if (token.IsEffectivelyExpired() && InProgress == false) await token.RefreshTokenAsync();

            ValidateInProgress();
            InProgress = true;

            RevokeAccessRequestUrl requestUrl = new(token.AccessToken);

            using UnityWebRequest webRequest = requestUrl.CreatePostRequest();
            await webRequest.SendWebRequest();
            CheckResponseForErrors(webRequest, "Revoke access");

            InProgress = false;
        }

        internal async Task<bool> CacheTokenAsync(string userId, TokenResponse tokenResponse)
        {
            ValidateInProgress();
            InProgress = true;

            try
            {
                ITokenStorage tokenStorage = GetTokenStorage();

                if (!IsTokenValid(tokenResponse))
                    return false;

                bool result =
                    await tokenStorage.SaveTokenAsync(userId, tokenResponse.ToJson());
                return result;
            }
            finally
            {
                InProgress = false;
            }
        }

        private ITokenStorage GetTokenStorage()
        {
            ITokenStorage tokenStorage = Options.TokenStorage;
            if (tokenStorage == null)
                throw new NullReferenceException(
                    "To cache a token, you must first set the storage to GoogleAuthOptions.");

            return tokenStorage;
        }

        private bool IsTokenValid(TokenResponse tokenResponse)
        {
            if (string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                Debug.LogWarning("TokenResponse does not contain RefreshToken. There is no point in caching.");
                return false;
            }

            return true;
        }

        private void RefreshTokenProperties(TokenResponse tokenResponse, string json)
        {
            JsonUtility.FromJsonOverwrite(json, tokenResponse);
        }

        private void CheckResponseForErrors(UnityWebRequest tokenRequest, string method)
        {
            if (tokenRequest.result == UnityWebRequest.Result.Success) return;

            RequestFailedException exception;

            try
            {
                ErrorResponse errorResponse =
                    StringSerializationAPI.Deserialize<ErrorResponse>(tokenRequest.downloadHandler.text);
                exception = new RequestFailedException(CommonErrorCodes.ResponseError,
                    $"[{errorResponse.Error}] {method} request failed with error ({errorResponse.ErrorDescription})");
            }
            catch (Exception)
            {
                exception = new RequestFailedException(CommonErrorCodes.ResponseError,
                    $"{method} request failed with error: ({tokenRequest.error}) {tokenRequest.downloadHandler.text}");
            }

            throw exception;
        }
    }
}