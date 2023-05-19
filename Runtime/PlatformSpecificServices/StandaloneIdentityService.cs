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
            if (InProgress)
                throw new InvalidOperationException(
                    "An operation is already in progress.");

            InProgress = true;
            AuthorizationRequestUrl requestUrl = GetAuthorizationRequestUrl();
            HttpCodeListener        listener   = new(requestUrl.RedirectUri, Options.ResponseHtml);

            try
            {
                Application.OpenURL(requestUrl.BuildUrl());
            }
            catch (NullReferenceException exception)
            {
                throw AuthorizationFailedException.Create(CommonErrorCodes.DeveloperError, exception);
            }

            string code = await listener.WaitForCodeAsync(requestUrl.State);

            TokenResponse result =
                await SendCodeExchangeRequestAsync(code, requestUrl.ProofCodeKey.codeVerifier, requestUrl.RedirectUri);

            InProgress = false;
            return result;
        }

        internal override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        private AuthorizationRequestUrl GetAuthorizationRequestUrl()
        {
            try
            {
                AuthorizationRequestUrl result = AuthorizationRequestUrl
                    .CreateDefaultFromOptions(Options);
                result.ResponseType = "code";
                result.AccessType   = "offline";
                result.Prompt       = "consent";
                result.ProofCodeKey = new AuthorizationRequestUrl.ProofKey(true);

                return result;
            }
            catch (NotSupportedException exception)
            {
                throw AuthorizationFailedException.Create(CommonErrorCodes.NetworkError, exception);
            }
            catch (Exception exception)
            {
                throw AuthorizationFailedException.Create(CommonErrorCodes.NetworkError, exception);
            }
        }
    }
}