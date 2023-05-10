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

        public abstract event Action                        OnSignIn;
        public abstract event Action<GoogleSignInException> OnRequestError;
        public abstract event Action                        OnSignOut;

        public abstract bool InProgress();

        public abstract Task SignIn();

        public abstract Task SignIn(string userId);

        public abstract   void SignOut();
        internal abstract Task InitializeAsync();

        internal abstract Task<bool> RefreshTokenAsync(TokenResponse token);

        internal abstract Task<bool> RevokeAccessAsync(TokenResponse token);
    }
}