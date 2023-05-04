using System;

namespace Izhguzin.GoogleIdentity
{
    public class InitializationException : Exception
    {
        public InitializationException(string message) : base(message) { }
    }
}