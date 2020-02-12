using System.Diagnostics.CodeAnalysis;

namespace Storage.Statements
{
    /// <summary>
    /// The SQL statements for updating data.
    /// </summary>
    public class UpdateStatements
    {
        /// <summary>
        /// A SQL query string to mark a user as deleted.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string MarkUserAsDeleted =
            @"UPDATE user SET deletedat = now() WHERE id = @Id;";

        /// <summary>
        /// A SQL query string to update a user.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string UpdateUser =
            @"UPDATE user SET
                username = @UserName,
                passwordhash = @PasswordHash,
                clientidprefix = @ClientIdPrefix,
                clientid = @ClientId,
                validateclientid = @ValidateClientId,
                throttleuser = @ThrottleUser,
                monthlybytelimit = @MonthlyByteLimit,
                updatedat = now()
            WHERE id = @Id;";

        /// <summary>
        /// A SQL query string to mark a database version as deleted.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string MarkDatabaseVersionAsDeleted =
            @"UPDATE databaseversion SET deletedat = now() WHERE id = @Id;";

        /// <summary>
        /// A SQL query string to update a database version.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string UpdateDatabaseVersion =
            @"UPDATE databaseversion SET
                name = @Name,
                number = @Number,
                updatedat = now()
            WHERE id = @Id;";

        /// <summary>
        /// A SQL query string to mark a whitelist item as deleted.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string MarkWhitelistItemAsDeleted =
            @"UPDATE whitelist SET deletedat = now() WHERE id = @Id;";

        /// <summary>
        /// A SQL query string to update a whitelist item.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string UpdateWhitelistItem =
            @"UPDATE whitelist SET
                userid = @UserId,
                type = @Type,
                value = @Value,
                updatedat = now()
            WHERE id = @Id;";

        /// <summary>
        /// A SQL query string to mark a blacklist item as deleted.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string MarkBlacklistItemAsDeleted =
            @"UPDATE blacklist SET deletedat = now() WHERE id = @Id;";

        /// <summary>
        /// A SQL query string to update a blacklist item.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string UpdateBlacklistItem =
            @"UPDATE blacklist SET
                userid = @UserId,
                type = @Type,
                value = @Value,
                updatedat = now()
            WHERE id = @Id;";

        /// <summary>
        /// A SQL query string to reset a password for a user.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string ResetPasswordForUser =
            @"UPDATE user SET
                passwordhash = @PasswordHash,
                updatedat = now()
                WHERE id = @Id;";
    }
}
