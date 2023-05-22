using System;
using System.Security.Cryptography;
using System.Text;

namespace Izhguzin.GoogleIdentity.Utils
{
    internal static class PKCECodeProvider
    {
        public static string GetCodeChallenge(string codeVerifier, bool useS256GenerationMethod)
        {
            return useS256GenerationMethod ? GetBase64UrlEncodeNoPadding(ToSHA256(codeVerifier)) : codeVerifier;
        }

        public static string GetRandomBase64URL(int length)
        {
            byte[] bytes;
            using (RNGCryptoServiceProvider rng = new())
            {
                bytes = new byte[length];
                rng.GetBytes(bytes);
            }

            return GetBase64UrlEncodeNoPadding(bytes);
        }

        private static byte[] ToSHA256(string inputString)
        {
            byte[]              bytes  = Encoding.ASCII.GetBytes(inputString);
            using SHA256Managed sha256 = new();
            return sha256.ComputeHash(bytes);
        }

        private static string GetBase64UrlEncodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
    }
}