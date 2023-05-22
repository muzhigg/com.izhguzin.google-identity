using System;
using Izhguzin.GoogleIdentity.JWTDecoder;
using Unity.VisualScripting.FullSerializer;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     The UserCredential class represents user credentials
    ///     obtained from a Google authentication token.
    ///     It contains various fields and properties that
    ///     store information about the user.
    /// </summary>
    [Serializable, fsObject]
    public sealed class UserCredential
    {
        internal UserCredential() { }

        /// <summary>
        ///     Determines if the user credentials have expired.
        /// </summary>
        /// <returns>It returns true if the current date and time exceed the expiration time specified in the credentials.</returns>
        public bool IsExpired()
        {
            DateTimeOffset expiresAt = DateTimeOffset.FromUnixTimeSeconds(ExpirationTime);
            return DateTimeOffset.UtcNow > expiresAt;
        }

        /// <summary>
        ///     Returns a string representation of the user credentials.
        /// </summary>
        /// <returns>It retrieves the payload of the ID token and returns it as a string.</returns>
        public override string ToString()
        {
            string result = Decoder.DecodeToken(Token.IdToken).Payload;
            return result;
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

        /// <summary>
        ///     Represents the email address of the user.
        /// </summary>
        [fsProperty("email")]
        public string Email { get; internal set; }

        /// <summary>
        ///     Indicates whether the user's email address has been verified.
        /// </summary>
        [fsProperty("email_verified")]
        public bool EmailVerified { get; internal set; }

        /// <summary>
        ///     Represents the name of the user.
        /// </summary>
        [fsProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        ///     Represents the URL of the user's profile picture.
        /// </summary>
        [fsProperty("picture")]
        public string PictureUrl { get; internal set; }

        /// <summary>
        ///     Represents the given name (first name) of the user.
        /// </summary>
        [fsProperty("given_name")]
        public string GivenName { get; internal set; }

        /// <summary>
        ///     Represents the family name (last name) of the user.
        /// </summary>
        [fsProperty("family_name")]
        public string FamilyName { get; internal set; }

        /// <summary>
        ///     Represents the locale of the user.
        /// </summary>
        [fsProperty("locale")]
        public string Locale { get; internal set; }

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