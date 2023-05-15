using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Android;
using Izhguzin.GoogleIdentity.Flows;

namespace Izhguzin.GoogleIdentity
{
    internal class AndroidIdentityService : BaseIdentityService
    {
        #region Fileds and Properties

        private GoogleSignInClientProxy _clientProxy;

        #endregion

        public AndroidIdentityService(GoogleAuthOptions options) : base(options) { }

        public override Task<TokenResponse> Authorize()
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