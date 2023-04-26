using Izhguzin.GoogleIdentity.Utils;
using UnityEngine;

namespace Izhguzin.GoogleIdentity.Android
{
    internal class GoogleSignInClientProxy : AndroidJavaObjectWrapper
    {
        public class OnFailureListener : AndroidJavaProxy
        {
            #region Fileds and Properties

            private readonly OnFailureCallback _callback;

            #endregion

            public OnFailureListener(OnFailureCallback callback) : base(
                "com.izhguzin.gsi.GoogleSignInClientProxy$OnFailureListener")
            {
                _callback = callback;
            }

            // ReSharper disable once InconsistentNaming
            private void onFailure(int errorCode, string message)
            {
                //CommonStatus errorType = !Enum.IsDefined(typeof(CommonStatus), errorCode)
                //    ? CommonStatus.Other
                //    : (CommonStatus)errorCode;

                //_callback?.Invoke(errorType, message);
            }
        }

        public class OnSuccessListener : AndroidJavaProxy
        {
            #region Fileds and Properties

            private readonly OnSuccessCallback _callback;

            #endregion

            public OnSuccessListener(OnSuccessCallback callback) : base(
                "com.izhguzin.gsi.GoogleSignInClientProxy$OnSuccessListener")
            {
                _callback = callback;
            }

            // ReSharper disable once InconsistentNaming
            private void onSuccess(AndroidJavaObject credential)
            {
                UnityMainThread.RunOnMainThread(() =>
                {
                    Debug.Log($"Main Thread? {UnityMainThread.IsRunningOnMainThread()}");

                    SignInCredential cred = (SignInCredential)credential;
                    Debug.Log(cred.GetGoogleIdToken());
                    UserCredential userCredential = new()
                    {
                        Token =
                        {
                            IdToken = cred.GetGoogleIdToken()
                        }
                    };
                    Debug.Log(1);
                    _callback?.Invoke(userCredential);
                });

                //
            }
        }

        public GoogleSignInClientProxy(AndroidJavaObject javaObject) : base(javaObject) { }

        public void SetListeners(OnSuccessListener onSuccessListener, OnFailureListener onFailureListener)
        {
            androidJavaObject.Call("setListeners", onSuccessListener, onFailureListener);
        }

        public void BuildRequest(string clientId, bool filterByAuthorizedAccounts, bool autoSelectEnabled)
        {
            androidJavaObject.Call("buildRequest", clientId, filterByAuthorizedAccounts, autoSelectEnabled);
        }

        public void BeginSignIn()
        {
            androidJavaObject.Call("beginSignIn");
        }
    }
}