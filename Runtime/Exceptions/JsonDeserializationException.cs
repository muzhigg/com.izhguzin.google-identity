using System;

namespace Izhguzin.GoogleIdentity
{
    internal class JsonDeserializationException : Exception
    {
        public JsonDeserializationException(string message) : base(message) { }
    }
}