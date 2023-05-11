#if UNITY_WEBGL
using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Flows;

namespace Izhguzin.GoogleIdentity
{
    internal class WebGLIdentityService : BaseIdentityService
    {
        public override event Action                        OnSignIn;
        public override event Action<GoogleSignInException> OnRequestError;
        public override event Action                        OnSignOut;

        public WebGLIdentityService(GoogleAuthOptions options) : base(options) { }

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
            TaskCompletionSource<string> tcs = new();

            Flow = Options.UseAuthorizationCodeFlow
                ? new WebGLAuthorizationCodeFlow(Options, tcs)
                : new WebGLImplicitFlow(Options, tcs);

            return tcs.Task;
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
#endif