using System;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity.Utils;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     Entry class to the Authentication Service.
    /// </summary>
    public sealed class GoogleIdentityService
    {
        public static async Task InitializeAsync(GoogleAuthOptions options)
        {
            if (UnityMainThread.IsRunningOnMainThread() == false)
                throw new InitializationException(
                    "You are attempting to initialize Google Identity Service from a non-Unity Main Thread. Google Identity Service can only be initialized from Main Thread");

            if (Application.isPlaying == false)
                throw new InitializationException(
                    "You are attempting to initialize Google Identity Service in Edit Mode. Google Identity Service can only be initialized in Play Mode");

            IIdentityService instance = CreateInstance(options);
            try
            {
                await ((BaseIdentityService)instance).InitializeAsync();
            }
            catch (Exception ex)
            {
                throw new InitializationException(
                    $"An error occurred during initialization: {ex.Message}");
            }

            _instance = instance;
        }

        private static IIdentityService CreateInstance(GoogleAuthOptions options)
        {
#if UNITY_STANDALONE
            return new StandaloneIdentityService(options);
#elif UNITY_ANDROID
            return new AndroidIdentityService(options);
#else
            throw new InitializationException(
                $"This platform ({Application.platform}) is not supported by "+
                $"the GoogleIdentityService");
#endif
        }

        //private static string GenerateMessage(CommonStatus commonStatus)
        //{
        //    return commonStatus switch
        //    {
        //        CommonStatus.ResponseError          => "Google server response error. Try again later.",
        //        CommonStatus.InvalidAccount         => "Invalid account.",
        //        CommonStatus.NetworkError           => "Connection error. Try again later.",
        //        CommonStatus.DeveloperError         => "Developer error. Please check application configuration.",
        //        CommonStatus.Error                  => "Unexpected error occurred.",
        //        CommonStatus.Timeout                => "Timeout while awaiting response. Try again later.",
        //        CommonStatus.Canceled               => "Operation canceled.",
        //        CommonStatus.DeserializationError   => "Deserialization Error.",
        //        CommonStatus.LoadingCachedUserError => "Error when loading data. Please sign out and sign in again.",
        //        _                                   => null
        //    };
        //}

        private static IIdentityService _instance;

        #region Fileds and Properties

        public static IIdentityService Instance
        {
            get
            {
                if (_instance == null)
                    throw new InitializationException(
                        "Google Identity Service is not initialized.");

                return _instance;
            }
        }

        #endregion

        ///// <summary>
        /////     The current user who gave access to his profile.
        /////     The property will have a null value in cases when the client object is just created and when it is logged out.
        ///// </summary>
        //public UserCredential CurrentUser { get; protected set; }

        ///// <summary>
        /////     The status of the last operation performed by this client.
        ///// </summary>
        //public CommonStatus Status
        //{
        //    get => _inProgress ? CommonStatus.InProgress : _status;
        //    private set => _status = value;
        //}

        //protected readonly GoogleAuthOptions options;

        //private bool         _inProgress;
        //private CommonStatus _status = CommonStatus.Success;

        ///// <inheritdoc />
        //public bool InProgress()
        //{
        //    return _inProgress;
        //}

        /// <inheritdoc />
        //public GoogleRequestAsyncOperation SignIn()
        //{
        //    GoogleRequestAsyncOperation asyncOp = new(this);

        //    Task.Run(() =>
        //    {
        //        if (!CanBeginOperation(asyncOp)) return;

        //        if (CurrentUser != null)
        //        {
        //            UnityMainThread.RunOnMainThread(() => InvokeOnSuccess(CurrentUser, asyncOp, true));
        //            return;
        //        }

        //        UnityMainThread.RunOnMainThread(() => BeginSignIn(asyncOp));
        //    });

        //    return asyncOp;
        //}

        //public GoogleRequestAsyncOperation SignOut()
        //{
        //    GoogleRequestAsyncOperation asyncOp = new(this);

        //    Task.Run(() =>
        //    {
        //        if (!CanBeginOperation(asyncOp)) return;

        //        UnityMainThread.RunOnMainThread(() => BeginSignOut(asyncOp));
        //    });

        //    return asyncOp;
        //}

        //protected void BeginSignOut(GoogleRequestAsyncOperation operation) { }

        //protected void BeginSignIn(GoogleRequestAsyncOperation operation) { }

        //protected void InvokeOnSuccess(UserCredential credential, GoogleRequestAsyncOperation asyncOp)
        //{
        //    CurrentUser = credential;
        //    InvokeOnComplete(asyncOp, CommonStatus.Success);
        //}

        //protected void InvokeOnSuccess(UserCredential credential, GoogleRequestAsyncOperation asyncOp, bool fromCache)
        //{
        //    CurrentUser = credential;
        //    InvokeOnComplete(asyncOp, fromCache ? CommonStatus.SuccessCache : CommonStatus.Success);
        //}

        //protected void InvokeOnComplete(GoogleRequestAsyncOperation asyncOp, CommonStatus code)
        //{
        //    Status        = code;
        //    asyncOp.Error = GenerateMessage(code);
        //    UnityMainThread.RunOnMainThread(() => asyncOp.InvokeCompletionEvent(code));
        //    _inProgress = false;
        //}

        //protected void OnExceptionCatch(GoogleRequestAsyncOperation asyncOperation, CommonStatus commonStatus,
        //    Exception                                               exception)
        //{
        //    InvokeOnComplete(asyncOperation, commonStatus);
        //    Debug.LogException(exception);
        //}

        //private bool CanBeginOperation(GoogleRequestAsyncOperation asyncOp)
        //{
        //    if (InProgress())
        //    {
        //        Debug.LogWarning("GoogleIdentityService is already performing the request.");
        //        InvokeOnComplete(asyncOp, CommonStatus.Canceled);
        //        return false;
        //    }

        //    _inProgress = true;
        //    return true;
        //}
    }
}