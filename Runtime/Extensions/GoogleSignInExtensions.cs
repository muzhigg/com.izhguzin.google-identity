using System;
using UnityEngine.Networking;

namespace Izhguzin.GoogleIdentity
{
    public static class GoogleSignInExtensions
    {
        public static SignInAsyncOperationAwaiter GetAwaiter(this GoogleRequestAsyncOperation asyncOp)
        {
            return new SignInAsyncOperationAwaiter(asyncOp);
        }

        internal static bool IsNull(this string s)
        {
            return s == null;
        }

        internal static string ThrowIfNull(this string value, Exception exception)
        {
            if (value == null) throw exception;

            return value;
        }

        internal static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        internal static string ThrowIfNullOrEmpty(this string value, Exception exception)
        {
            if (string.IsNullOrEmpty(value)) throw exception;

            return value;
        }

        internal static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UnityWebRequestAwaiter(asyncOp);
        }
    }
}