namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     The CommonErrorCodes class contains constants that
    ///     represent error codes related to authentication and
    ///     connecting to Google services. Each constant has
    ///     an English description that explains what this error code means.
    /// </summary>
    public static class CommonErrorCodes
    {
        #region Fileds and Properties

        /// <summary>
        ///     The ResponseError occurs when there is an error in the response received from the Google authentication server,
        ///     such as incorrect credentials or insufficient access rights.
        /// </summary>
        public const int ResponseError = 10000;

        /// <summary>
        ///     A network error occurred. Retrying should resolve the problem.
        /// </summary>
        public const int NetworkError = 7;

        /// <summary>
        ///     The application is misconfigured. This error is not recoverable and will be treated as fatal.
        /// </summary>
        public const int DeveloperError = 10;

        /// <summary>
        ///     The operation failed with no more detailed information.
        /// </summary>
        public const int Error = 13;

        /// <summary>
        ///     Timed out while awaiting the result.
        /// </summary>
        public const int Timeout = 15;

        public const int DeserializationError = 2000;

        /// <summary>
        ///     The sign in was cancelled by the user. i.e.
        ///     user cancelled some of the sign in resolutions, e.g. account picking or OAuth consent.
        /// </summary>
        public const int SignInCancelled = 12501;

        #endregion
    }
}