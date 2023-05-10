using System;
using UnityEngine;

namespace Izhguzin.GoogleIdentity.Android
{
    internal class GoogleSignInClientProxy : AndroidJavaObjectWrapper
    {
        public class OnTaskCompleteListener : AndroidJavaProxy
        {
            #region Fileds and Properties

            public string Value { get; private set; }

            public CommonStatus StatusCode { get; private set; }

            public string Error { get; private set; }

            private readonly Action<OnTaskCompleteListener> _callback;

            #endregion

            public OnTaskCompleteListener(Action<OnTaskCompleteListener> callback) : base(
                "com.izhguzin.gsi.GoogleSignInClientProxy$OnTaskCompleteListener")
            {
                _callback = callback;
            }

            private void onComplete(string value, int statusCode, string errorMessage)
            {
                CommonStatus status = Enum.IsDefined(typeof(CommonStatus), statusCode)
                    ? (CommonStatus)statusCode
                    : CommonStatus.Error;

                Value      = value;
                StatusCode = status;
                Error      = errorMessage;

                _callback(this);
            }
        }

        internal GoogleSignInClientProxy(AndroidJavaObject javaObject) : base(javaObject) { }

        public void InitOptions(GoogleSignInOptions options)
        {
            androidJavaObject.Call("initOptions", options.GetAndroidJavaObject());
        }

        public void ConfigureClient(string clientId, bool singleUse)
        {
            androidJavaObject.Call("configureClient", clientId, singleUse);
        }

        public void SignIn(OnTaskCompleteListener listener)
        {
            androidJavaObject.Call("signIn", listener);
        }

        public void SignOut(OnTaskCompleteListener listener)
        {
            androidJavaObject.Call("signOut", listener);
        }

        public void RevokeAccess(OnTaskCompleteListener listener)
        {
            androidJavaObject.Call("revokeAccess", listener);
        }
    }
}