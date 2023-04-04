using System;
using System.Text;

namespace Izhguzin.GoogleIdentity
{
    internal static class JwtDecoder
    {
        public static string GetPayload(string token)
        {
            string[] split = token.Split('.');

            if (split.Length <= 1)
                throw new InvalidTokenPartsException(
                    "The provided token does not contain the expected number of parts.");

            string jsonData = Base64DecodeToString(split[1]);
            return jsonData;
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
                }

                return Encoding.UTF8.GetString(Convert.FromBase64String(decodePrepped));
            }
            catch (FormatException)
            {
                throw new FormatException("The token contains invalid base64 characters.");
            }
            catch (Exception ex)
            {
                throw new InvalidTokenException("The token is invalid.", ex);
            }
        }
    }
}