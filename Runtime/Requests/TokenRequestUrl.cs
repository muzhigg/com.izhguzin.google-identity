using UnityEngine.Scripting;

namespace Izhguzin.GoogleIdentity
{
    internal class TokenRequestUrl : RequestUrl
    {
        #region Fileds and Properties

        [RequestParameter("code", true), Preserve]
        public string Code { [Preserve] get; set; }

        [RequestParameter("redirect_uri", false), Preserve]
        public string RedirectUri { get; set; }

        [RequestParameter("client_id", true), Preserve]
        public string ClientId { get; set; }

        [RequestParameter("code_verifier", false), Preserve]
        public string CodeVerifier { get; set; }

        [RequestParameter("client_secret", true), Preserve]
        public string ClientSecret { get; set; }

        [RequestParameter("grant_type", false), Preserve]
        public string GrantType { get; set; } = "authorization_code";

        public override string EndPointUrl => GoogleAuthConstants.TokenUrl;

        #endregion
    }
}