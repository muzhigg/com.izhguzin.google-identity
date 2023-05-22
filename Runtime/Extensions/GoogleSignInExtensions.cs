using System;
using UnityEngine.Networking;

namespace Izhguzin.GoogleIdentity
{
    internal static class GoogleSignInExtensions
    {
        internal static string ThrowIfNull(this string value, string exceptionMessage)
        {
            if (value == null) throw new NullReferenceException(exceptionMessage);

            return value;
        }

        internal static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        internal static string ThrowIfNullOrEmpty(this string value, string exceptionMessage)
        {
            if (string.IsNullOrEmpty(value)) throw new NullReferenceException(exceptionMessage);

            return value;
        }

        internal static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UnityWebRequestAwaiter(asyncOp);
        }
    }
}