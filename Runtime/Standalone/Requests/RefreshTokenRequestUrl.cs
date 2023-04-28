using System;

namespace Izhguzin.GoogleIdentity.Standalone
{
    internal class RefreshTokenRequestUrl : RequestUrl
    {
        #region Fileds and Properties

        public override string EndPointUrl => GoogleAuthConstants.TokenUrl;

        [RequestParameter("client_id", true)] public string ClientId { get; set; }

        [RequestParameter("client_secret", true)]
        public string ClientSecret { get; set; }

        [RequestParameter("grant_type", true)] public string GrantType => "refresh_token";

        [RequestParameter("refresh_token", true)]
        public string RefreshToken { get; set; }

        #endregion

        /// <exception cref="NullReferenceException"></exception>
        public RefreshTokenRequestUrl(StandaloneSignInOptions options, string refreshToken)
        {
            ClientId = options.ClientId.ThrowIfNullOrEmpty(
                new NullReferenceException("Client Id is not set in SignInOptions"));
            ClientSecret =
                options.ClientSecret.ThrowIfNullOrEmpty(
                    new NullReferenceException("Client secret is not set it SignInOptions"));
            RefreshToken = refreshToken;
        }
    }
}