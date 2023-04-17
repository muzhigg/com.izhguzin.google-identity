using System;
using UnityEngine;

namespace Izhguzin.GoogleIdentity.Android
{
    internal interface IAndroidJavaObjectWrapper : IDisposable
    {
        public AndroidJavaObject GetAndroidJavaObject();
    }
}