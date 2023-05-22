using UnityEngine.Networking;

namespace Izhguzin.GoogleIdentity
{
    internal interface IRequestUrl
    {
        #region Fileds and Properties

        public string EndPointUrl { get; }

        #endregion

        public string          BuildUrl();
        public string          BuildBody();
        public UnityWebRequest CreatePostRequest();
    }
}