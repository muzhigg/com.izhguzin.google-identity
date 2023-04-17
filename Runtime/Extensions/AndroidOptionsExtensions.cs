using System;

namespace Izhguzin.GoogleIdentity
{
    public static class AndroidOptionsExtensions
    {
        #region Fileds and Properties

        internal const string ClientIdKey                   = "android-client-id";
        internal const string FilterByAuthorizedAccountsKey = "android-filter-by-auth";
        internal const string AutoSelectEnabledKey          = "android-auto-select";

        #endregion

        public static SignInOptions SetAndroidCredentials(this SignInOptions options, string webClientId)
        {
            if (string.IsNullOrEmpty(webClientId))
                throw new ArgumentException("Client ID cannot be null or empty");

            return options.SetOption(ClientIdKey, webClientId);
        }

        public static SignInOptions SetFilterByAuthorizedAccounts(this SignInOptions options,
            bool                                                                     filterByAuthorizedAccounts)
        {
            return options.SetOption(FilterByAuthorizedAccountsKey, filterByAuthorizedAccounts);
        }

        public static SignInOptions SetAutoSelectEnabled(this SignInOptions options, bool autoSelectEnabled)
        {
            return options.SetOption(AutoSelectEnabledKey, autoSelectEnabled);
        }

        internal static string GetAndroidCredentials(this SignInOptions options)
        {
            if (!options.TryGetOption(ClientIdKey, out string clientId))
                options.TryGetOption(SignInOptions.RootClientIdKey, out clientId);

            return clientId;
        }

        internal static bool GetFilterByAuthorizedAccounts(this SignInOptions options)
        {
            if (!options.TryGetOption(FilterByAuthorizedAccountsKey, out bool result)) result = true;

            return result;
        }

        internal static bool GetAutoSelectEnabled(this SignInOptions options)
        {
            options.TryGetOption(AutoSelectEnabledKey, out bool result);

            return result;
        }
    }
}