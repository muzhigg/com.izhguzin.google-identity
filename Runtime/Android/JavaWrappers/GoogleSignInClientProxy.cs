using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Izhguzin.GoogleIdentity.Android
{
    internal class GoogleSignInClientProxy : AndroidJavaObjectWrapper
    {
        [CanBeNull]
        public static GoogleSignInClientProxy GetInstance()
        {
            using AndroidJavaClass javaClass  = new("com.izhguzin.gsi.GoogleSignInClientProxy");
            AndroidJavaObject      javaObject = javaClass.CallStatic<AndroidJavaObject>("getInstance");

            return javaObject == null ? null : new GoogleSignInClientProxy(javaObject);
        }

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