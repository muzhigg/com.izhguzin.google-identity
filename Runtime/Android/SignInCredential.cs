using UnityEngine;

namespace Izhguzin.GoogleIdentity.Android
{
    internal class SignInCredential : AndroidJavaObjectWrapper
    {
        private SignInCredential(AndroidJavaObject javaObject) : base(javaObject) { }

        public static explicit operator SignInCredential(AndroidJavaObject obj)
        {
            return new SignInCredential(obj);
        }

        public string GetProfilePictureUri()
        {
            AndroidJavaObject uriObj = androidJavaObject.Call<AndroidJavaObject>("getProfilePictureUri");
            string            result = uriObj.Call<string>("toString");
            return result;
        }

        public string GetDisplayName()
        {
            return androidJavaObject.Call<string>("getDisplayName");
        }

        public string GetFamilyName()
        {
            return androidJavaObject.Call<string>("getFamilyName");
        }

        public string GetGivenName()
        {
            return androidJavaObject.Call<string>("getGivenName");
        }

        public string GetGoogleIdToken()
        {
            return androidJavaObject.Call<string>("getGoogleIdToken");
        }
    }
}