namespace Izhguzin.GoogleIdentity.Standalone
{
    internal interface IRequestUrl
    {
        #region Fileds and Properties

        public string EndPointUrl { get; }

        #endregion

        public string BuildUrl();
        public string BuildBody();
    }
}