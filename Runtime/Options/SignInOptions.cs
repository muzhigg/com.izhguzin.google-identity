using System;
using System.Collections;
using System.Collections.Generic;

namespace Izhguzin.GoogleIdentity
{
    public class SignInOptions
    {
        public class Builder : IOptionsBuilder
        {
            #region Fileds and Properties

            private readonly SignInOptions _options;

            #endregion

            public Builder()
            {
                _options = new SignInOptions();
            }

            public SignInOptions Build()
            {
                return _options;
            }

            /// <summary>
            ///     Adds standalone platform options to the sign-in options. If you have already added options for this platform
            ///     before, builder will overwrite them.
            /// </summary>
            /// <returns>The current instance of the builder.</returns>
            public StandaloneSignInOptions.Builder AddStandaloneOptions()
            {
                return new StandaloneSignInOptions.Builder(_options);
            }

            /// <summary>
            ///     Adds Android platform options to the sign in options. If you have already added options for this platform before,
            ///     builder will overwrite them.
            /// </summary>
            /// <returns>The current instance of the builder.</returns>
            public AndroidSignInOptions.Builder AddAndroidOptions()
            {
                return new AndroidSignInOptions.Builder(_options);
            }
        }

        #region Fileds and Properties

        internal const string RootClientIdKey     = "root-client-id";
        internal const string RootClientSecretKey = "root-client-secret";


        [Obsolete] internal IDictionary<string, object> Values { get; }

        private StandaloneSignInOptions _standaloneOptions;
        private AndroidSignInOptions    _androidOptions;

        #endregion

        private SignInOptions()
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

        public void AddAndroidOptions(AndroidSignInOptions androidSignInOptions)
        {
            _androidOptions = androidSignInOptions;
        }

        internal void AddStandaloneOptions(StandaloneSignInOptions standaloneSignInOptions)
        {
            _standaloneOptions = standaloneSignInOptions;
        }
    }
}