using System;
using Izhguzin.GoogleIdentity.Standalone;

namespace Izhguzin.GoogleIdentity
{
    internal class TokenRequestUrl : RequestUrl
    {
        #region Fileds and Properties

        [RequestParameter("code", true)] public string Code { get; set; }

        [RequestParameter("redirect_uri", true)]
        public string RedirectUri { get; set; }

        [RequestParameter("client_id", true)] public string ClientId { get; set; }

        [RequestParameter("code_verifier", true)]
        public string CodeVerifier { get; set; }

        [RequestParameter("client_secret", true)]
        public string ClientSecret { get; set; }

        [RequestParameter("grant_type", true)] public string GrantType { get; set; } = "authorization_code";

        public override string EndPointUrl => GoogleAuthConstants.TokenUrl;

        #endregion

        /// <exception cref="NullReferenceException"></exception>
        public TokenRequestUrl(StandaloneSignInOptions options)
        {
            ClientId = options.ClientId.ThrowIfNullOrEmpty(
                new NullReferenceException($"Client ID not set in {typeof(SignInOptions)}."));

            ClientSecret =
                options.ClientSecret.ThrowIfNullOrEmpty(
                    new NullReferenceException($"Client secret not set in {typeof(SignInOptions)}."));
        }
    }
}