using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Izhguzin.GoogleIdentity.Android
{
    internal class Scope : AndroidJavaObjectWrapper
    {
        public Scope(string scopeUri) : base(new AndroidJavaObject(
            "com.google.android.gms.common.api.Scope", scopeUri)) { }
    }

    internal class GoogleSignInOptions : AndroidJavaObjectWrapper
    {
        internal class Builder : AndroidJavaObjectWrapper
        {
            public Builder(GoogleSignInOptions googleSignInOptions) : base(
                new AndroidJavaObject(
                    "com.google.android.gms.auth.api.signin.GoogleSignInOptions$Builder",
                    googleSignInOptions.androidJavaObject)) { }

            public GoogleSignInOptions Build()
            {
                return new GoogleSignInOptions(
                    androidJavaObject.Call<AndroidJavaObject>("build"));
            }

            public Builder RequestEmail()
            {
                using AndroidJavaObject _ =
                    androidJavaObject.Call<AndroidJavaObject>("requestEmail");

                return this;
            }

            public Builder RequestId()
            {
                using AndroidJavaObject _ =
                    androidJavaObject.Call<AndroidJavaObject>("requestId");

                return this;
            }

            public Builder RequestIdToken(string serverClientId)
            {
                using AndroidJavaObject _ =
                    androidJavaObject.Call<AndroidJavaObject>(
                        "requestIdToken", serverClientId);

                return this;
            }

            public Builder RequestProfile()
            {
                using AndroidJavaObject _ =
                    androidJavaObject.Call<AndroidJavaObject>(
                        "requestProfile");

                return this;
            }

            public Builder RequestScopes(params Scope[] scopes)
            {
                if (scopes.Length == 0) return this;

                IntPtr clazz =
                    AndroidJNI.FindClass("com.google.android.gms.common.api.Scope");

                IntPtr objArray =
                    AndroidJNI.NewObjectArray(scopes.Length - 1, clazz,
                        scopes[0].GetAndroidJavaObject().GetRawObject());

                for (int i = 1; i < scopes.Length; i++)
                    AndroidJNI.SetObjectArrayElement(objArray, i - 1,
                        scopes[i].GetAndroidJavaObject().GetRawObject());

                AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(),
                    AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(),
                        "requestScopes"),
                    new[]
                    {
                        new jvalue
                        {
                            l = scopes[0].GetAndroidJavaObject().GetRawObject()
                        },
                        new jvalue
                        {
                            l = objArray
                        }
                    });

                return this;
            }

            public Builder RequestServerAuthCode(string serverClientId)
            {
                using AndroidJavaObject _ =
                    androidJavaObject.Call<AndroidJavaObject>(
                        "requestServerAuthCode", serverClientId);

                return this;
            }

            public Builder RequestServerAuthCode(string serverClientId, bool forceCodeForRefreshToken)
            {
                using AndroidJavaObject _ =
                    androidJavaObject.Call<AndroidJavaObject>(
                        "requestServerAuthCode", serverClientId, forceCodeForRefreshToken);

                return this;
            }

            public Builder SetAccountName(string accountName)
            {
                using AndroidJavaObject _ =
                    androidJavaObject.Call<AndroidJavaObject>(
                        "setAccountName", accountName);

                return this;
            }
        }

        #region Fileds and Properties

        public static GoogleSignInOptions DefaultSignIn
        {
            get
            {
                using AndroidJavaClass javaClass =
                    new("com.google.android.gms.auth.api.signin.GoogleSignInOptions");
                AndroidJavaObject obj = javaClass.GetStatic<AndroidJavaObject>("DEFAULT_SIGN_IN");

                if (obj == null)
                    throw new NullReferenceException("Can't find static filed: DEFAULT_SIGN_IN");

                return new GoogleSignInOptions(obj);
            }
        }

        #endregion

        private GoogleSignInOptions([NotNull] AndroidJavaObject androidJavaObject) : base(androidJavaObject) { }
    }
}