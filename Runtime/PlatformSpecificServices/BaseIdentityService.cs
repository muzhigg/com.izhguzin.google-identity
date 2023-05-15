using System;
using System.Threading.Tasks;

namespace Izhguzin.GoogleIdentity
{
    internal abstract class BaseIdentityService : IIdentityService
    {
        #region Fileds and Properties

        protected IAuthorizationModel Flow { get; set; }

        protected GoogleAuthOptions Options { get; }

        #endregion

        protected BaseIdentityService(GoogleAuthOptions options)
        {
            Options = options;
        }

        public abstract Task<TokenResponse> Authorize();

        public Task<TokenResponse> GetCachedTokenAsync(string userId)
        {
            throw new NotImplementedException();
        }

        internal abstract Task InitializeAsync();

        internal abstract Task<bool> RefreshTokenAsync(TokenResponse token);

        internal abstract Task<bool> RevokeAccessAsync(TokenResponse token);
    }
}