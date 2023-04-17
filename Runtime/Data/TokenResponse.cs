using System;
using Unity.VisualScripting.FullSerializer;

namespace Izhguzin.GoogleIdentity
{
    [Serializable, fsObject]
    public sealed class TokenResponse
    {
        internal const int TokenRefreshTimeWindowSeconds    = 60 * 6;
        internal const int TokenHardExpiryTimeWindowSeconds = 60 * 5;

        public TokenResponse()
        {
            IssuedUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     The access token issued by the authorization server.
        /// </summary>
        [fsProperty("access_token")]
        public string AccessToken { get; internal set; }

        /// <summary>
        ///     The lifetime in seconds of the access token.
        /// </summary>
        [fsProperty("expires_in")]
        public long ExpiresInSeconds { get; internal set; }

        /// <summary>
        ///     The id_token, which is a JSON Web Token (JWT) as specified in
        ///     http://tools.ietf.org/html/draft-ietf-oauth-json-web-token
        /// </summary>
        [fsProperty("id_token")]
        public string IdToken { get; internal set; }

        /// <summary>
        ///     The refresh token which can be used to obtain a new access token.
        ///     For example, the value "3600" denotes that the access token will expire in one hour from the time the
        ///     response was generated.
        /// </summary>
        [fsProperty("refresh_token")]
        public string RefreshToken { get; internal set; }

        /// <summary>
        ///     The scope of the access token as specified in http://tools.ietf.org/html/rfc6749#section-3.3.
        /// </summary>
        [fsProperty("scope")]
        public string Scope { get; internal set; }

        /// <summary>
        ///     The token type as specified in http://tools.ietf.org/html/rfc6749#section-7.1.
        /// </summary>
        [fsProperty("token_type")]
        public string TokenType { get; internal set; }

        public DateTime IssuedUtc { get; }

        public bool IsExpired()
        {
            return IssuedUtc.AddSeconds(ExpiresInSeconds - TokenRefreshTimeWindowSeconds) <= DateTime.UtcNow;
        }

        public bool IsEffectivelyExpired()
        {
            return IssuedUtc.AddSeconds(ExpiresInSeconds - TokenHardExpiryTimeWindowSeconds) <= DateTime.UtcNow;
        }
    }
}