using System.Threading.Tasks;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     Inherit your class from this interface to implement a storage for access tokens.
    /// </summary>
    public interface ITokenStorage
    {
        /// <summary>
        ///     Asynchronously saves the access token represented by the jsonToken parameter for the specified userId in the
        ///     storage. The userId parameter serves as the unique user identifier or key.
        /// </summary>
        /// <param name="userId">Unique user identifier, use this parameter as a key.</param>
        /// <param name="jsonToken">TokenResponse in json format.</param>
        /// <returns>
        ///     The method returns a Task&lt;bool&gt; representing the asynchronous operation, indicating whether the token
        ///     saving process was successful (true) or not (false).
        /// </returns>
        public Task<bool> SaveTokenAsync(string userId, string jsonToken);

        /// <summary>
        ///     Asynchronously loads the access token for the specified userId from the storage. The userId parameter is used as
        ///     the key to retrieve the corresponding token.
        /// </summary>
        /// <param name="userId">Unique user identifier, use this parameter as a key.</param>
        /// <returns>TokenResponse in json format.</returns>
        public Task<string> LoadTokenAsync(string userId);
    }
}