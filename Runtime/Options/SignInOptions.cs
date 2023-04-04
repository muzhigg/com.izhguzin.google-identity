using System.Collections;
using System.Collections.Generic;

namespace Izhguzin.GoogleIdentity
{
    public class SignInOptions
    {
        #region Fileds and Properties

        internal const string RootClientIdKey     = "root-client-id";
        internal const string RootClientSecretKey = "root-client-secret";

        internal IDictionary<string, object> Values { get; }

        #endregion

        public SignInOptions()
        {
            Values = new Dictionary<string, object>();
        }

        /// <summary>
        ///     Stores the given <paramref name="value" /> for the given <paramref name="key" />.
        /// </summary>
        /// <param name="key">
        ///     The identifier of the configuration entry.
        /// </param>
        /// <param name="value">
        ///     The value to store.
        /// </param>
        /// <returns>
        ///     Return this instance.
        /// </returns>
        public SignInOptions SetOption(string key, bool value)
        {
            Values[key] = value;
            return this;
        }

        public bool TryGetOption<T>(string key, out T option)
        {
            option = default;

            if (!Values.TryGetValue(key, out object rawValue) || rawValue is not T value) return false;

            option = value;
            return true;
        }

        public SignInOptions SetOption(string key, IEnumerable value)
        {
            Values[key] = value;
            return this;
        }
    }
}