namespace Storage
{
    /// <summary>
    ///     A class for the database connection settings.
    /// </summary>
    public class DatabaseConnectionSettings
    {
        /// <summary>
        ///     Gets or sets the host of the database.
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        ///     Gets or sets the database name.
        /// </summary>
        public string Database { get; set; } = "mqtt";

        /// <summary>
        ///     Gets or sets the user name.
        /// </summary>
        public string Username { get; set; } = "mqtt";

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        public string Password { get; set; } = "mqtt";

        /// <summary>
        ///     Gets or sets the port.
        /// </summary>
        public int Port { get; set; } = 5432;

        /// <summary>
        ///     Gets or sets a value indicating whether the pooling is enabled or not.
        /// </summary>
        public bool Pooling { get; set; } = false;

        /// <summary>
        ///     Gets or sets the time zone.
        /// </summary>
        public string Timezone { get; set; } = "Europe/Berlin";

        /// <summary>
        ///     Gets the connection string.
        /// </summary>
        /// <returns>The connection string from the connection settings.</returns>
        public string ToConnectionString()
        {
            return
                $"Server={Host};Port={Port};Database={Database};Username={Username};Password={Password};Pooling={Pooling};Timezone={Timezone};Enlist=false;Maximum Pool Size=400;ConvertInfinityDateTime=true";
        }

        /// <summary>
        /// Gets the administrator connection string.
        /// </summary>
        /// <returns>The administrator connection string from the connection settings.</returns>
        public string ToAdminConnectionString()
        {
            return $"Host={Host};Port={Port};Username={Username};Password={Password};Database=postgres;Pooling={Pooling};Timezone={Timezone};Enlist=false;Maximum Pool Size=400;ConvertInfinityDateTime=true";
        }
    }
}