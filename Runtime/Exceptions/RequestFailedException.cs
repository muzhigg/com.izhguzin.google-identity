using System;

namespace Izhguzin.GoogleIdentity
{
    public class AuthorizationFailedException : RequestFailedException
    {
        internal new static AuthorizationFailedException Create(int errorCode, Exception exception)
        {
            return new AuthorizationFailedException(errorCode,
                $"An error occurred during authorization: ({exception.GetType().Name}) {exception.Message}");
        }

        public AuthorizationFailedException(int errorCode, string message) : base(errorCode, message) { }
    }

    public class RequestFailedException : Exception
    {
        internal static RequestFailedException Create(int errorCode, Exception exception)
        {
            return new RequestFailedException(errorCode, $"({exception.GetType().Name}) {exception.Message}");
        }

        #region Fileds and Properties

        public int ErrorCode { get; }

        public CommonStatus CommonStatus { get; }

        #endregion

        [Obsolete]
        public RequestFailedException(CommonStatus commonStatus, string message) : base(message)
        {
            CommonStatus = commonStatus;
        }

        [Obsolete]
        public RequestFailedException(CommonStatus commonStatus, string message, Exception innerException) : base(
            message,
            innerException) { }

        public RequestFailedException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}