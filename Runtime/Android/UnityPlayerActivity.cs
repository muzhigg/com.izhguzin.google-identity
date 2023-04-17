using UnityEngine;

namespace Izhguzin.GoogleIdentity.Android
{
    internal class GsiAppCompatActivity : UnityPlayerActivity
    {
        #region Fileds and Properties

        public static GoogleSignInClientProxy ClientProxy
        {
            get
            {
                using AndroidJavaClass javaClass = new("com.izhguzin.gsi.GsiAppCompatActivity");
                return new GoogleSignInClientProxy(javaClass.GetStatic<AndroidJavaObject>("clientProxy"));
            }
        }

        #endregion

        internal GsiAppCompatActivity(AndroidJavaObject activity) : base(activity) { }
    }

    internal class UnityPlayerActivity : AndroidJavaObjectWrapper
    {
        internal UnityPlayerActivity(AndroidJavaObject activity) : base(activity) { }
    }
}