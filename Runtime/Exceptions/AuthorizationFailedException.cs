using System;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     The AuthorizationFailedException class represents an exception that occurs when authorization fails during a
    ///     request to Google Identity. This class inherits from the RequestFailedException class.
    /// </summary>
    public class AuthorizationFailedException : RequestFailedException
    {
        internal new static AuthorizationFailedException Create(int errorCode, Exception exception)
        {
            return new AuthorizationFailedException(errorCode,
                $"An error occurred during authorization: ({exception.GetType().Name}) {exception.Message}");
        }

        public AuthorizationFailedException(int errorCode, string message) : base(errorCode, message) { }

        public AuthorizationFailedException(int errorCode, string message, Exception innerException) : base(errorCode,
            message, innerException) { }
    }
}