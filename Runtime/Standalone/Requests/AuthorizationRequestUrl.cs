using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

namespace Izhguzin.GoogleIdentity.Standalone
{
    // https://developers.google.com/identity/openid-connect/openid-connect#prompt
    internal class AuthorizationRequestUrl : RequestUrl
    {
        public static AuthorizationRequestUrl CreateDefaultFromOptions(GoogleAuthOptions options)
        {
            AuthorizationRequestUrl requestUrl = new()
            {
                ClientId    = options.ClientId,
                RedirectUri = $"http://{IPAddress.Loopback}:{GetAvailablePort(options)}/",
                Scope       = string.Join(' ', options.Scopes),
                State       = PKCECodeProvider.GetRandomBase64URL(32)
            };

            return requestUrl;
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
            catch (SocketException)
            {
                throw new Exception("Failed to get a random unused port.");
            }
        }

        private static int GetAvailablePort(GoogleAuthOptions options)
        {
            return GetUnusedPortFromArray(options.Ports);
        }

        private static int GetUnusedPortFromArray(ICollection<int> readOnlyCollection)
        {
            if (readOnlyCollection.Count == 0) return GetRandomUnusedPort();

            List<int> ports = new(readOnlyCollection);

            foreach (IPEndPoint activeTcpListener in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners())
                if (ports.Contains(activeTcpListener.Port))
                    ports.Remove(activeTcpListener.Port);

            if (ports.Count != 0)
            {
                Debug.Log(ports.First());
                return ports.First();
            }

            //if (IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
            //    .All(endPoint => !readOnlyCollection.Contains(endPoint.Port)))
            //    return readOnlyCollection.First();

            throw new NotSupportedException("All specified TCP ports are already busy.");
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
        public string ResponseType { get; set; }

        [RequestParameter("nonce", false)] public string Nonce { get; set; }

        [RequestParameter("client_id", true)] public string ClientId { get; set; }

        [RequestParameter("redirect_uri", true)]
        public string RedirectUri { get; set; }

        [RequestParameter("scope", true)] public string Scope { get; set; }

        [RequestParameter("code_challenge", false)]
        public string CodeChallenge => ProofCodeKey.codeChallenge;

        [RequestParameter("code_challenge_method", false)]
        public string CodeChallengeMethod => CodeChallenge == null ? null : ProofCodeKey.codeChallengeMethod;

        [RequestParameter("state", false)] public string State { get; set; }

        [RequestParameter("prompt", false)] public string Prompt { get; set; }

        [RequestParameter("access_type", false)]
        public string AccessType { get; set; } // set offline to receive refresh token

        [RequestParameter("display", false)] public string Display { get; set; }

        public ProofKey ProofCodeKey { get; set; }

        public override string EndPointUrl => GoogleAuthConstants.AuthorizationUrl;

        #endregion
    }
}