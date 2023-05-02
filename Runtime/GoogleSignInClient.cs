using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Utils;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     A client that allows you to request Google's servers to retrieve user data.
    /// </summary>
    public abstract class GoogleSignInClient : ISignInClient
    {
        /// <summary>
        ///     Creates a new client instance for a specific platform.
        ///     If the current platform is not supported, an exception will be thrown.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static GoogleSignInClient CreateInstance(SignInOptions options)
        {
#if UNITY_STANDALONE
            return new StandaloneSignInClient(options);
#elif UNITY_ANDROID
            return new AndroidSignInClient(options);
#else
            throw new NotSupportedException(
                $"This platform ({Application.platform}) is not supported by the GoogleSignInClient");
#endif
        }

        private static string GenerateMessage(CommonStatus commonStatus)
        {
            return commonStatus switch
            {
                CommonStatus.ResponseError          => "Google server response error. Try again later.",
                CommonStatus.InvalidAccount         => "Invalid account.",
                CommonStatus.NetworkError           => "Connection error. Try again later.",
                CommonStatus.DeveloperError         => "Developer error. Please check application configuration.",
                CommonStatus.Error                  => "Unexpected error occurred.",
                CommonStatus.Timeout                => "Timeout while awaiting response. Try again later.",
                CommonStatus.Canceled               => "Operation canceled.",
                CommonStatus.DeserializationError   => "Deserialization Error.",
                CommonStatus.LoadingCachedUserError => "Error when loading data. Please sign out and sign in again.",
                _                                   => null
            };
        }

        #region Fileds and Properties

        /// <summary>
        ///     The current user who gave access to his profile.
        ///     The property will have a null value in cases when the client object is just created and when it is logged out.
        /// </summary>
        public UserCredential CurrentUser { get; protected set; }

        /// <summary>
        ///     The status of the last operation performed by this client.
        /// </summary>
        public CommonStatus Status
        {
            get => _inProgress ? CommonStatus.InProgress : _status;
            private set => _status = value;
        }

        protected readonly SignInOptions options;

        private bool         _inProgress;
        private CommonStatus _status = CommonStatus.Success;

        #endregion

        protected GoogleSignInClient(SignInOptions options)
        {
            this.options = options;
        }

        /// <inheritdoc />
        public bool InProgress()
        {
            return _inProgress;
        }

        /// <inheritdoc />
        public GoogleRequestAsyncOperation SignIn()
        {
            GoogleRequestAsyncOperation asyncOp = new(this);

            Task.Run(() =>
            {
                if (!CanBeginOperation(asyncOp)) return;

                if (CurrentUser != null)
                {
                    UnityMainThread.RunOnMainThread(() => InvokeOnSuccess(CurrentUser, asyncOp, true));
                    return;
                }

                UnityMainThread.RunOnMainThread(() => BeginSignIn(asyncOp));
            });

            return asyncOp;
        }

        public GoogleRequestAsyncOperation RefreshToken()
        {
            GoogleRequestAsyncOperation asyncOp = new(this);

            Task.Run(() =>
            {
                if (!CanBeginOperation(asyncOp)) return;

                UnityMainThread.RunOnMainThread(() => BeginRefreshToken(asyncOp));
            });

            return asyncOp;
        }

        public GoogleRequestAsyncOperation RevokeAccess()
        {
            GoogleRequestAsyncOperation asyncOp = new(this);

            Task.Run(() =>
            {
                if (!CanBeginOperation(asyncOp)) return;

                UnityMainThread.RunOnMainThread(() => BeginRevokeAccess(asyncOp));
            });

            return asyncOp;
        }

        protected abstract void BeginSignIn(GoogleRequestAsyncOperation operation);

        protected abstract void BeginRefreshToken(GoogleRequestAsyncOperation operation);

        protected abstract void BeginRevokeAccess(GoogleRequestAsyncOperation operation);

        protected void InvokeOnSuccess(UserCredential credential, GoogleRequestAsyncOperation asyncOp)
        {
            CurrentUser = credential;
            InvokeOnComplete(asyncOp, CommonStatus.Success);
        }

        protected void InvokeOnSuccess(UserCredential credential, GoogleRequestAsyncOperation asyncOp, bool fromCache)
        {
            CurrentUser = credential;
            InvokeOnComplete(asyncOp, fromCache ? CommonStatus.SuccessCache : CommonStatus.Success);
        }

        protected void InvokeOnComplete(GoogleRequestAsyncOperation asyncOp, CommonStatus code)
        {
            Status        = code;
            asyncOp.Error = GenerateMessage(code);
            UnityMainThread.RunOnMainThread(() => asyncOp.InvokeCompletionEvent(code));
            _inProgress = false;
        }

        protected void OnExceptionCatch(GoogleRequestAsyncOperation asyncOperation, CommonStatus commonStatus,
            Exception                                               exception)
        {
            InvokeOnComplete(asyncOperation, commonStatus);
            Debug.LogException(exception);
        }

        private bool CanBeginOperation(GoogleRequestAsyncOperation asyncOp)
        {
            if (InProgress())
            {
                Debug.LogWarning("GoogleSignInClient is already performing the request.");
                InvokeOnComplete(asyncOp, CommonStatus.Canceled);
                return false;
            }

            _inProgress = true;
            return true;
        }
    }
}