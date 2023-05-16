using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Android;
using Izhguzin.GoogleIdentity.Utils;

#pragma warning disable CS4014

namespace Izhguzin.GoogleIdentity
{
    internal class AndroidIdentityService : GoogleIdentityService
    {
        #region Fileds and Properties

        private GoogleSignInClientProxy _clientProxy;

        #endregion

        public AndroidIdentityService(GoogleAuthOptions options) : base(options) { }

        public override Task<TokenResponse> Authorize()
        {
            _clientProxy.SignOut();

            TaskCompletionSource<TokenResponse> completionSource = new();

            _clientProxy.SignIn(new GoogleSignInClientProxy.OnTaskCompleteListener(listener =>
                UnityMainThread.RunOnMainThread(() => PerformCodeExchangeRequestAsync(listener, completionSource))));

            return completionSource.Task;
        }

        internal override Task InitializeAsync()
        {
            _clientProxy = GsiAppCompatActivity.ClientProxy;

            if (_clientProxy == null)
                throw new NullReferenceException(
                    "The current Android Activity must be GsiAppCompatActivity " +
                    "or inherited from it.");

            using GoogleSignInOptions.Builder optionsBuilder =
                new(GoogleSignInOptions.DefaultSignIn);

            optionsBuilder.RequestServerAuthCode(Options.ClientId.ThrowIfNullOrEmpty(
                new NullReferenceException(
                    $"Client Id is not set in {nameof(GoogleAuthOptions)}.")));

            Scope[] scopes = new Scope[Options.Scopes.Count];

            for (int i = 0; i < scopes.Length; i++) scopes[i] = new Scope(Options.Scopes[i]);

            optionsBuilder.RequestScopes(scopes);
            _clientProxy.InitOptions(optionsBuilder.Build());

            foreach (Scope scope in scopes) scope.Dispose();

            return Task.CompletedTask;
        }

        private async Task PerformCodeExchangeRequestAsync(GoogleSignInClientProxy.OnTaskCompleteListener listener,
            TaskCompletionSource<TokenResponse> completionSource)
        {
            if (listener.StatusCode is 1)
                try
                {
                    TokenResponse result =
                        await SendCodeExchangeRequestAsync(listener.Code, null, "");
                    _clientProxy.SignOut();
                    completionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    completionSource.SetException(ex);
                }
            else
                completionSource.SetException(new AuthorizationFailedException(listener.StatusCode,
                    $"An error occurred during authorization: {listener.Error}"));

            listener.javaInterface.Dispose();
        }
    }
}