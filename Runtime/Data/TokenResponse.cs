using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.JWTDecoder;
using Izhguzin.GoogleIdentity.Utils;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     This class represents a response to an authorization
    ///     token request in the Google API. It contains information
    ///     about the issued token, its expiration time, token type,
    ///     scope, and other parameters. The class also includes methods
    ///     for refreshing the token, revoking access, and caching the token.
    /// </summary>
    [Serializable]
    public sealed class TokenResponse
    {
        internal const int TokenRefreshTimeWindowSeconds    = 60 * 6;
        internal const int TokenHardExpiryTimeWindowSeconds = 60 * 5;

        [SerializeField] private string access_token;

        [SerializeField] private long expires_in;

        [SerializeField] private string id_token;

        [SerializeField] private string refresh_token;

        [SerializeField] private string scope;

        [SerializeField] private string token_type;

        [SerializeField] private long iss;

        internal TokenResponse()
        {
            IssuedUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     The access token issued by the authorization server.
        /// </summary>
        public string AccessToken
        {
            get => access_token;
            internal set => access_token = value;
        }

        /// <summary>
        ///     The lifetime in seconds of the access token.
        /// </summary>
        public long ExpiresInSeconds
        {
            get => expires_in;
            internal set => expires_in = value;
        }

        /// <summary>
        ///     The id_token, which is a JSON Web Token (JWT) as specified in
        ///     http://tools.ietf.org/html/draft-ietf-oauth-json-web-token
        /// </summary>
        public string IdToken
        {
            get => id_token;
            internal set => id_token = value;
        }

        /// <summary>
        ///     The refresh token which can be used to obtain a new access token.
        ///     For example, the value "3600" denotes that the access token will expire in one hour from the time the
        ///     response was generated.
        /// </summary>
        public string RefreshToken
        {
            get => refresh_token;
            internal set => refresh_token = value;
        }

        /// <summary>
        ///     The scope of the access token as specified in http://tools.ietf.org/html/rfc6749#section-3.3.
        /// </summary>
        public string Scope
        {
            get => scope;
            internal set => scope = value;
        }

        /// <summary>
        ///     The token type as specified in http://tools.ietf.org/html/rfc6749#section-7.1.
        /// </summary>
        public string TokenType
        {
            get => token_type;
            internal set => token_type = value;
        }

        public DateTime IssuedUtc
        {
            get => DateTimeOffset.FromUnixTimeSeconds(iss).DateTime;
            internal set => iss = ((DateTimeOffset)value.ToUniversalTime()).ToUnixTimeSeconds();
        }

        /// <summary>
        ///     Checks if the token has expired by comparing
        ///     the current time with the token's expiration time.
        ///     If the token has expired, the method returns true.
        /// </summary>
        public bool IsExpired()
        {
            return IssuedUtc.AddSeconds(ExpiresInSeconds - TokenRefreshTimeWindowSeconds) <= DateTime.UtcNow;
        }

        /// <summary>
        ///     Checks if the token has effectively expired by
        ///     comparing the current time plus a grace period
        ///     with the token's expiration time. If the token
        ///     has effectively expired, the method returns true.
        /// </summary>
        public bool IsEffectivelyExpired()
        {
            return IssuedUtc.AddSeconds(ExpiresInSeconds - TokenHardExpiryTimeWindowSeconds) <= DateTime.UtcNow;
        }

        /// <summary>
        ///     Retrieves the user credentials associated with the token. It creates a new UserCredential object using the token's
        ///     access token and the Google API's client secrets.
        /// </summary>
        public UserCredential GetUserCredential()
        {
            if (string.IsNullOrEmpty(IdToken)) return null;

            UserCredential result = new();
            JsonUtility.FromJsonOverwrite(Decoder.DecodeToken(IdToken).Payload, result);
            return result;

            //return Decoder.DecodePayload<UserCredential>(IdToken);
        }

        /// <summary>
        ///     Refreshes the token by sending a request to the Google OAuth2 server. It updates the current TokenResponse object
        ///     with the new token information.
        /// </summary>
        /// <exception cref="RequestFailedException"></exception>
        public async Task RefreshTokenAsync()
        {
            if (string.IsNullOrEmpty(RefreshToken))
                throw new RequestFailedException(CommonErrorCodes.Error,
                    "An error occurred while refreshing the token. TokenResponse does not contain RefreshToken. To retrieve it, revoke access and authorize again.");

            await GoogleIdentityService.Instance
                .RefreshTokenAsync(this);
        }

        /// <summary>
        ///     Revokes the access token by sending a request
        ///     to the Google OAuth2 server. It invalidates the token.
        /// </summary>
        public async Task RevokeAccessAsync()
        {
            await GoogleIdentityService.Instance
                .RevokeAccessAsync(this);
        }

        /// <summary>
        ///     Saves the token by storing it in the storage specified by the developer.
        /// </summary>
        /// <param name="userId">Unique user identifier, use this parameter as a key.</param>
        /// <returns>It returns a boolean value indicating whether the token was successfully saved.</returns>
        public async Task<bool> StoreAsync(string userId)
        {
            return await GoogleIdentityService.Instance.CacheTokenAsync(userId, this);
        }

        /// <exception cref="JsonDeserializationException"></exception>
        internal static TokenResponse FromJson(string json)
        {
            try
            {
                TokenResponse response = new();
                JsonUtility.FromJsonOverwrite(json, response);
                //TokenResponse response = StringSerializationAPI.Deserialize<TokenResponse>(json);

                if (string.IsNullOrEmpty(response.AccessToken))
                    throw new NullReferenceException(
                        "The response from the server is not complete. AccessToken have a null value.");

                return response;
            }
            catch (Exception exception)
            {
                throw new JsonDeserializationException(exception.Message, exception);
            }
        }

        internal string ToJson()
        {
            return StringSerializationAPI.Serialize(this);
        }
    }
}