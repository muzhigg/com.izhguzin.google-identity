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

        public StandaloneSignInOptions Standalone => _standaloneOptions ??= new StandaloneSignInOptions();

        public AndroidSignInOptions Android => _androidOptions ??= new AndroidSignInOptions();

        private StandaloneSignInOptions _standaloneOptions;
        private AndroidSignInOptions    _androidOptions;

        #endregion

        private SignInOptions() { }

        internal void AddAndroidOptions(AndroidSignInOptions androidSignInOptions)
        {
            _androidOptions = androidSignInOptions;
        }

        internal void AddStandaloneOptions(StandaloneSignInOptions standaloneSignInOptions)
        {
            _standaloneOptions = standaloneSignInOptions;
        }
    }
}