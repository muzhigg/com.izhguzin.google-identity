using UnityEngine;

namespace Izhguzin.GoogleIdentity.Android
{
    internal sealed class UnityPlayer
    {
        #region Fileds and Properties

        public static UnityPlayerActivity CurrentActivity
        {
            get
            {
                using AndroidJavaClass javaClass = new("com.unity3d.player.UnityPlayer");
                return new UnityPlayerActivity(javaClass.GetStatic<AndroidJavaObject>("currentActivity"));
            }
        }

        #endregion
    }
}