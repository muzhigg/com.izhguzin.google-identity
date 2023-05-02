﻿using System;

namespace Izhguzin.GoogleIdentity
{
    public class AndroidSignInOptions
    {
        public sealed class Builder : IOptionsBuilder
        {
            #region Fileds and Properties

            private readonly SignInOptions        _rootOptions;
            private readonly AndroidSignInOptions _options;

            #endregion

            internal Builder(SignInOptions options)
            {
                _rootOptions = options;
                _options     = new AndroidSignInOptions();
                _rootOptions.AddAndroidOptions(_options);
            }

            public SignInOptions Build()
            {
                return _rootOptions;
            }

            /// <summary>
            ///     Adds standalone platform options to the sign-in options. If you have already added options for this platform
            ///     before, builder will overwrite them.
            /// </summary>
            /// <returns>The current instance of the builder.</returns>
            public StandaloneSignInOptions.Builder AddStandaloneOptions()
            {
                return new StandaloneSignInOptions.Builder(_rootOptions);
            }

            /// <summary>
            ///     Sets the Client Id for the sign-in request on Android devices. Use the Client Id of the web application type.
            /// </summary>
            /// <param name="webClientId"></param>
            /// <returns>The current instance of the builder.</returns>
            /// <exception cref="ArgumentException"></exception>
            public Builder SetWebClientId(string webClientId)
            {
                if (string.IsNullOrEmpty(webClientId))
                    throw new ArgumentException("Client ID cannot be null or empty");

                _options.WebClientId = webClientId;
                return this;
            }

            public Builder SetWebClientSecret(string clientSecret)
            {
                _options.WebClientSecret =
                    clientSecret.ThrowIfNullOrEmpty(new ArgumentException("Client Secret cannot be null or empty"));
                return this;
            }

            /// <summary>
            ///     Sets the type of token to receive from the Google service. If set to true, the received JWToken cannot be
            ///     refreshed.
            /// </summary>
            public Builder SetUseOneTimeToken(bool useOneTimeToken)
            {
                _options.UseOneTimeToken = useOneTimeToken;
                return this;
            }
        }

        #region Fileds and Properties

        public string WebClientId { get; private set; }

        public string WebClientSecret { get; private set; }

        public bool UseOneTimeToken { get; private set; }

        #endregion

        internal AndroidSignInOptions() { }
    }
}