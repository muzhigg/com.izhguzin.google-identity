using System;

namespace Izhguzin.GoogleIdentity
{
    internal class InvalidTokenException : Exception
    {
        public InvalidTokenException(string message, Exception exception) : base(message, exception) { }
    }
}