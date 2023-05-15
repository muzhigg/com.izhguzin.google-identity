using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Standalone;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    internal class StandaloneIdentityService : GoogleIdentityService
    {
        public StandaloneIdentityService(GoogleAuthOptions options) : base(options) { }

        public override async Task<TokenResponse> Authorize()
        {
            AuthorizationRequestUrl requestUrl = GetAuthorizationRequestUrl();
            HttpCodeListener        listener   = new(requestUrl.RedirectUri, Options.ResponseHtml);

            Application.OpenURL(requestUrl.BuildUrl());

            string code = await listener.WaitForCodeAsync(requestUrl.State);

            TokenResponse result =
                await SendCodeExchangeRequestAsync(code, requestUrl.ProofCodeKey.codeVerifier, requestUrl.RedirectUri);

            return result;
        }

        internal override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        //internal override async Task RefreshTokenAsync(TokenResponse token)
        //{
        //    RefreshTokenRequestUrl requestUrl = new()
        //    {
        //        RefreshToken = token.RefreshToken
        //    };
        //}

        //internal override Task RevokeAccessAsync(TokenResponse token)
        //{
        //    throw new NotImplementedException();
        //}

        private AuthorizationRequestUrl GetAuthorizationRequestUrl()
        {
            try
            {
                AuthorizationRequestUrl result = AuthorizationRequestUrl
                    .CreateDefaultWithOptions(Options);
                result.ResponseType = "code";
                result.AccessType   = "offline";
                result.Prompt       = "consent";
                result.ProofCodeKey = new AuthorizationRequestUrl.ProofKey(true);

                return result;
            }
            catch (NullReferenceException exception)
            {
                throw AuthorizationFailedException.Create(CommonErrorCodes.DeveloperError, exception);
            }
        }
    }
}