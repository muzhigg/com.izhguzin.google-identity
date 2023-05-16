using System;
using System.Threading;
using UnityEngine;

namespace Izhguzin.GoogleIdentity.Android
{
    internal class GoogleSignInClientProxy : AndroidJavaObjectWrapper
    {
        public class OnTaskCompleteListener : AndroidJavaProxy
        {
            #region Fileds and Properties

            public string Code { get; private set; }

            public int StatusCode { get; private set; }

            public string Error { get; private set; }

            private readonly Action<OnTaskCompleteListener> _callback;

            #endregion

            public OnTaskCompleteListener(Action<OnTaskCompleteListener> callback) : base(
                "com.izhguzin.gsi.GoogleSignInClientProxy$OnTaskCompleteListener")
            {
                _callback = callback;
            }

            private void onComplete(string code, int statusCode, string errorMessage)
            {
                //CommonStatus status = Enum.IsDefined(typeof(CommonStatus), statusCode)
                //    ? (CommonStatus)statusCode
                //    : CommonStatus.Error;
                Debug.Log(Thread.CurrentThread.ManagedThreadId);
                //AndroidJNI.AttachCurrentThread();
                Code       = code;
                StatusCode = statusCode;
                Error      = errorMessage;

                _callback(this);
            }
        }

        internal GoogleSignInClientProxy(AndroidJavaObject javaObject) : base(javaObject) { }

        public void InitOptions(GoogleSignInOptions options)
        {
            androidJavaObject.Call("initOptions", options.GetAndroidJavaObject());
        }

        public void SignIn(OnTaskCompleteListener listener)
        {
            androidJavaObject.Call("signIn", listener);
        }

        public void SignOut()
        {
            androidJavaObject.Call("signOut");
        }
    }
}