using System;
using Izhguzin.GoogleIdentity.JWTDecoder;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     The UserCredential class represents user credentials
    ///     obtained from a Google authentication token.
    ///     It contains various fields and properties that
    ///     store information about the user.
    /// </summary>
    [Serializable]
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

        [SerializeField] private string iss;
        [SerializeField] private string azp;
        [SerializeField] private string sub;
        [SerializeField] private string email;
        [SerializeField] private bool   email_verified;
        [SerializeField] private string name;
        [SerializeField] private string picture;
        [SerializeField] private string given_name;
        [SerializeField] private string family_name;
        [SerializeField] private string locale;
        [SerializeField] private long   iat;
        [SerializeField] private long   exp;
        [SerializeField] private string aud;

        /// <summary>
        ///     Who created and signed this token.
        /// </summary>
        public string Issuer => iss;


        /// <summary>
        ///     The party to which this token was issued.
        /// </summary>
        public string AuthorizedParty => azp;


        /// <summary>
        ///     Who and what the token is intended for.
        /// </summary>
        public string Audience => aud;


        /// <summary>
        ///     Whom the token refers to.
        /// </summary>
        public string Subject => sub;

        /// <summary>
        ///     Represents the email address of the user.
        /// </summary>
        public string Email => email;

        /// <summary>
        ///     Indicates whether the user's email address has been verified.
        /// </summary>
        public bool EmailVerified => email_verified;

        /// <summary>
        ///     Represents the name of the user.
        /// </summary>
        public string Name => name;

        /// <summary>
        ///     Represents the URL of the user's profile picture.
        /// </summary>
        public string PictureUrl => picture;

        /// <summary>
        ///     Represents the given name (first name) of the user.
        /// </summary>
        public string GivenName => given_name;

        /// <summary>
        ///     Represents the family name (last name) of the user.
        /// </summary>
        public string FamilyName => family_name;

        /// <summary>
        ///     Represents the locale of the user.
        /// </summary>
        public string Locale => locale;

        /// <summary>
        ///     Seconds since Unix epoch.
        /// </summary>
        public long IssuedAt => iat;

        /// <summary>
        ///     Seconds since Unix epoch.
        /// </summary>
        public long ExpirationTime => exp;

        internal TokenResponse Token { get; set; }

        #endregion
    }
}