using System;
using Izhguzin.GoogleIdentity.Android;

namespace Izhguzin.GoogleIdentity
{
    public delegate void OnSuccessCallback(UserCredential credential);

    public delegate void OnFailureCallback(CommonStatus commonStatus, string errorMessage);

    internal class AndroidSignInClient : GoogleSignInClient
    {
        public AndroidSignInClient(SignInOptions options) : base(options)
        {
            GoogleSignInClientProxy clientProxy = GsiAppCompatActivity.ClientProxy;

            if (clientProxy == null)
                throw new GoogleSignInException(CommonStatus.DeveloperError,
                    "The current android activity must be GsiAppCompatActivity or inherited from it.");
        }

        protected override void BeginSignIn(GoogleRequestAsyncOperation operation)
        {
            throw new NotImplementedException();
        }

        protected override void BeginRefreshToken(GoogleRequestAsyncOperation operation)
        {
            throw new NotImplementedException();
        }

        protected override void BeginRevokeAccess(GoogleRequestAsyncOperation operation)
        {
            throw new NotImplementedException();
        }
    }
}