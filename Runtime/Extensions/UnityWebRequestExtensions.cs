using UnityEngine.Networking;

namespace Izhguzin.GoogleIdentity
{
    internal static class UnityWebRequestExtensions
    {
        internal static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UnityWebRequestAwaiter(asyncOp);
        }
    }
}