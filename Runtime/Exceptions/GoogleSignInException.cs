using System;

namespace Izhguzin.GoogleIdentity
{
    public class GoogleSignInException : Exception
    {
        public GoogleSignInException(string message) : base(message) { }
        public GoogleSignInException(string message, Exception innerException) : base(message, innerException) { }
    }
}