using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Standalone;

namespace Izhguzin.GoogleIdentity.Flows
{
    internal class StandaloneImplicitFlow : IStandaloneAuthorizationModel
    {
        #region Fileds and Properties

        private readonly GoogleAuthOptions       _options;
        private          AuthorizationRequestUrl _authorizationRequestUrl;

        #endregion

        public StandaloneImplicitFlow(GoogleAuthOptions options)
        {
            _options = options;
        }

        /// <exception cref="RequestFailedException"></exception>
        public AuthorizationRequestUrl GetAuthorizationRequestUrl()
        {
            try
            {
                AuthorizationRequestUrl result = AuthorizationRequestUrl
                    .CreateDefaultFromOptions(_options);
                result.ResponseType = "token id_token";
                result.Nonce        = PKCECodeProvider.GetRandomBase64URL(32);

                return result;
            }
            catch (NullReferenceException exception)
            {
                throw RequestFailedException.Create(CommonErrorCodes.DeveloperError, exception);
            }
        }

        /// <exception cref="RequestFailedException"></exception>
        public async Task Authorize()
        {
            //// ok
            //_authorizationRequestUrl = GetAuthorizationRequestUrl();

            //// ok
            //HttpTokenListener listener = new(_authorizationRequestUrl.RedirectUri,
            //    _options.ResponseHtml);

            //Application.OpenURL(_authorizationRequestUrl.BuildUrl());

            //TokenResponse tokenResponse = await listener.WaitForTokenAsync(_authorizationRequestUrl.State);
        }
    }
}