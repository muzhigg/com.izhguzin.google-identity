using System;
using System.Threading.Tasks;

namespace Izhguzin.GoogleIdentity
{
    internal class StandaloneIdentityService : BaseIdentityService
    {
        public override event Action                        OnSignIn;
        public override event Action<GoogleSignInException> OnRequestError;
        public override event Action                        OnSignOut;

        public override bool InProgress()
        {
            throw new NotImplementedException();
        }

        public override Task SignIn()
        {
            throw new NotImplementedException();
        }

        public override Task SignIn(string userId)
        {
            throw new NotImplementedException();
        }

        public override void SignOut()
        {
            throw new NotImplementedException();
        }

        internal override Task<bool> RefreshToken(UserCredential credential)
        {
            throw new NotImplementedException();
        }

        internal override Task<bool> RevokeAccess(UserCredential credential)
        {
            throw new NotImplementedException();
        }
    }
}