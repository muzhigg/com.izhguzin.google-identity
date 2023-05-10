using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Android;
using Izhguzin.GoogleIdentity.Flows;

namespace Izhguzin.GoogleIdentity
{
    internal class AndroidIdentityService : BaseIdentityService
    {
        public override event Action                        OnSignIn;
        public override event Action<GoogleSignInException> OnRequestError;
        public override event Action                        OnSignOut;

        #region Fileds and Properties

        private GoogleSignInClientProxy _clientProxy;

        #endregion

        public AndroidIdentityService(GoogleAuthOptions options) : base(options) { }

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

        internal override Task InitializeAsync()
        {
            _clientProxy = GsiAppCompatActivity.ClientProxy;

            if (_clientProxy == null)
                throw new NullReferenceException(
                    "The current Android Activity must be GsiAppCompatActivity " +
                    "or inherited from it.");

            Flow = Options.UseAuthorizationCodeFlow
                ? new AndroidAuthorizationCodeFlow(Options)
                : throw new NotSupportedException(
                    "Implicit Flow is not supported by Google on Android devices.");

            return Task.CompletedTask;
        }

        internal override Task<bool> RefreshTokenAsync(TokenResponse token)
        {
            throw new NotImplementedException();
        }

        internal override Task<bool> RevokeAccessAsync(TokenResponse token)
        {
            throw new NotImplementedException();
        }
    }
}