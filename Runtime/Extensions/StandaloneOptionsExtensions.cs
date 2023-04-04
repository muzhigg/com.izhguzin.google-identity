using System;
using System.Collections.Generic;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    public static class StandaloneOptionsExtensions
    {
        #region Fileds and Properties

        internal const string DefaultResponseHtml =
            "<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>";

        internal const string CodeChallengeGenerationMethodKey = "standalone-use-s256";
        internal const string ListeningTcpPortsKey             = "standalone-ports";
        internal const string ThrowExceptionKey                = "standalone-throw-exception";
        internal const string ClientIdKey                      = "standalone-client-id";
        internal const string ClientSecretKey                  = "standalone-client-secret";
        internal const string ResponseHtmlKey                  = "standalone-response-html";

        #endregion

        public static SignInOptions SetS256GenerationMethodUsage(this SignInOptions options,
            bool                                                                    useS256GenerationMethod)
        {
            return options.SetOption(CodeChallengeGenerationMethodKey, useS256GenerationMethod);
        }

        /// <summary>
        ///     Sets the ports for listening to the authorization response from Google.
        ///     If no ports are specified, a random available port will be used.
        ///     If all specified ports are unavailable, an error message will be displayed.
        /// </summary>
        public static SignInOptions SetListeningTcpPorts(this SignInOptions options, int[] ports)
        {
            List<int> portsList = new();

            foreach (int port in ports)
                switch (port)
                {
                    case > 65535:
                        Debug.LogWarning($"Port {port} not available for use");
                        continue;
                    case < 1023:
                        Debug.LogWarning($"Port {port} are reserved for system services");
                        continue;
                    default:
                        portsList.Add(port);
                        break;
                }

            if (portsList.Count != 0) return options.SetOption(ListeningTcpPortsKey, portsList.ToArray());

            Debug.LogWarning("All specified ports are already in use or not available.");
            return options;
        }

        /// <summary>
        ///     If the value is equal to true, an exception will be thrown when
        ///     requesting authorization and if all ports are occupied. However,
        ///     if the listening ports have not been set, this option will be ignored.
        /// </summary>
        public static SignInOptions SetThrowExceptionOnAllPortsOccupied(this SignInOptions options, bool throwException)
        {
            return options.SetOption(ThrowExceptionKey, throwException);
        }

        public static SignInOptions SetStandaloneCredentials(this SignInOptions options, string clientId,
            string                                                              clientSecret)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                throw new ArgumentException("Client ID and Client secret cannot be null or empty");

            return options.SetOption(ClientIdKey, clientId).SetOption(ClientSecretKey, clientSecret);
        }

        public static SignInOptions SetResponseHtml(this SignInOptions options, string responseHtml)
        {
            if (responseHtml == null) throw new ArgumentNullException(nameof(responseHtml), "Value cannot be null");

            return options.SetOption(ResponseHtmlKey, responseHtml);
        }

        internal static string GetResponseHtml(this SignInOptions options)
        {
            if (!options.TryGetOption(ResponseHtmlKey, out string result)) result = DefaultResponseHtml;
            return result;
        }

        internal static int[] GetListeningTcpPorts(this SignInOptions options)
        {
            options.TryGetOption(ListeningTcpPortsKey, out int[] ports);
            return ports;
        }

        internal static bool GetThrowExceptionOnAllPortsOccupied(this SignInOptions options)
        {
            if (!options.TryGetOption(ThrowExceptionKey, out bool result)) result = true;

            return result;
        }

        internal static string GetClientId(this SignInOptions options)
        {
            if (!options.TryGetOption(ClientIdKey, out string clientId))
                options.TryGetOption(SignInOptions.RootClientIdKey, out clientId);

            return clientId;
        }

        internal static bool UseS256GenerationMethod(this SignInOptions options)
        {
            if (options.TryGetOption(CodeChallengeGenerationMethodKey,
                    out bool useS256GenerationMethod) == false) useS256GenerationMethod = true;

            return useS256GenerationMethod;
        }

        internal static string GetClientSecret(this SignInOptions options)
        {
            if (options.TryGetOption(ClientSecretKey, out string clientSecret) == false)
                options.TryGetOption(SignInOptions.RootClientSecretKey, out clientSecret);

            return clientSecret;
        }
    }
}