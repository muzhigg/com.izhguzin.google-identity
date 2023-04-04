using System;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     Exception thrown when when a token does not consist of three parts delimited by dots (".").
    /// </summary>
    internal class InvalidTokenPartsException : ArgumentOutOfRangeException
    {
        /// <summary>
        ///     Creates an instance of <see cref="InvalidTokenPartsException" />
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception</param>
        public InvalidTokenPartsException(string paramName)
            : base(paramName, "Token must consist of 3 delimited by dot parts.") { }
    }
}