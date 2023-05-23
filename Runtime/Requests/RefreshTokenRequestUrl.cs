using UnityEngine.Scripting;

namespace Izhguzin.GoogleIdentity
{
    internal class RefreshTokenRequestUrl : RequestUrl
    {
        #region Fileds and Properties

        public override string EndPointUrl => GoogleAuthConstants.TokenUrl;

        [RequestParameter("client_id", true), Preserve]
        public string ClientId { get; set; }

        [RequestParameter("client_secret", true), Preserve]
        public string ClientSecret { get; set; }

        [RequestParameter("grant_type", true), Preserve]
        public string GrantType => "refresh_token";

        [RequestParameter("refresh_token", true), Preserve]
        public string RefreshToken { get; set; }

        #endregion
    }
}