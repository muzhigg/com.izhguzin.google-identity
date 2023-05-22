using UnityEngine;

namespace Izhguzin.GoogleIdentity.Android
{
    internal class Scope : AndroidJavaObjectWrapper
    {
        public Scope(string scopeUri) : base(new AndroidJavaObject(
            "com.google.android.gms.common.api.Scope", scopeUri)) { }
    }
}