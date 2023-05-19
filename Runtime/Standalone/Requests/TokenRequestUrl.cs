using Izhguzin.GoogleIdentity.Standalone;

namespace Izhguzin.GoogleIdentity
{
    internal class TokenRequestUrl : RequestUrl
    {
        #region Fileds and Properties

        [RequestParameter("code", true)] public string Code { get; set; }

        [RequestParameter("redirect_uri", false)]
        public string RedirectUri { get; set; }

        [RequestParameter("client_id", true)] public string ClientId { get; set; }

        [RequestParameter("code_verifier", false)]
        public string CodeVerifier { get; set; }

        [RequestParameter("client_secret", true)]
        public string ClientSecret { get; set; }

        [RequestParameter("grant_type", false)]
        public string GrantType { get; set; } = "authorization_code";

        public override string EndPointUrl => GoogleAuthConstants.TokenUrl;

        #endregion
    }
}