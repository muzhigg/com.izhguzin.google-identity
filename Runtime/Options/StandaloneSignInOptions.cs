using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    public sealed class StandaloneSignInOptions
    {
        public sealed class Builder : IOptionsBuilder
        {
            #region Fileds and Properties

            private readonly SignInOptions           _rootOptions;
            private readonly StandaloneSignInOptions _standaloneOptions;

            #endregion

            internal Builder(SignInOptions options)
            {
                _rootOptions       = options;
                _standaloneOptions = new StandaloneSignInOptions();
                _rootOptions.AddStandaloneOptions(_standaloneOptions);
            }

            public SignInOptions Build()
            {
                return _rootOptions;
            }

            /// <summary>
            ///     Adds Android platform options to the sign in options. If you have already added options for this platform before,
            ///     builder will overwrite them.
            /// </summary>
            /// <returns>The current instance of the builder.</returns>
            public AndroidSignInOptions.Builder AddAndroidOptions()
            {
                return new AndroidSignInOptions.Builder(_rootOptions);
            }

            /// <summary>
            ///     Sets the ports for listening to the authorization response from Google.
            ///     If no ports are specified, a random available port will be used.
            ///     If all specified ports are unavailable, an error message will be displayed.
            /// </summary>
            /// <returns>The current instance of the builder.</returns>
            public Builder SetListeningTcpPorts(int[] ports)
            {
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
                            _standaloneOptions._ports.Add(port);
                            break;
                    }

                if (_standaloneOptions._ports.Count == 0)
                    Debug.LogWarning("All specified ports are already in use or not available.");

                return this;
            }

            /// <summary>
            ///     Sets the Client Id and Client secret for the sign-in request on Standalone platforms. Use the Client Id of the web
            ///     application type.
            /// </summary>
            /// <param name="clientId"></param>
            /// <param name="clientSecret"></param>
            /// <returns>The current instance of the builder.</returns>
            /// <exception cref="ArgumentException"></exception>
            public Builder SetClientCredentials(string clientId, string clientSecret)
            {
                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                    throw new ArgumentException("Client ID and Client secret cannot be null or empty");

                _standaloneOptions.ClientId     = clientId;
                _standaloneOptions.ClientSecret = clientSecret;

                return this;
            }

            /// <summary>
            ///     Sets the html page when redirected from the google service.
            ///     The user will see this page after selecting an account.
            ///     Instead of html code it is possible to pass plain text.
            /// </summary>
            /// <param name="html"></param>
            /// <returns>The current instance of the builder.</returns>
            /// <exception cref="ArgumentNullException"></exception>
            public Builder SetResponseHtml(string html)
            {
                _standaloneOptions.ResponseHtml =
                    html ?? throw new ArgumentNullException(nameof(html), "Value cannot be null");

                return this;
            }
        }

        #region Fileds and Properties

        internal const string DefaultResponseHtml =
            "<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>";

        public bool UseS256GenerationMethod => true;

        public ReadOnlyCollection<int> Ports => _ports.AsReadOnly();

        public string ClientId { get; private set; }

        public string ClientSecret { get; private set; }

        public string ResponseHtml { get; private set; } = DefaultResponseHtml;

        private readonly List<int> _ports = new();

        #endregion

        internal StandaloneSignInOptions() { }
    }
}