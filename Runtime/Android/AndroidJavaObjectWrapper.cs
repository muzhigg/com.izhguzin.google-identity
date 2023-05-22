using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Izhguzin.GoogleIdentity.Android
{
    internal abstract class AndroidJavaObjectWrapper : IAndroidJavaObjectWrapper
    {
        public static bool operator ==([CanBeNull] AndroidJavaObjectWrapper a, [CanBeNull] AndroidJavaObjectWrapper b)
        {
            if (ReferenceEquals(a, b)) return true;

            if (a is not null && b is not null)
                return AndroidJNI.IsSameObject(a.androidJavaObject.GetRawObject(),
                    b.androidJavaObject.GetRawObject());

            return false;
        }

        public static bool operator !=([CanBeNull] AndroidJavaObjectWrapper a, [CanBeNull] AndroidJavaObjectWrapper b)
        {
            return !(a == b);
        }

        #region Fileds and Properties

        protected readonly AndroidJavaObject androidJavaObject;

        #endregion

        protected AndroidJavaObjectWrapper([NotNull] AndroidJavaObject androidJavaObject)
        {
            this.androidJavaObject = androidJavaObject ?? throw new ArgumentNullException(nameof(androidJavaObject));
        }

        public AndroidJavaObject GetAndroidJavaObject()
        {
            return androidJavaObject;
        }

        public void Dispose()
        {
            androidJavaObject?.Dispose();
        }

        public bool Equals(AndroidJavaObjectWrapper other)
        {
            using AndroidJavaClass obj = new("java.util.Objects");
            return obj.CallStatic<bool>("equals", androidJavaObject, other.androidJavaObject);
        }

        public override bool Equals(object obj)
        {
            if (obj is not AndroidJavaObjectWrapper wrapper) return false;

            return Equals(wrapper);
        }

        public override int GetHashCode()
        {
            return androidJavaObject != null ? androidJavaObject.Call<int>("hashCode") : 0;
        }
    }
}