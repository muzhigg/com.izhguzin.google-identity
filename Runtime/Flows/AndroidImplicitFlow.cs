using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Android;

namespace Izhguzin.GoogleIdentity.Flows
{
    internal class AndroidImplicitFlow : IAuthorizationModel
    {
        public AndroidImplicitFlow(GoogleAuthOptions options)
        {
            GoogleSignInOptions.Builder optionsBuilder =
                new(GoogleSignInOptions.DefaultSignIn);

            //optionsBuilder.RequestIdToken(options.ClientId.ThrowIfNullOrEmpty(
            //    new NullReferenceException($"Client Id is not set in {nameof(GoogleAuthOptions)}.")));
        }

        public Task Authorize()
        {
            throw new NotImplementedException();
        }
    }
}