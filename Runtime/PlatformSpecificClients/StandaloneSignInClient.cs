using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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

        private StandaloneHttpCodeListener _listener;
        private bool                       _inProgress;

        #endregion

        public StandaloneSignInClient(SignInOptions options, Action<UserCredential> onSuccessCallback,
            Action                                  onFailureCallback) : base(options,
            onSuccessCallback, onFailureCallback) { }

        public override bool InProgress()
        {
            return _inProgress;
        }

        public override void BeginSignIn()
        {
            if (!CanBeginSignIn()) return;

            _inProgress = true;
            ProofKey proofKey = new(options.UseS256GenerationMethod());
            string   state    = PKCECodeProvider.GetRandomBase64URL(32);

            try
            {
                PerformSignIn(proofKey, state);
            }
            catch (GoogleSignInException)
            {
                InvokeOnFailureCallback();
                throw;
            }
        }

        private void InvokeOnFailureCallback()
        {
            _inProgress = false;
            onFailureCallback.Invoke();
        }

        private bool CanBeginSignIn()
        {
            if (_inProgress)
            {
                Debug.LogError("Sign-in process already in progress");
                return false;
            }

            if (!UnityMainThread.IsRunningOnMainThread())
            {
                Debug.LogError("Method BeginSignIn() can only be called from the main thread.");
                return false;
            }

            return true;
        }

        private async Task PerformSignIn(ProofKey proofKey, string state)
        {
            string redirectUri = $"http://{IPAddress.Loopback}:{GetAvailablePort()}/";
            Debug.Log(redirectUri);
            StandaloneHttpCodeListener listener = new(redirectUri, options.GetResponseHtml());
            Application.OpenURL(BuildAuthUrl(redirectUri, proofKey, state));
            try
            {
                await listener.StartAsync(state,
                    code => PerformCodeExchangeAsync(code, proofKey.codeVerifier, redirectUri));
            }
            catch (GoogleSignInException exception)
            {
                InvokeOnFailureCallback();
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
                throw new GoogleSignInException(
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

                InvokeOnSuccessCallback(GetCredential(tokenRequest.downloadHandler.text));
            }
            catch (Exception exception)
            {
                throw new GoogleSignInException(
                    $"Failed to exchange authorization code for access token: {exception.Message}", exception);
            }
        }

        private void InvokeOnSuccessCallback(UserCredential credential)
        {
            _inProgress = false;
            onSuccessCallback.Invoke(credential);
        }

        private void HandleTokenResponse(UnityWebRequest tokenRequest)
        {
            if (tokenRequest.result != UnityWebRequest.Result.Success)
                throw new GoogleSignInException($"Token request failed with error: {tokenRequest.error}");

            if (tokenRequest.responseCode != 200)
                throw new GoogleSignInException(
                    $"Token request failed with status code {tokenRequest.responseCode} and message: {tokenRequest.downloadHandler.text}");
        }

        private static UserCredential GetCredential(string jsonResponse)
        {
            try
            {
                TokenResponse response = StringDeserializationAPI.Deserialize<TokenResponse>(jsonResponse);
                UserCredential credential =
                    StringDeserializationAPI.Deserialize<UserCredential>(JwtDecoder.GetPayload(response.IdToken));
                credential.Token = response;
                return credential;
            }
            catch (Exception exception)
            {
                throw new GoogleSignInException($"Error deserializing JSON response: {exception.Message}", exception);
            }
        }

        private string GetClientSecret()
        {
            string clientSecret = options.GetClientSecret();

            if (string.IsNullOrEmpty(clientSecret))
                throw new GoogleSignInException(
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
                throw new GoogleSignInException($"Error occurred in Sign In Client: {e.Message}");
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

#if UNITY_STANDALONE_WIN

        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

#endif
    }
}