using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    internal static class StringDeserializationAPI
    {
        private static readonly fsSerializer Serializer = new();

        public static T Deserialize<T>(string json) where T : class
        {
            object   deserializedToken = null;
            fsResult result = Serializer.TryDeserialize(fsJsonParser.Parse(json), typeof(T), ref deserializedToken);
            result.AssertSuccess();

            if (deserializedToken is not T response)
                throw new JsonDeserializationException($"Failed to deserialize JSON to {typeof(T).Name}. " +
                                                       $"Expected an object of type {typeof(T).Name} but received {deserializedToken.GetType().Name}.");

            if (result.HasWarnings) Debug.LogWarning(result.FormattedMessages);

            return response;
        }
    }
}