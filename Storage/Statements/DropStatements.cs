using System.Diagnostics.CodeAnalysis;

namespace Storage.Statements
{
    /// <summary>
    ///     The SQL statements for table deletion.
    /// </summary>
    public class DropStatements
    {
        /// <summary>
        ///     A SQL query string to drop the database.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string DropDatabase = @"DROP DATABASE IF EXISTS @Database;";

        /// <summary>
        ///     A SQL query string to delete the database version table.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string DropDatabaseVersionTable = @"DROP TABLE IF EXISTS databaseversion;";

        /// <summary>
        ///     A SQL query string to delete the blacklist table.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string DropBlacklistTable = @"DROP TABLE IF EXISTS blacklist;";

        /// <summary>
        ///     A SQL query string to delete the whitelist table.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string DropWhitelistTable = @"DROP TABLE IF EXISTS whitelist;";

        /// <summary>
        ///     A SQL query string to delete the user table.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string DropUserTable = @"DROP TABLE IF EXISTS user;";
    }
}
