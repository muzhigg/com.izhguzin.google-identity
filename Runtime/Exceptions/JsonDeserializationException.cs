using System;

namespace Izhguzin.GoogleIdentity
{
    internal class JsonDeserializationException : Exception
    {
        public JsonDeserializationException(string message) : base(message) { }

        public JsonDeserializationException(string message, Exception innerException) :
            base(message, innerException) { }
    }
}