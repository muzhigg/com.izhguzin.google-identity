using System;

namespace Izhguzin.GoogleIdentity
{
    internal interface ISignInClient
    {
        public bool InProgress();

        [Obsolete]
        public void BeginSignInOld();

        [Obsolete]
        public void SignOutOld();

        [Obsolete]
        public void RefreshTokenOld(UserCredential credential, OnSuccessCallback callback);

        [Obsolete]
        public void RevokeAccessOld(UserCredential credential);
    }
}