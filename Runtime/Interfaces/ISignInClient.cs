namespace Izhguzin.GoogleIdentity
{
    internal interface ISignInClient
    {
        /// <summary>
        ///     Is the client in the progress of executing the request at the moment?
        /// </summary>
        public bool InProgress();

        /// <summary>
        ///     Starts an asynchronous sign-in request operation.
        ///     After calling the method, the user will need to select a Google account to sign in.
        ///     The sign-in process differs depending on the platform.
        ///     <para>A browser will be opened on the standalone platform.</para>
        ///     <para>On android devices, the activity will run on top of the Unity activity.</para>
        ///     <para>On the WebGl platform, the way differs from the client options.</para>
        /// </summary>
        public GoogleRequestAsyncOperation SignIn();
    }
}