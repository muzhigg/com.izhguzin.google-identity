using System.Threading.Tasks;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     Inherit your class from this interface to implement a storage for access tokens.
    /// </summary>
    public interface ITokenStorage
    {
        /// <summary>
        ///     GoogleIdentityService calls this method after the user has successfully signed in.
        /// </summary>
        /// <param name="userId">Unique user identifier, use this parameter as a key.</param>
        /// <param name="jsonToken">Access token in json format.</param>
        public Task SaveToken(string userId, string jsonToken);

        /// <summary>
        ///     GoogleIdentityService calls this method before the authorization flow.
        ///     If your method returns an access token from your storage, the client will use that token.
        ///     <para>Return a null or empty string if there is no token in your storage for that user.</para>
        /// </summary>
        /// <param name="userId">Unique user identifier, use this parameter as a key.</param>
        /// <returns>Access token in json format.</returns>
        public Task<string> LoadToken(string userId);

        /// <summary>
        ///     GoogleIdentityService calls this method when the user revokes access.
        /// </summary>
        /// <param name="userId">Unique user identifier, use this parameter as a key.</param>
        public Task DeleteToken(string userId);
    }
}