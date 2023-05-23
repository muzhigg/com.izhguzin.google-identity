//using Unity.VisualScripting.FullSerializer;

using UnityEngine;

namespace Izhguzin.GoogleIdentity.Utils
{
    public static class StringSerializationAPI
    {
        public static string Serialize<T>(T value) where T : class
        {
            return JsonUtility.ToJson(value);

            //fsResult result = _serializer.TrySerialize(typeof(T), value, out fsData data);
            //result.AssertSuccess();

            //return fsJsonPrinter.CompressedJson(data);
        }

        /// <exception cref="JsonDeserializationException"></exception>
        public static T Deserialize<T>(string json) where T : class, new()
        {
            T result = new();
            JsonUtility.FromJsonOverwrite(json, result);

            return result;

            //object deserializedToken = null;
            //fsResult result =
            //    _serializer.TryDeserialize(fsJsonParser.Parse(json), typeof(T), ref deserializedToken);
            //result.AssertSuccess();
            //if (result.HasWarnings) Debug.LogWarning(result.FormattedMessages);

            //if (deserializedToken is not T response)
            //    throw new JsonDeserializationException($"Failed to deserialize JSON to {typeof(T).Name}. " +
            //                                           $"Expected an object of type {typeof(T).Name} but received {deserializedToken.GetType().Name}.");

            //return response;
        }

        //private static readonly fsSerializer _serializer = new();
    }
}