using System;

namespace Izhguzin.GoogleIdentity
{
    public class GoogleSignInException : Exception
    {
        #region Fileds and Properties

        public CommonStatus CommonStatus { get; }

        #endregion

        public GoogleSignInException(CommonStatus commonStatus, string message) : base(message)
        {
            CommonStatus = commonStatus;
        }

        public GoogleSignInException(CommonStatus commonStatus, string message, Exception innerException) : base(
            message,
            innerException) { }
    }
}