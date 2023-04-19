using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable CS4014

namespace Izhguzin.GoogleIdentity
{
    internal sealed class StandaloneSignInClient : GoogleSignInClient
    {
        private readonly struct ProofKey
        {
            public readonly string codeVerifier;
            public readonly string codeChallenge;

            public readonly string codeChallengeMethod;

            public ProofKey(bool useS256GenerationMethod)
            {
                codeVerifier        = PKCECodeProvider.GetRandomBase64URL(32);
                codeChallenge       = PKCECodeProvider.GetCodeChallenge(codeVerifier, useS256GenerationMethod);
                codeChallengeMethod = useS256GenerationMethod ? "S256" : "plain";
            }
        }

        #region Fileds and Properties

        private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";

        private const string PrefsKey = "gsi.user";

        #endregion

        public StandaloneSignInClient(SignInOptions options, OnSuccessCallback onSuccessCallback,
            OnFailureCallback                       onFailureCallback) : base(options,
            onSuccessCallback, onFailureCallback) { }

        public override void BeginSignInOld()
        {
            base.BeginSignInOld();

            if (!CanBeginSignIn()) return;

            ProofKey proofKey = new(options.GetUseS256GenerationMethod());
            string   state    = PKCECodeProvider.GetRandomBase64URL(32);

            try
            {
                if (TryLoadCredential(out UserCredential credential))
                    PerformSignInFromToken(credential);
                else
                    PerformSignIn(proofKey, state);
            }
            catch (GoogleSignInException exception)
            {
                InvokeOnFailureCallback(exception.ErrorCode);
                throw;
            }
        }

        public override void SignOutOld()
        {
            PlayerPrefs.DeleteKey(PrefsKey);
        }

        public override void RefreshTokenOld(UserCredential credential, OnSuccessCallback callback)
        {
            if (!CanBeginSignIn()) return;

            Debug.Log("Refresh");

            RefreshTokenAsync(credential, callback);
        }

        public override void RevokeAccessOld(UserCredential credential)
        {
            if (!CanBeginSignIn()) return;

            RevokeAccessAsync1(credential);
        }

        private SignInAsyncOperation RevokeAccessAsync1(UserCredential credential)
        {
            SignInAsyncOperation asyncOperation = new();
            RevokeAccessAsync(credential, asyncOperation);

            return asyncOperation;
        }

        private async Task RevokeAccessAsync(UserCredential credential, SignInAsyncOperation asyncOperation)
        {
            string tokenRequestBody = $"token={credential.Token.AccessToken}";

            using UnityWebRequest tokenRequest = new("https://oauth2.googleapis.com/revoke", "POST")
            {
                uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(tokenRequestBody))
                {
                    contentType = "application/x-www-form-urlencoded"
                },
                downloadHandler = new DownloadHandlerBuffer()
            };
            await tokenRequest.SendWebRequest();

            asyncOperation.IsDone = true;
        }

