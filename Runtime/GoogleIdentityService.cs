using System;
using System.Text;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Standalone;
using Izhguzin.GoogleIdentity.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     Entry class to the Authentication Service.
    /// </summary>
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

        #endregion

        protected GoogleIdentityService(GoogleAuthOptions options)
        {
            Options = options;
        }

        public abstract Task<TokenResponse> Authorize();

        public async Task<TokenResponse> GetCachedTokenAsync(string userId)
        {
            if (Options.TokenStorage == null)
            {
                Debug.LogWarning("To get a cached token, you must first set the storage to GoogleAuthOptions.");
                return null;
            }

            string tokenJson = await Options.TokenStorage.LoadTokenAsync(userId);

            if (string.IsNullOrEmpty(tokenJson))
            {
                Debug.LogWarning("TokenStorage did not return a json string.");
                return null;
            }

            TokenResponse response = TokenResponse.FromJson(tokenJson);
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

            using UnityWebRequest webRequest = CreatePostRequest(tokenRequestUrl);
            await webRequest.SendWebRequest();

            try
            {
                CheckResponseForErrors(webRequest, "Token exchange");
            }
            catch (RequestFailedException exception)
            {
                throw new AuthorizationFailedException(CommonErrorCodes.ResponseError, exception.Message);
            }

            try
            {
                return TokenResponse.FromJson(webRequest.downloadHandler.text);
            }
            catch (Exception exception)
            {
                throw new AuthorizationFailedException(CommonErrorCodes.DeserializationError,
                    $"Deserialization error: {exception.Message}");
            }
        }

        internal abstract Task InitializeAsync();

        internal async Task RefreshTokenAsync(TokenResponse token)
        {
            RefreshTokenRequestUrl requestUrl = new()
            {
                RefreshToken = token.RefreshToken,
                ClientId     = Options.ClientId,
                ClientSecret = Options.ClientSecret
            };

            using UnityWebRequest webRequest = CreatePostRequest(requestUrl);
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
        }

        internal async Task RevokeAccessAsync(TokenResponse token)
        {
            if (token.IsEffectivelyExpired()) await token.RefreshTokenAsync();

            RevokeAccessRequestUrl requestUrl = new(token.AccessToken);

            using UnityWebRequest webRequest = CreatePostRequest(requestUrl);
            await webRequest.SendWebRequest();
            CheckResponseForErrors(webRequest, "Revoke access");
        }

        internal async Task<bool> CacheTokenAsync(string userId, TokenResponse tokenResponse)
        {
            if (Options.TokenStorage == null)
            {
                Debug.LogWarning("To cache a token, you must first set the storage to GoogleAuthOptions.");
                return false;
            }

            if (string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                Debug.LogWarning("TokenResponse does not contain RefreshToken. There is no point in caching.");
                return false;
            }

            await Options.TokenStorage.SaveTokenAsync(userId, StringSerializationAPI.Serialize(tokenResponse));
            return true;
        }

        private void RefreshTokenProperties(TokenResponse tokenResponse, string json)
        {
            TokenResponse newResponse = TokenResponse.FromJson(json);
            tokenResponse.IdToken          = newResponse.IdToken;
            tokenResponse.AccessToken      = newResponse.AccessToken;
            tokenResponse.ExpiresInSeconds = newResponse.ExpiresInSeconds;
            tokenResponse.IssuedUtc        = newResponse.IssuedUtc;
            tokenResponse.Scope            = newResponse.Scope;
            tokenResponse.TokenType        = newResponse.TokenType;
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
                throw new RequestFailedException(CommonErrorCodes.ResponseError,
                    $"{method} request failed with error: ({tokenRequest.error}) {tokenRequest.downloadHandler.text}");

            if (tokenRequest.responseCode != 200)
                throw new RequestFailedException(CommonErrorCodes.ResponseError,
                    $"{method} request failed with status code {tokenRequest.responseCode} and message: {tokenRequest.downloadHandler.text}");
        }
    }
}