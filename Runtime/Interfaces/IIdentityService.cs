using System.Threading.Tasks;

namespace Izhguzin.GoogleIdentity
{
    public interface IIdentityService
    {
        Task<TokenResponse> Authorize();
        Task<TokenResponse> GetCachedTokenAsync(string userId);
    }
}