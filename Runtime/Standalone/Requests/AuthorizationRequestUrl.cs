using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Izhguzin.GoogleIdentity.Standalone
{
    internal class AuthorizationRequestUrl : RequestUrl
    {
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
            catch (SocketException)
            {
                throw new Exception("Failed to get a random unused port.");
            }
        }

        internal readonly struct ProofKey
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

        [RequestParameter("response_type", true)]
        // https://developers.google.com/identity/openid-connect/openid-connect#prompt
        public string ResponseType { get; set; } // set "code" to AuthCodeFlow, set "token id_token" to jwt

        [RequestParameter("client_id", true)] public string ClientId { get; set; }

        [RequestParameter("redirect_uri", true)]
        public string RedirectUri { get; set; }

        [RequestParameter("scope", true)] public string Scope { get; set; }

        [RequestParameter("code_challenge", false)]
        public string CodeChallenge { get; set; }

        [RequestParameter("code_challenge_method", false)]
        public string CodeChallengeMethod { get; set; }

        [RequestParameter("state", false)] public string State { get; set; }

        [RequestParameter("prompt", false)] public string Prompt { get; set; }

        [RequestParameter("access_type", false)]
        public string AccessType { get; set; } // set offline to receive refresh token

        public ProofKey ProofCodeKey { get; set; }

        public override string EndPointUrl => GoogleAuthConstants.AuthorizationUrl;

        #endregion

        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="GoogleSignInException"></exception>
        public AuthorizationRequestUrl(StandaloneSignInOptions optionsStandalone)
        {
            optionsStandalone ??= new StandaloneSignInOptions();

            ClientId = optionsStandalone.ClientId.ThrowIfNullOrEmpty(
                new NullReferenceException($"Client ID not set in {typeof(GoogleAuthOptions)}."));
            RedirectUri         = $"http://{IPAddress.Loopback}:{GetAvailablePort(optionsStandalone)}/";
            ProofCodeKey        = new ProofKey(optionsStandalone.UseS256GenerationMethod);
            CodeChallenge       = ProofCodeKey.codeChallenge;
            CodeChallengeMethod = ProofCodeKey.codeChallengeMethod;
            State               = PKCECodeProvider.GetRandomBase64URL(32);
        }

        private int GetAvailablePort(StandaloneSignInOptions optionsStandalone)
        {
            return GetUnusedPortFromArray(optionsStandalone.Ports);
        }

        private int GetUnusedPortFromArray(ICollection<int> readOnlyCollection)
        {
            try
            {
                if (readOnlyCollection.Count == 0) return GetRandomUnusedPort();

                if (IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
                    .All(endPoint => !readOnlyCollection.Contains(endPoint.Port)))
                    return readOnlyCollection.First();

                throw new NotSupportedException("All specified TCP ports are already busy.");
            }
            catch (Exception e)
            {
                throw new GoogleSignInException(CommonStatus.NetworkError,
                    $"Error occurred in Sign In Client: {e.Message}");
            }
        }
    }
}