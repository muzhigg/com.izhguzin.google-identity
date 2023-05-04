using System;
using System.Threading.Tasks;

namespace Izhguzin.GoogleIdentity
{
    internal abstract class BaseIdentityService : IIdentityService
    {
        #region Fileds and Properties

        protected IAuthorizationFlow Flow { get; set; }

        #endregion

        public abstract event Action                        OnSignIn;
        public abstract event Action<GoogleSignInException> OnRequestError;
        public abstract event Action                        OnSignOut;

        public abstract bool InProgress();

        public abstract Task SignIn();

        public abstract Task SignIn(string userId);

        public abstract void SignOut();

        internal abstract Task<bool> RefreshToken(UserCredential credential);

        internal abstract Task<bool> RevokeAccess(UserCredential credential);
    }
}