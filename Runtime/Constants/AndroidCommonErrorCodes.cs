namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     The AndroidCommonErrorCodes class contains constants
    ///     that represent error codes related to connecting to Google Play
    ///     services on Android devices. Each constant has an English
    ///     description that explains what this error code means.
    /// </summary>
    public static class AndroidCommonErrorCodes
    {
        #region Fileds and Properties

        /// <summary>
        ///     The client attempted to connect to the service but the user is not signed in.
        /// </summary>
        public const int SignInRequired = 4;

        /// <summary>
        ///     The client attempted to connect to the service with an invalid account name specified.
        /// </summary>
        public const int InvalidAccount = 5;

        /// <summary>
        ///     Completing the operation requires some form of resolution.
        /// </summary>
        public const int ResolutionRequired = 6;

        /// <summary>
        ///     An internal error occurred. Retrying should resolve the problem.
        /// </summary>
        public const int InternalError = 8;

        /// <summary>
        ///     A blocking call was interrupted while waiting and did not run to completion.
        /// </summary>
        public const int Interrupted = 14;

        /// <summary>
        ///     The result was canceled either due to client disconnect.
        /// </summary>
        public const int Canceled = 16;

        /// <summary>
        ///     The client attempted to call a method from an API that failed to connect. Possible reasons include:
        ///     <para>The API previously failed to connect with a resolvable error, but the user declined the resolution.</para>
        ///     <para>The device does not support GmsCore.</para>
        ///     <para>The specific API cannot connect on this device.</para>
        /// </summary>
        public const int ApiNotConnected = 17;

        /// <summary>
        ///     There was a non-DeadObjectException RemoteException while calling a connected service.
        ///     This signifies that an API was able to connect to Google Play services and received an instance, but that when
        ///     calling an individual method, a RemoteException was thrown.
        ///     Note that the exception would not be a DeadObjectException, which indicates that the binding has died (for example
        ///     if the remote process died). This is because DeadObjectExceptions are handled by the connection management
        ///     infrastructure of GoogleApi.
        ///     If this is encountered during an API call for an API that uses the Google Play services ServiceBroker (most do), it
        ///     is after a bound service is successfully received from the ServiceBroker.
        /// </summary>
        public const int RemoteException = 19;

        /// <summary>
        ///     The connection was suspended while the call was in-flight.
        ///     API calls will be failed with this status if ServiceConnection.onServiceDisconnected(ComponentName) is called by
        ///     the Android platform before the call is completed.
        ///     This indicates that the underlying service was bound, since onServiceDisconnected should never be called unless
        ///     onServiceConnected was called first.
        ///     It is possible this failure could be resolved by retrying.
        /// </summary>
        public const int ConnectionSuspendedDuringCall = 20;

        /// <summary>
        ///     The connection timed-out while waiting for Google Play services to update.
        ///     This failure indicates a connection to Google Play services was successfully established, however it was
        ///     disconnected because Google Play services was updated, and the update took far longer than expected.
        ///     Any API calls failed with this status would have been initiated after the disconnection for the update.
        /// </summary>
        public const int ReconnectionTimedOutDuringUpdate = 21;

        /// <summary>
        ///     The sign in attempt didn't succeed with the current account.
        ///     Unlike <see cref="AndroidCommonErrorCodes.SignInRequired" /> when seeing this error code,
        ///     there is nothing user can do to recover from the sign in failure.
        ///     Switching to another account may or may not help.
        ///     Check adb log to see details if any.
        /// </summary>
        public const int SignInFailed = 12500;

        /// <summary>
        ///     A sign in process is currently in progress and the current one cannot continue.
        ///     e.g. the user clicks the SignInButton multiple times and more than one sign in intent was launched.
        /// </summary>
        public const int SignInCurrentlyInProgress = 12502;

        #endregion
    }
}