        private async Task RefreshTokenAsync(UserCredential credential, OnSuccessCallback callback)
        {
            Debug.LogWarning(credential.Token.RefreshToken);
            string tokenRequestBody =
                $"client_id={GetClientId()}&client_secret={GetClientSecret()}&refresh_token={credential.Token.RefreshToken}&grant_type=refresh_token";

            try
            {
                Debug.Log("Start");
                using UnityWebRequest tokenRequest = new("https://oauth2.googleapis.com/token", "POST")
                {
                    uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(tokenRequestBody))
                    {
                        contentType = "application/x-www-form-urlencoded"
                    },
                    downloadHandler = new DownloadHandlerBuffer()
                };
                Debug.Log("Start2");
                await tokenRequest.SendWebRequest();
                Debug.Log("Start3");
                Debug.Log(tokenRequest.result);
                Debug.Log(tokenRequest.responseCode);
                Debug.Log(tokenRequest.downloadHandler.text);
                HandleTokenResponse(tokenRequest);
                Debug.Log("Start4");
                Debug.Log(tokenRequest.downloadHandler.text);
            }
            catch (Exception exception)
            {
                throw new GoogleSignInException(ErrorCode.ResponseError,
                    $"Failed to refresh authorization code for access token: {exception.Message}", exception);
            }
        }

        private void PerformSignInFromToken(UserCredential credential)
        {
            InvokeOnSuccessCallback(credential);
        }

        private bool CanBeginSignIn()
        {
            if (!UnityMainThread.IsRunningOnMainThread())
            {
                Debug.LogError("Method BeginSignInOld() can only be called from the main thread.");
                return false;
            }

            return true;
        }

        private async Task PerformSignIn(ProofKey proofKey, string state)
        {
            string                     redirectUri = $"http://{IPAddress.Loopback}:{GetAvailablePort()}/";
            StandaloneHttpCodeListener listener    = new(redirectUri, options.GetResponseHtml());
            Application.OpenURL(BuildAuthUrl(redirectUri, proofKey, state));
            try
            {
                await listener.StartAsync(state,
                    code => PerformCodeExchangeAsync(code, proofKey.codeVerifier, redirectUri));
            }
            catch (GoogleSignInException exception)
            {
                InvokeOnFailureCallback(exception.ErrorCode);
                Debug.LogException(exception);
            }
        }

        private int GetAvailablePort()
        {
            return GetUnusedPortFromArray(options.GetListeningTcpPorts());
        }

        private string BuildAuthUrl(string redirectUri, ProofKey proofKey, string state)
        {
            string clientId = GetClientId();

            return $"{AuthorizationEndpoint}?client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                   $"&response_type=code&scope=email%20profile&code_challenge={proofKey.codeChallenge}&code_challenge_method={proofKey.codeChallengeMethod}&state={state}";
        }

        private string GetClientId()
        {
            string clientId = options.GetClientId();

            if (string.IsNullOrEmpty(clientId))
                throw new GoogleSignInException(ErrorCode.DeveloperError,
                    $"Client ID not set in {typeof(SignInOptions)}.");

            return clientId;
        }

        private async Task PerformCodeExchangeAsync(string code, string codeVerifier, string redirectUri)
        {
            string tokenRequestBody =
                $"code={code}&redirect_uri={Uri.EscapeDataString(redirectUri)}&client_id={GetClientId()}&code_verifier={codeVerifier}&client_secret={GetClientSecret()}&scope=&grant_type=authorization_code";

            try
            {
                using UnityWebRequest tokenRequest = new("https://www.googleapis.com/oauth2/v4/token", "POST")
                {
                    uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(tokenRequestBody))
                    {
                        contentType = "application/x-www-form-urlencoded"
                    },
                    downloadHandler = new DownloadHandlerBuffer()
                };

                await tokenRequest.SendWebRequest();
                HandleTokenResponse(tokenRequest);

                TokenResponse response = DeserializeTokenResponse(tokenRequest.downloadHandler.text);
                Debug.Log(tokenRequest.downloadHandler.text);
                InvokeOnSuccessCallback(DeserializeCredential(response));
            }
            catch (Exception exception)
            {
                throw new GoogleSignInException(ErrorCode.ResponseError,
                    $"Failed to exchange authorization code for access token: {exception.Message}", exception);
            }
        }

        private static TokenResponse DeserializeTokenResponse(string token)
        {
            try
            {
                TokenResponse response = StringSerializationAPI.Deserialize<TokenResponse>(token);
                return response;
            }
            catch (Exception exception)
            {
                throw new GoogleSignInException(ErrorCode.Error,
                    $"Error deserializing JSON response: {exception.Message}", exception);
            }
        }

        private static UserCredential DeserializeCredential(TokenResponse response)
        {
            try
            {
                UserCredential credential =
                    StringSerializationAPI.Deserialize<UserCredential>(JwtDecoder.GetPayload(response.IdToken));
                credential.Token = response;

                SaveCredential(credential);
                return credential;
            }
            catch (Exception exception)
            {
                throw new GoogleSignInException(ErrorCode.Error,
                    $"Error deserializing JSON response: {exception.Message}", exception);
            }
        }

        private static void SaveCredential(UserCredential credential)
        {
            string tokenJson    = StringSerializationAPI.Serialize<TokenResponse>(credential.Token);
            string encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenJson));
            PlayerPrefs.SetString(PrefsKey, encodedToken);
        }

        private static bool TryLoadCredential(out UserCredential credential)
        {
            credential = null;

            if (!PlayerPrefs.HasKey(PrefsKey)) return false;

            string encodedString = PlayerPrefs.GetString(PrefsKey);
            string decodedToken  = Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));

            TokenResponse response = DeserializeTokenResponse(decodedToken);
            credential = DeserializeCredential(response);
            return true;
        }

        private void HandleTokenResponse(UnityWebRequest tokenRequest)
        {
            if (tokenRequest.result != UnityWebRequest.Result.Success)
                throw new GoogleSignInException(ErrorCode.ResponseError,
                    $"Token request failed with error: {tokenRequest.error}");

            if (tokenRequest.responseCode != 200)
                throw new GoogleSignInException(ErrorCode.ResponseError,
                    $"Token request failed with status code {tokenRequest.responseCode} and message: {tokenRequest.downloadHandler.text}");
        }

        private string GetClientSecret()
        {
            string clientSecret = options.GetClientSecret();

            if (string.IsNullOrEmpty(clientSecret))
                throw new GoogleSignInException(ErrorCode.DeveloperError,
                    $"Client Secret not set in {typeof(SignInOptions)}.");

            return clientSecret;
        }

        private static int GetUnusedPortFromArray(int[] ports)
        {
            try
            {
                if (ports is not { Length: not 0 }) return GetRandomUnusedPort();

                if (IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
                    .All(endPoint => !ports.Contains(endPoint.Port)))
                    return ports.First();

                throw new NotSupportedException("All specified TCP ports are already busy.");
            }
            catch (Exception e)
            {
                throw new GoogleSignInException(ErrorCode.NetworkError,
                    $"Error occurred in Sign In Client: {e.Message}", e);
            }
        }

        private static int GetRandomUnusedPort()
        {
            try
            {
                TcpListener listener = new(IPAddress.Loopback, 0);
                listener.Start();
                int port = ((IPEndPoint)listener.LocalEndpoint).Port;
                listener.Stop();
                return port;
            }
            catch (SocketException ex)
            {
                throw new Exception("Failed to get a random unused port.", ex);
            }
        }
    }
}