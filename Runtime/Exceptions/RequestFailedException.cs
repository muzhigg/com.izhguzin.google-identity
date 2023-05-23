using System;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     The RequestFailedException class represents an exception that occurs when a request to Google Identity fails.
    /// </summary>
    public class RequestFailedException : Exception
    {
        internal static RequestFailedException Create(int errorCode, Exception exception)
        {
            return new RequestFailedException(errorCode, $"({exception.GetType().Name}) {exception.Message}");
        }

        #region Fileds and Properties

        /// <summary>
        ///     Represents the error code associated with the exception.
        ///     The error code can be compared with the constants defined
        ///     in the <see cref="CommonErrorCodes" /> and <see cref="AndroidCommonErrorCodes" /> classes for further error code
        ///     analysis and handling.
        /// </summary>
        public int ErrorCode { get; }

        #endregion

        public RequestFailedException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public RequestFailedException(int errorCode, string message, Exception innerException) : base(message,
            innerException)
        {
            ErrorCode = errorCode;
        }
    }
}