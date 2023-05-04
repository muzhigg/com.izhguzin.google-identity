using System;
using System.Threading.Tasks;

namespace Izhguzin.GoogleIdentity
{
    public interface IIdentityService
    {
        public event Action                        OnSignIn;
        public event Action<GoogleSignInException> OnRequestError;
        public event Action                        OnSignOut;

        /// <summary>
        ///     Is the client in the progress of executing the request at the moment?
        /// </summary>
        public bool InProgress();

        // maybe rename to Authorize ?
        public Task SignIn();

        public Task SignIn(string userId);

        public void SignOut();
    }
}