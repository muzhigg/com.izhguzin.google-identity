namespace Izhguzin.GoogleIdentity
{
    /// <summary>
    ///     This class contains constants that represent various
    ///     scopes for authentication and authorization in Google API.
    ///     Each constant has an English description that explains
    ///     what data and functionality this scope provides.
    ///     Some scopes are sensitive and require verification before use.
    ///     Constants that represent sensitive scopes contain URLs that point to APIs associated with these scopes.
    ///     <para>
    ///         A complete list of OAuth 2.0 scopes can be found here:
    ///         <see href="https://developers.google.com/identity/protocols/oauth2/scopes" />
    ///     </para>
    /// </summary>
    public static class Scopes
    {
        #region Fileds and Properties

        /// <summary>
        ///     See your primary Google Account email address
        /// </summary>
        public const string Email = "email";

        /// <summary>
        ///     See your personal info, including any personal info you've made publicly available
        /// </summary>
        public const string Profile = "profile";

        /// <summary>
        ///     Associate you with your personal info on Google
        /// </summary>
        public const string OpenId = "openid";

        /// <summary>
        ///     View and manage your data in Google BigQuery and
        ///     see the email address for your Google Account
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string BigQuery = "https://www.googleapis.com/auth/bigquery";

        /// <summary>
        ///     See, edit, configure, and delete your Google Cloud data and
        ///     see the email address for your Google Account.
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string CloudPlatform = "https://www.googleapis.com/auth/cloud-platform";

        /// <summary>
        ///     View your data in Google BigQuery
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string BigQueryReadOnly = "https://www.googleapis.com/auth/bigquery.readonly";

        /// <summary>
        ///     View your data across Google Cloud services and
        ///     see the email address of your Google Account
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string CloudPlatformReadOnly = "https://www.googleapis.com/auth/cloud-platform.read-only";

        /// <summary>
        ///     Manage your data and permissions in Cloud Storage and see the email address for your Google Account
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string DevStorageFullControl = "https://www.googleapis.com/auth/devstorage.full_control";

        /// <summary>
        ///     View your data in Google Cloud Storage
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string DevStorageReadOnly = "https://www.googleapis.com/auth/devstorage.read_only";

        /// <summary>
        ///     Manage your data in Cloud Storage and
        ///     see the email address of your Google Account
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string DevStorageReadWrite = "https://www.googleapis.com/auth/devstorage.read_write";

        /// <summary>
        ///     Insert data into Google BigQuery
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string BigQueryInsertData = "https://www.googleapis.com/auth/bigquery.insertdata";

        /// <summary>
        ///     View and manage your Google Cloud Datastore data
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string DataStore = "https://www.googleapis.com/auth/datastore";

        /// <summary>
        ///     Use Stackdriver Debugger
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string CloudDebugger = "https://www.googleapis.com/auth/cloud_debugger";

        /// <summary>
        ///     View log data for your projects
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string LoggingRead = "https://www.googleapis.com/auth/logging.read";

        /// <summary>
        ///     Administrate log data for your projects
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string LoggingAdmin = "https://www.googleapis.com/auth/logging.admin";

        /// <summary>
        ///     Submit log data for your projects
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string LoggingWrite = "https://www.googleapis.com/auth/logging.write";

        /// <summary>
        ///     View and write monitoring data for all of your Google
        ///     and third-party Cloud and API projects
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string Monitoring = "https://www.googleapis.com/auth/monitoring";

        /// <summary>
        ///     View monitoring data for all of your Google Cloud and third-party projects
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string MonitoringRead = "https://www.googleapis.com/auth/monitoring.read";

        /// <summary>
        ///     Publish metric data to your Google Cloud projects
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string MonitoringWrite = "https://www.googleapis.com/auth/monitoring.write";

        /// <summary>
        ///     Manage your data in Google Cloud Storage
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string DevStorageWriteOnly = "https://www.googleapis.com/auth/devstorage.write_only";

        /// <summary>
        ///     Read Trace data for a project or application
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string TraceReadOnly = "https://www.googleapis.com/auth/trace.readonly";

        /// <summary>
        ///     Write Trace data for a project or application
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string TraceAppend = "https://www.googleapis.com/auth/trace.append";

        /// <summary>
        ///     Manage your Google API service configuration
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string ServiceManagement = "https://www.googleapis.com/auth/service.management";

        /// <summary>
        ///     View your Google API service configuration
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string ServiceManagementReadOnly = "https://www.googleapis.com/auth/service.management.readonly";

        #endregion
    }
}