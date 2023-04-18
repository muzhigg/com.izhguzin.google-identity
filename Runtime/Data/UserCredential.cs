using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    [Serializable, fsObject]
    public sealed class UserCredential
    {
        public string GetJWT()
        {
            return Token.IdToken;
        }

        public bool IsExpired()
        {
            DateTimeOffset expiresAt = DateTimeOffset.FromUnixTimeSeconds(ExpirationTime);
            Debug.Log($"ff: {Token.IsExpired()}");
            return DateTimeOffset.UtcNow > expiresAt;
        }

        #region Fileds and Properties

        /// <summary>
        ///     Who created and signed this token.
        /// </summary>
        [fsProperty("iss")]
        public string Issuer { get; internal set; }

        /// <summary>
        ///     The party to which this token was issued.
        /// </summary>
        [fsProperty("azp")]
        public string AuthorizedParty { get; internal set; }

        /// <summary>
        ///     Who and what the token is intended for.
        /// </summary>
        [fsProperty("aud")]
        public string Audience { get; internal set; }

        /// <summary>
        ///     Whom the token refers to.
        /// </summary>
        [fsProperty("sub")]
        public string Subject { get; internal set; }

        [fsProperty("email")] public string Email { get; internal set; }

        [fsProperty("email_verified")] public bool EmailVerified { get; internal set; }

        [fsProperty("name")] public string Name { get; internal set; }

        [fsProperty("picture")] public string PictureUrl { get; internal set; }

        [fsProperty("given_name")] public string GivenName { get; internal set; }

        [fsProperty("family_name")] public string FamilyName { get; internal set; }

        [fsProperty("locale")] public string Locale { get; internal set; }

        /// <summary>
        ///     Seconds since Unix epoch.
        /// </summary>
        [fsProperty("iat")]
        public long IssuedAt { get; internal set; }

        /// <summary>
        ///     Seconds since Unix epoch.
        /// </summary>
        [fsProperty("exp")]
        public long ExpirationTime { get; internal set; }

        internal TokenResponse Token { get; set; }

        #endregion
    }
}