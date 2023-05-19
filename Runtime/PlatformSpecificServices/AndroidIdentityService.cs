using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Android;
using Izhguzin.GoogleIdentity.Utils;

#pragma warning disable CS4014

namespace Izhguzin.GoogleIdentity
{
    internal class AndroidIdentityService : GoogleIdentityService
    {
        public AndroidIdentityService(GoogleAuthOptions options) : base(options) { }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public override Task<TokenResponse> Authorize()
        {
            GoogleSignInClientProxy clientProxy = GoogleSignInClientProxy.GetInstance();
            clientProxy.SignOut();

            TaskCompletionSource<TokenResponse> completionSource = new();

            clientProxy.SignIn(new GoogleSignInClientProxy.OnTaskCompleteListener(listener =>
                UnityMainThread.RunOnMainThread(() => PerformCodeExchangeRequestAsync(listener, completionSource))));

            return completionSource.Task;
        }

        internal override Task InitializeAsync()
        {
            GoogleSignInClientProxy clientProxy = GoogleSignInClientProxy.GetInstance();

            if (clientProxy == null)
                throw new NullReferenceException(
                    "The current Android activity should initialize GoogleSignInClientProxy.");

            using GoogleSignInOptions.Builder optionsBuilder =
                new(GoogleSignInOptions.DefaultSignIn);

            optionsBuilder.RequestServerAuthCode(Options.ClientId.ThrowIfNullOrEmpty(
                new NullReferenceException(
                    $"Client Id is not set in {nameof(GoogleAuthOptions)}.")));

            Scope[] scopes = new Scope[Options.Scopes.Count];

            for (int i = 0; i < scopes.Length; i++) scopes[i] = new Scope(Options.Scopes[i]);

            optionsBuilder.RequestScopes(scopes);
            clientProxy.InitOptions(optionsBuilder.Build());

            foreach (Scope scope in scopes) scope.Dispose();

            return Task.CompletedTask;
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private async Task PerformCodeExchangeRequestAsync(GoogleSignInClientProxy.OnTaskCompleteListener listener,
            TaskCompletionSource<TokenResponse> completionSource)
        {
            if (listener.StatusCode is 1)
                try
                {
                    TokenResponse result =
                        await SendCodeExchangeRequestAsync(listener.Code, null, "");
                    GoogleSignInClientProxy.GetInstance().SignOut();
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

        //private GoogleSignInClientProxy _clientProxy;
    }
}