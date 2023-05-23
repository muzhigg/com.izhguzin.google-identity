using UnityEngine.Scripting;

namespace Izhguzin.GoogleIdentity
{
    internal class RevokeAccessRequestUrl : RequestUrl
    {
        #region Fileds and Properties

        public override string EndPointUrl => GoogleAuthConstants.RevokeUrl;

        [RequestParameter("token", true), Preserve]
        public string Token { get; set; }

        #endregion

        public RevokeAccessRequestUrl(string token)
        {
            Token = token;
        }
    }
}