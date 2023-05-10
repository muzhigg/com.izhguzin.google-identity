﻿using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Flows;
using Izhguzin.GoogleIdentity.Standalone;

namespace Izhguzin.GoogleIdentity
{
    internal class StandaloneIdentityService : BaseIdentityService
    {
        public override event Action                        OnSignIn;
        public override event Action<GoogleSignInException> OnRequestError;
        public override event Action                        OnSignOut;

        #region Fileds and Properties

        private          AuthorizationRequestUrl _authorizationRequestUrl;
        private readonly bool                    _inProgress = false;

        #endregion

        public StandaloneIdentityService(GoogleAuthOptions options) : base(options) { }

        public override bool InProgress()
        {
            return _inProgress;
        }

        public override async Task SignIn()
        {
            //HttpCodeListener listener = new(_authorizationRequestUrl.RedirectUri,
            //    Options.ResponseHtml);
            await Flow.Authorize();
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
            Flow = Options.UseAuthorizationCodeFlow
                ? new StandaloneAuthorizationCodeFlow(Options)
                : new StandaloneImplicitFlow(Options);

            _authorizationRequestUrl =
                ((IStandaloneAuthorizationModel)Flow).GetAuthorizationRequestUrl();

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