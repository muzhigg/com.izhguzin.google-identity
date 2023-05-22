using Izhguzin.GoogleIdentity.Utils;
using UnityEngine.Scripting;

namespace Izhguzin.GoogleIdentity
{
    internal class AuthorizationRequestUrl : RequestUrl
    {
        internal readonly struct ProofKey
        {
            public readonly string codeVerifier;
            public readonly string codeChallenge;

            public readonly string codeChallengeMethod;

            public ProofKey(bool useS256GenerationMethod)
            {
                codeVerifier        = PKCECodeProvider.GetRandomBase64URL(32);
                codeChallenge       = PKCECodeProvider.GetCodeChallenge(codeVerifier, useS256GenerationMethod);
                codeChallengeMethod = useS256GenerationMethod ? "S256" : "plain";
            }
        }

        #region Fileds and Properties

        [RequestParameter("response_type", true)]
        public string ResponseType { get; set; }

        [RequestParameter("client_id", true)] public string ClientId { get; set; }

        [RequestParameter("redirect_uri", true)]
        public string RedirectUri { get; set; }

        [RequestParameter("scope", true)] public string Scope { get; set; }

        [RequestParameter("code_challenge", false)]
        public string CodeChallenge => ProofCodeKey.codeChallenge;

        [RequestParameter("code_challenge_method", false), Preserve]
        public string CodeChallengeMethod => CodeChallenge == null ? null : ProofCodeKey.codeChallengeMethod;

        [RequestParameter("state", false)] public string State { get; set; }

        [RequestParameter("prompt", false)] public string Prompt { get; set; }

        [RequestParameter("access_type", false)]
        public string AccessType { get; set; }

        public ProofKey ProofCodeKey { get; set; }

        public override string EndPointUrl => GoogleAuthConstants.AuthorizationUrl;

        #endregion
    }
}