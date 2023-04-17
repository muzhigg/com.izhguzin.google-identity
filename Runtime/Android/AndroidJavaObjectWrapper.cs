using UnityEngine;

namespace Izhguzin.GoogleIdentity.Android
{
    internal abstract class AndroidJavaObjectWrapper : IAndroidJavaObjectWrapper
    {
        #region Fileds and Properties

        protected AndroidJavaObject androidJavaObject;

        #endregion

        protected AndroidJavaObjectWrapper(AndroidJavaObject androidJavaObject)
        {
            this.androidJavaObject = androidJavaObject;
        }

        public AndroidJavaObject GetAndroidJavaObject()
        {
            return androidJavaObject;
        }

        public void Dispose()
        {
            if (androidJavaObject != null)
            {
                androidJavaObject.Dispose();
                androidJavaObject = null;
            }
        }
    }
}