using UnityEngine.Networking;

namespace Izhguzin.GoogleIdentity
{
    public static class GoogleSignInExtensions
    {
        public static SignInAsyncOperationAwaiter GetAwaiter(this SignInAsyncOperation asyncOp)
        {
            return new SignInAsyncOperationAwaiter(asyncOp);
        }
    }

    internal static class UnityWebRequestExtensions
    {
        internal static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UnityWebRequestAwaiter(asyncOp);
        }
    }
}