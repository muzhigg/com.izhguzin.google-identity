namespace Izhguzin.GoogleIdentity
{
    internal interface ISignInClient
    {
        public bool InProgress();
        public void BeginSignIn();
        public void SignOut();
        public void RefreshToken(UserCredential credential, OnSuccessCallback callback);
        public void RevokeAccess(UserCredential credential);
    }
}