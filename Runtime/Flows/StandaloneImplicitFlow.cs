using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Standalone;
using UnityEngine;

namespace Izhguzin.GoogleIdentity.Flows
{
    internal class StandaloneImplicitFlow : IStandaloneAuthorizationModel
    {
        #region Fileds and Properties

        private readonly GoogleAuthOptions       _options;
        private readonly AuthorizationRequestUrl _authorizationRequestUrl;

        #endregion

        public StandaloneImplicitFlow(GoogleAuthOptions options)
        {
            _options                 = options;
            _authorizationRequestUrl = GetAuthorizationRequestUrl();
        }

        /// <exception cref="NullReferenceException"></exception>
        public AuthorizationRequestUrl GetAuthorizationRequestUrl()
        {
            AuthorizationRequestUrl result = AuthorizationRequestUrl
                .CreateDefaultWithOptions(_options);
            result.ResponseType = "token id_token";
            result.Nonce        = PKCECodeProvider.GetRandomBase64URL(32);

            return result;
        }

        public async Task Authorize()
        {
            Debug.Log(_authorizationRequestUrl.BuildUrl());
            Application.OpenURL(_authorizationRequestUrl.BuildUrl());
        }
    }
}