using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Android;

namespace Izhguzin.GoogleIdentity.Flows
{
    internal class AndroidAuthorizationCodeFlow : IAuthorizationModel
    {
        public AndroidAuthorizationCodeFlow(GoogleAuthOptions options)
        {
            using GoogleSignInOptions.Builder optionsBuilder =
                new(GoogleSignInOptions.DefaultSignIn);

            optionsBuilder.RequestServerAuthCode(options.ClientId.ThrowIfNullOrEmpty(
                new NullReferenceException(
                    $"Client Id is not set in {nameof(GoogleAuthOptions)}.")));

            Scope[] scopes = new Scope[options.Scopes.Count];

            for (int i = 0; i < scopes.Length; i++) scopes[i] = new Scope(options.Scopes[i]);

            optionsBuilder.RequestScopes(scopes);

            using GoogleSignInClientProxy clientProxy = GsiAppCompatActivity.ClientProxy;
            clientProxy.InitOptions(optionsBuilder.Build());

            foreach (Scope scope in scopes) scope.Dispose();
        }

        public Task Authorize()
        {
            throw new NotImplementedException();
        }
    }
}