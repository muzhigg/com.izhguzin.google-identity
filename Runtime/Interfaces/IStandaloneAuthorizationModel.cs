using Izhguzin.GoogleIdentity.Standalone;

namespace Izhguzin.GoogleIdentity
{
    internal interface IStandaloneAuthorizationModel : IAuthorizationModel
    {
        AuthorizationRequestUrl GetAuthorizationRequestUrl();
    }
}