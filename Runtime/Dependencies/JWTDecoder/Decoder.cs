// MIT License
// 
// Copyright (c) 2020 Dale Myszewski
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Text;
using Izhguzin.GoogleIdentity.Utils;

namespace Izhguzin.GoogleIdentity.JWTDecoder
{
    /// <summary>
    ///     Sometimes all you need is a simple decoder.
    /// </summary>
    public static class Decoder
    {
        /// <summary>
        ///     Decode the specified token.
        /// </summary>
        /// <returns>A tupal contained the decoded Header and Payload.</returns>
        /// <param name="token">The token you with to decode.</param>
        public static (JwtHeader Header, string Payload, string Verification) DecodeToken(string token)
        {
            string[] split = token.Split('.');
            if (split.Length > 1)
            {
                JwtHeader jsonHeaderData =
                    StringSerializationAPI.Deserialize<JwtHeader>(Base64DecodeToString(split[0]));

                string jsonData = Base64DecodeToString(split[1]);

                string verification = split[2];

                return (jsonHeaderData, jsonData, verification);
            }

            throw new InvalidTokenPartsException("token");
        }

        private static string Base64DecodeToString(string toDecode)
        {
            try
            {
                string decodePrepped = toDecode.Replace("-", "+").Replace("_", "/");

                switch (decodePrepped.Length % 4)
                {
                    case 0:
                        break;
                    case 2:
                        decodePrepped += "==";
                        break;
                    case 3:
                        decodePrepped += "=";
                        break;
                    default:
                        throw new Exception("Not a legal base64 string!");
                }

                return Encoding.UTF8.GetString(Convert.FromBase64String(decodePrepped));
            }
            catch (FormatException)
            {
                throw new FormatException("The token contains invalid base64 characters.");
            }
        }

        /// <summary>
        ///     Exception thrown when when a token does not consist of three parts delimited by dots (".").
        /// </summary>
        public class InvalidTokenPartsException : ArgumentOutOfRangeException
        {
            /// <summary>
            ///     Creates an instance of <see cref="InvalidTokenPartsException" />
            /// </summary>
            /// <param name="paramName">The name of the parameter that caused the exception</param>
            public InvalidTokenPartsException(string paramName)
                : base(paramName, "Token must consist of 3 delimited by dot parts.") { }
        }
    }
}