using System.Diagnostics.CodeAnalysis;

namespace Storage.Statements
{
    /// <summary>
    ///     The SQL statements for inserting data.
    /// </summary>
    public class InsertStatements
    {
        /// <summary>
        ///     A SQL query string to insert a user.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string InsertUser =
            @"INSERT INTO user (id, username, passwordhash, clientidprefix, clientid, validateclientid, throttleuser, monthlybytelimit)
            VALUES (@Id, @UserName, @PasswordHash, ClientIdPrefix, ClientId, ValidateClientId, ThrottleUser, MonthlyByteLimit);";

        /// <summary>
        ///     A SQL query string to insert a database version.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string InsertDatabaseVersion =
            @"INSERT INTO databaseversion (id, name, number)
            VALUES (@Id, @Name, @Number);";

        /// <summary>
        ///     A SQL query string to insert a blacklist item.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string InsertBlacklistItem =
            @"INSERT INTO blacklist (id, userid, type, value)
            VALUES (@Id, @UserId, @Type, @Value);";

        /// <summary>
        ///     A SQL query string to insert a whitelist item.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string InsertWhitelistItem =
            @"INSERT INTO whitelist (id, userid, type, value)
            VALUES (@Id, @UserId, @Type, @Value);";
    }
}
