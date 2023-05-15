namespace Izhguzin.GoogleIdentity
{
    public static class CommonErrorCodes
    {
        #region Fileds and Properties

        /// <summary>
        ///     The ResponseError occurs when there is an error in the response received from the Google authentication server,
        ///     such as incorrect credentials or insufficient access rights.
        /// </summary>
        public const int ResponseError = 2;

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
        ///     A network error occurred. Retrying should resolve the problem.
        /// </summary>
        public const int NetworkError = 7;

        /// <summary>
        ///     An internal error occurred. Retrying should resolve the problem.
        /// </summary>
        public const int InternalError = 8;

        /// <summary>
        ///     The application is misconfigured. This error is not recoverable and will be treated as fatal.
        /// </summary>
        public const int DeveloperError = 10;

        /// <summary>
        ///     The operation failed with no more detailed information.
        /// </summary>
        public const int Error = 13;

        /// <summary>
        ///     A blocking call was interrupted while waiting and did not run to completion.
        /// </summary>
        public const int Interrupted = 14;

        /// <summary>
        ///     Timed out while awaiting the result.
        /// </summary>
        public const int Timeout = 15;

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

        public const int DeserializationError = 2000;

        public const int LoadingCachedUserError = 2001;

        /// <summary>
        ///     The sign in attempt didn't succeed with the current account.
        ///     Unlike CommonStatus.SignInRequired when seeing this error code,
        ///     there is nothing user can do to recover from the sign in failure.
        ///     Switching to another account may or may not help.
        ///     <para>For Android: Check adb log to see details if any.</para>
        /// </summary>
        public const int SignInFailed = 12500;

        /// <summary>
        ///     The sign in was cancelled by the user. i.e.
        ///     user cancelled some of the sign in resolutions, e.g. account picking or OAuth consent.
        /// </summary>
        public const int SignInCancelled = 12501;

        /// <summary>
        ///     A sign in process is currently in progress and the current one cannot continue.
        ///     e.g. the user clicks the SignInButton multiple times and more than one sign in intent was launched.
        /// </summary>
        public const int SignInCurrentlyInProgress = 12502;

        #endregion
    }

    public enum CommonStatus
    {
        InProgress = -2,

        /// <summary>
        ///     The operation was successful, but was used the device's cache.
        /// </summary>
        SuccessCache = -1,

        /// <summary>
        ///     The operation was successful.
        /// </summary>
        Success = 0,

        /// <summary>
        ///     The ResponseError occurs when there is an error in the response received from the Google authentication server,
        ///     such as incorrect credentials or insufficient access rights.
        /// </summary>
        ResponseError = 2,

        /// <summary>
        ///     The client attempted to connect to the service but the user is not signed in.
        /// </summary>
        SignInRequired = 4,

        /// <summary>
        ///     The client attempted to connect to the service with an invalid account name specified.
        /// </summary>
        InvalidAccount = 5,

        /// <summary>
        ///     Completing the operation requires some form of resolution.
        /// </summary>
        ResolutionRequired = 6,

        /// <summary>
        ///     A network error occurred. Retrying should resolve the problem.
        /// </summary>
        NetworkError = 7,

        /// <summary>
        ///     An internal error occurred. Retrying should resolve the problem.
        /// </summary>
        InternalError = 8,

        /// <summary>
        ///     The application is misconfigured. This error is not recoverable and will be treated as fatal.
        /// </summary>
        DeveloperError = 10,

        /// <summary>
        ///     The operation failed with no more detailed information.
        /// </summary>
        Error = 13,

        /// <summary>
        ///     A blocking call was interrupted while waiting and did not run to completion.
        /// </summary>
        Interrupted = 14,

        /// <summary>
        ///     Timed out while awaiting the result.
        /// </summary>
        Timeout = 15,

        /// <summary>
        ///     The result was canceled either due to client disconnect.
        /// </summary>
        Canceled = 16,

        /// <summary>
        ///     The client attempted to call a method from an API that failed to connect. Possible reasons include:
        ///     <para>The API previously failed to connect with a resolvable error, but the user declined the resolution.</para>
        ///     <para>The device does not support GmsCore.</para>
        ///     <para>The specific API cannot connect on this device.</para>
        /// </summary>
        ApiNotConnected = 17,

        DeserializationError = 2000,

        LoadingCachedUserError = 2001,

        /// <summary>
        ///     The sign in attempt didn't succeed with the current account.
        ///     Unlike CommonStatus.SignInRequired when seeing this error code,
        ///     there is nothing user can do to recover from the sign in failure.
        ///     Switching to another account may or may not help.
        ///     <para>For Android: Check adb log to see details if any.</para>
        /// </summary>
        SignInFailed = 12500,

        /// <summary>
        ///     The sign in was cancelled by the user. i.e.
        ///     user cancelled some of the sign in resolutions, e.g. account picking or OAuth consent.
        /// </summary>
        SignInCancelled = 12501,

        /// <summary>
        ///     A sign in process is currently in progress and the current one cannot continue.
        ///     e.g. the user clicks the SignInButton multiple times and more than one sign in intent was launched.
        /// </summary>
        SignInCurrentlyInProgress = 12502
    }
}