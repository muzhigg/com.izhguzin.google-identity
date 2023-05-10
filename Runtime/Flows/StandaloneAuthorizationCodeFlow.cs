using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Standalone;

namespace Izhguzin.GoogleIdentity.Flows
{
    internal class StandaloneAuthorizationCodeFlow : IStandaloneAuthorizationModel
    {
        #region Fileds and Properties

        private readonly GoogleAuthOptions       _options;
        private          AuthorizationRequestUrl _authorizationRequestUrl;

        #endregion

        public StandaloneAuthorizationCodeFlow(GoogleAuthOptions options)
        {
            _options                 = options;
            _authorizationRequestUrl = GetAuthorizationRequestUrl();
        }

        /// <exception cref="NullReferenceException"></exception>
        public AuthorizationRequestUrl GetAuthorizationRequestUrl()
        {
            AuthorizationRequestUrl result = AuthorizationRequestUrl
                .CreateDefaultWithOptions(_options);
            result.ResponseType = "code";
            result.AccessType   = "offline";
            result.Prompt       = "consent";
            result.ProofCodeKey = new AuthorizationRequestUrl.ProofKey(true);

            return result;
        }

        public Task Authorize()
        {
            throw new NotImplementedException();
        }
    }
}