using JetBrains.Annotations;
using UnityEngine;

namespace Izhguzin.GoogleIdentity.Android
{
    internal class GsiAppCompatActivity : UnityPlayerActivity
    {
        #region Fileds and Properties

        [CanBeNull]
        public static GoogleSignInClientProxy ClientProxy
        {
            get
            {
                using AndroidJavaClass javaClass = new("com.izhguzin.gsi.GsiAppCompatActivity");
                AndroidJavaObject      javaObj   = javaClass.GetStatic<AndroidJavaObject>("clientProxy");

                return javaObj == null ? null : new GoogleSignInClientProxy(javaObj);
            }
        }

        #endregion

        private GsiAppCompatActivity(AndroidJavaObject activity) : base(activity) { }
    }
}