using System;
using System.Threading.Tasks;

namespace Izhguzin.GoogleIdentity
{
    public interface IIdentityService
    {
        event Action                        OnSignIn;
        event Action<GoogleSignInException> OnRequestError;
        event Action                        OnSignOut;

        /// <summary>
        ///     Is the client in the progress of executing the request at the moment?
        /// </summary>
        bool InProgress();

        // maybe rename to Authorize ?
        Task SignIn();

        Task SignIn(string userId);

        void SignOut();
    }
}