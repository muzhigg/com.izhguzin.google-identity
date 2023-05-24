using System.Threading.Tasks;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     The IIdentityService interface defines the contract for an identity service related to Google Identity.
    /// </summary>
    internal interface IIdentityService
    {
        /// <summary>
        ///     Asynchronously authorizes the user and returns a <see cref="TokenResponse" /> representing the authorization token.
        /// </summary>
        Task<TokenResponse> AuthorizeAsync();

        /// <summary>
        ///     Asynchronously loads the stored token for the specified userId from the storage provided by the developer.
        ///     The userId parameter is used as the key to retrieve the corresponding token.
        /// </summary>
        Task<TokenResponse> LoadStoredTokenAsync(string userId);
    }
}