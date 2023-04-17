using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    internal static class StringSerializationAPI
    {
        private static readonly fsSerializer _serializer = new();

        public static string Serialize<T>(object value)
        {
            fsResult result = _serializer.TrySerialize(typeof(T), value, out fsData data);
            result.AssertSuccess();

            return fsJsonPrinter.CompressedJson(data);
        }

        public static T Deserialize<T>(string json) where T : class
        {
            object   deserializedToken = null;
            fsResult result = _serializer.TryDeserialize(fsJsonParser.Parse(json), typeof(T), ref deserializedToken);
            result.AssertSuccess();

            if (deserializedToken is not T response)
                throw new JsonDeserializationException($"Failed to deserialize JSON to {typeof(T).Name}. " +
                                                       $"Expected an object of type {typeof(T).Name} but received {deserializedToken.GetType().Name}.");

            if (result.HasWarnings) Debug.LogWarning(result.FormattedMessages);

            return response;
        }
    }
}