using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    public class GoogleAuthOptions
    {
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
            ///     If all specified ports are unavailable, an error message will be displayed.
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
            /// </summary>
            /// <param name="clientId">
            ///     The client ID string that you obtain from
            ///     the API Console Credentials page, as described in
            ///     Obtain OAuth 2.0 credentials.
            /// </param>
            /// <returns></returns>
            public Builder SetCredentials(string clientId)
            {
                _options.ClientId = clientId.ThrowIfNullOrEmpty(
                    new ArgumentException(
                        "Client ID cannot be a null or empty"));

                return this;
            }

            /// <summary>
            /// </summary>
            /// <param name="clientId">
            ///     The client ID string that you obtain from
            ///     the API Console Credentials page, as described in
            ///     Obtain OAuth 2.0 credentials.
            /// </param>
            /// <param name="clientSecret"></param>
            /// <returns></returns>
            public Builder SetCredentials(string clientId, string clientSecret)
            {
                _options.ClientSecret = clientSecret.ThrowIfNullOrEmpty(
                    new ArgumentException(
                        "Client secret cannot be a null or empty"));

                return SetCredentials(clientId);
            }

            public Builder SetCredentials(TextAsset credential)
            {
                fsData                     data = fsJsonParser.Parse(credential.text);
                Dictionary<string, fsData> dic  = data.AsDictionary;

                if (dic.ContainsKey("web"))
                {
                    Dictionary<string, fsData> webDic = dic["web"].AsDictionary;
                    return SetCredentials(webDic["client_id"].AsString,
                        webDic["client_secret"].AsString);
                }

                Dictionary<string, fsData> installedDic = dic["installed"]
                    .AsDictionary ?? throw new NullReferenceException(
                    "The credentials you provided do not contain the Client Id " +
                    "and Client Secret for web or installed app type.");

                _options.ClientId = installedDic["client_id"].AsString.ThrowIfNull(
                    new NullReferenceException(
                        "The credentials you provided do not contain the Client Id"));

                _options.ClientSecret = installedDic["client_secret"].AsString;

                return this;
            }

            public Builder SetAuthorizationCodeFlow(ITokenStorage storage)
            {
                _options.TokenStorage = storage;

                return this;
            }

            public Builder SetResponseHtml(string htmlPage)
            {
                _options.ResponseHtml = htmlPage;

                return this;
            }

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
        }

        #region Fileds and Properties

        internal const string DefaultResponseHtml =
            "<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>";

        public string ClientId { get; private set; }

        public string ClientSecret { get; private set; }

        public bool UseAuthorizationCodeFlow => TokenStorage != null;

        public ReadOnlyCollection<int> Ports => _ports.AsReadOnly();

        public ReadOnlyCollection<string> Scopes => _scopes.AsReadOnly();

        public string ResponseHtml { get; private set; } = DefaultResponseHtml;

        internal ITokenStorage TokenStorage { get; private set; }

        private readonly List<int>    _ports  = new();
        private readonly List<string> _scopes = new();

        #endregion

        private GoogleAuthOptions() { }
    }
}