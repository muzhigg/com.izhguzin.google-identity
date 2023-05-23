using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     The GoogleAuthOptions class represents the options for configuring Google authentication.
    /// </summary>
    public class GoogleAuthOptions
    {
        /// <summary>
        ///     A nested class that provides a fluent API for constructing instances of GoogleAuthOptions.
        /// </summary>
        public class Builder
        {
            #region Fileds and Properties

            private readonly GoogleAuthOptions _options;

            #endregion

            public Builder()
            {
                _options = new GoogleAuthOptions();
            }

            public GoogleAuthOptions Build()
            {
                return _options;
            }

            /// <summary>
            ///     Sets the ports for listening to the authorization response from Google.
            ///     If no ports are specified, a random available port will be used.
            ///     If all specified ports are unavailable, it will throw NotSupportedException.
            ///     <para>This option has an effect only on the standalone platform.</para>
            /// </summary>
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
                            _options._ports.Add(port);
                            break;
                    }

                if (_options._ports.Count == 0)
                    Debug.LogWarning("All specified ports are already in use or not available.");

                return this;
            }

            /// <summary>
            ///     Sets the client ID string and client secret string that you obtain from the API Console Credentials page. This is
            ///     required for OAuth 2.0 authentication.
            /// </summary>
            public Builder SetCredentials(string clientId, string clientSecret)
            {
                _options.ClientSecret = clientSecret.ThrowIfNullOrEmpty(
                    "Client secret cannot be a null or empty");

                return SetCredentials(clientId);
            }

            /// <summary>
            ///     Sets the token storage implementation to be used for storing and retrieving access tokens. This method allows you
            ///     to set a custom implementation of the ITokenStorage interface, which provides the functionality to save and load
            ///     access tokens.
            /// </summary>
            public Builder SetTokenStorage(ITokenStorage storage)
            {
                _options.TokenStorage = storage;

                return this;
            }

            /// <summary>
            ///     Sets the HTML content of the response page that is displayed after completing the authorization process. The
            ///     default response HTML is a simple page that refreshes and redirects to Google. This method allows you to customize
            ///     the HTML content of the response page displayed to the user.
            /// </summary>
            /// <param name="htmlPage"></param>
            /// <returns></returns>
            public Builder SetResponseHtml(string htmlPage)
            {
                _options.ResponseHtml = htmlPage;

                return this;
            }

            /// <summary>
            ///     Sets the scopes representing the requested permissions for accessing Google APIs. This method allows you to specify
            ///     the scopes required for accessing Google APIs.
            /// </summary>
            public Builder SetScopes(params string[] scopes)
            {
                foreach (string scope in scopes.Where(scope => !_options._scopes.Contains(scope)))
                {
                    if (NeedOpenIdScope(scope)
                        && !_options._scopes.Contains(GoogleIdentity.Scopes.OpenId))
                        _options._scopes.Insert(0, GoogleIdentity.Scopes.OpenId);

                    _options._scopes.Add(scope);
                }

                return this;

                bool NeedOpenIdScope(string scope)
                {
                    return scope is GoogleIdentity.Scopes.Email
                        or GoogleIdentity.Scopes.Profile;
                }
            }

            private Builder SetCredentials(string clientId)
            {
                _options.ClientId = clientId.ThrowIfNullOrEmpty(
                    "Client ID cannot be a null or empty");

                return this;
            }
        }

        #region Fileds and Properties

        internal const string DefaultResponseHtml =
            "<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>";

        /// <summary>
        ///     Gets the client ID string obtained from the API Console Credentials page. This is required for OAuth 2.0
        ///     authentication.
        /// </summary>
        public string ClientId { get; private set; }

        /// <summary>
        ///     Gets the client secret string obtained from the API Console Credentials page. This is required for OAuth 2.0
        ///     authentication.
        /// </summary>
        public string ClientSecret { get; private set; }

        /// <summary>
        ///     Gets a read-only collection of ports for listening to the authorization response from Google. If no ports are
        ///     specified, a random available port will be used. This option only applies to the standalone platform.
        /// </summary>
        public ReadOnlyCollection<int> Ports => _ports.AsReadOnly();

        /// <summary>
        ///     Gets a read-only collection of scopes representing the requested permissions for accessing Google APIs.
        /// </summary>
        public ReadOnlyCollection<string> Scopes => _scopes.AsReadOnly();

        /// <summary>
        ///     Gets the HTML content of the response page displayed after completing the authorization process. The default value
        ///     is a simple HTML page that refreshes and redirects to Google.
        /// </summary>
        public string ResponseHtml { get; private set; } = DefaultResponseHtml;

        internal ITokenStorage TokenStorage { get; private set; }

        private readonly List<int>    _ports  = new();
        private readonly List<string> _scopes = new();

        #endregion

        private GoogleAuthOptions() { }
    }
}