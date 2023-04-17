using System;

namespace Izhguzin.GoogleIdentity
{
    public class GoogleSignInException : Exception
    {
        #region Fileds and Properties

        public ErrorCode ErrorCode { get; }

        #endregion

        [Obsolete]
        public GoogleSignInException(string message) : base(message) { }

        public GoogleSignInException(ErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public GoogleSignInException(ErrorCode errorCode, string message, Exception innerException) : base(message,
            innerException) { }
    }
}