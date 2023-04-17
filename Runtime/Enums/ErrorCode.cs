namespace Izhguzin.GoogleIdentity
{
    public enum ErrorCode
    {
        Other = 0,

        /// <summary>
        ///     The ResponseError occurs when there is an error in the response received from the Google authentication server,
        ///     such as incorrect credentials or insufficient access rights.
        /// </summary>
        ResponseError = 2,

        /// <summary>
        ///     The client attempted to connect to the service with an invalid account name specified.
        /// </summary>
        InvalidAccount = 5,

        /// <summary>
        ///     A network error occurred. Retrying should resolve the problem.
        /// </summary>
        NetworkError = 7,

        /// <summary>
        ///     The application is misconfigured. This error is not recoverable and will be treated as fatal.
        /// </summary>
        DeveloperError = 10,

        /// <summary>
        ///     The operation failed with no more detailed information.
        /// </summary>
        Error = 13,

        /// <summary>
        ///     Timed out while awaiting the result.
        /// </summary>
        Timeout = 15,

        /// <summary>
        ///     The result was canceled either due to client disconnect.
        /// </summary>
        Canceled = 16
    }
}