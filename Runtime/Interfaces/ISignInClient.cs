namespace Izhguzin.GoogleIdentity
{
    internal interface ISignInClient
    {
        public bool InProgress();
        public void BeginSignIn();
    }
}