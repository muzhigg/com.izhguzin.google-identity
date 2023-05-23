using UnityEngine;

namespace Izhguzin.GoogleIdentity.Utils
{
    internal static class StringSerializationAPI
    {
        public static string Serialize<T>(T value) where T : class
        {
            return JsonUtility.ToJson(value);
        }

        /// <exception cref="JsonDeserializationException"></exception>
        public static T Deserialize<T>(string json) where T : class, new()
        {
            T result = new();
            JsonUtility.FromJsonOverwrite(json, result);

            return result;
        }
    }
}