using System;
using Unity.VisualScripting.FullSerializer;

namespace Izhguzin.GoogleIdentity
{
    [Serializable, fsObject]
    public sealed class UserCredential
    {
        #region Fileds and Properties

        [fsProperty("email")]          public string Email         { get; internal set; }
        [fsProperty("email_verified")] public bool   EmailVerified { get; internal set; }
        [fsProperty("name")]           public string Name          { get; internal set; }
        [fsProperty("picture")]        public string PictureUrl    { get; internal set; }
        [fsProperty("given_name")]     public string GivenName     { get; internal set; }
        [fsProperty("family_name")]    public string FamilyName    { get; internal set; }
        [fsProperty("locale")]         public string Locale        { get; internal set; }

        public TokenResponse Token { get; internal set; }

        #endregion
    }
}