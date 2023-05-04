namespace Izhguzin.GoogleIdentity
{
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
        public const string BigQuery = ApisUrl + "bigquery";

        /// <summary>
        ///     See, edit, configure, and delete your Google Cloud data and
        ///     see the email address for your Google Account.
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string CloudPlatform = ApisUrl + "cloud-platform";

        /// <summary>
        ///     View your data in Google BigQuery
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string BigQueryReadOnly = ApisUrl + "bigquery.readonly";

        /// <summary>
        ///     View your data across Google Cloud services and
        ///     see the email address of your Google Account
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string CloudPlatformReadOnly = ApisUrl + "cloud-platform.read-only";

        /// <summary>
        ///     Manage your data and permissions in Cloud Storage and see the email address for your Google Account
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string DevStorageFullControl = ApisUrl + "devstorage.full_control";

        /// <summary>
        ///     View your data in Google Cloud Storage
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string DevStorageReadOnly = ApisUrl + "devstorage.read_only";

        /// <summary>
        ///     Manage your data in Cloud Storage and
        ///     see the email address of your Google Account
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string DevStorageReadWrite = ApisUrl + "devstorage.read_write";

        /// <summary>
        ///     Insert data into Google BigQuery
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string BigQueryInsertData = ApisUrl + "bigquery.insertdata";

        /// <summary>
        ///     View and manage your Google Cloud Datastore data
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string DataStore = ApisUrl + "datastore";

        /// <summary>
        ///     Use Stackdriver Debugger
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string CloudDebugger = ApisUrl + "cloud_debugger";

        /// <summary>
        ///     View log data for your projects
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string LoggingRead = ApisUrl + "logging.read";

        /// <summary>
        ///     Administrate log data for your projects
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string LoggingAdmin = ApisUrl + "logging.admin";

        /// <summary>
        ///     Submit log data for your projects
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string LoggingWrite = ApisUrl + "logging.write";

        /// <summary>
        ///     View and write monitoring data for all of your Google
        ///     and third-party Cloud and API projects
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string Monitoring = ApisUrl + "monitoring";

        /// <summary>
        ///     View monitoring data for all of your Google Cloud and third-party projects
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string MonitoringRead = ApisUrl + "monitoring.read";

        /// <summary>
        ///     Publish metric data to your Google Cloud projects
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string MonitoringWrite = ApisUrl + "monitoring.write";

        /// <summary>
        ///     Manage your data in Google Cloud Storage
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string DevStorageWriteOnly = ApisUrl + "devstorage.write_only";

        /// <summary>
        ///     Read Trace data for a project or application
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string TraceReadOnly = ApisUrl + "trace.readonly";

        /// <summary>
        ///     Write Trace data for a project or application
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string TraceAppend = ApisUrl + "trace.append";

        /// <summary>
        ///     Manage your Google API service configuration
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string ServiceManagement = ApisUrl + "service.management";

        /// <summary>
        ///     View your Google API service configuration
        ///     <para>This is a sensitive scope, which will require verification before it can be used</para>
        /// </summary>
        public const string ServiceManagementReadOnly = ApisUrl + "service.management.readonly";

        private const string ApisUrl = "https://www.googleapis.com/auth/";

        #endregion
    }
}