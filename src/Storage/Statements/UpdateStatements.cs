// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateStatements.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The SQL statements for updating data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Statements;

/// <summary>
///     The SQL statements for updating data.
/// </summary>
public class UpdateStatements
{
    /// <summary>
    ///     A SQL query string to mark a user as deleted.
    /// </summary>
    public const string MarkUserAsDeleted =
        @"UPDATE mqttuser SET deletedat = now() WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to update a user.
    /// </summary>
    public const string UpdateUser =
        @"UPDATE mqttuser SET
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
    ///     A SQL query string to mark a database version as deleted.
    /// </summary>
    public const string MarkDatabaseVersionAsDeleted =
        @"UPDATE databaseversion SET deletedat = now() WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to update a database version.
    /// </summary>
    public const string UpdateDatabaseVersion =
        @"UPDATE databaseversion SET
                name = @Name,
                number = @Number,
                updatedat = now()
            WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to mark a whitelist item as deleted.
    /// </summary>
    public const string MarkWhitelistItemAsDeleted =
        @"UPDATE whitelist SET deletedat = now() WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to update a whitelist item.
    /// </summary>
    public const string UpdateWhitelistItem =
        @"UPDATE whitelist SET
                userid = @UserId,
                type = @Type,
                value = @Value,
                updatedat = now()
            WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to mark a blacklist item as deleted.
    /// </summary>
    public const string MarkBlacklistItemAsDeleted =
        @"UPDATE blacklist SET deletedat = now() WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to update a blacklist item.
    /// </summary>
    public const string UpdateBlacklistItem =
        @"UPDATE blacklist SET
                userid = @UserId,
                type = @Type,
                value = @Value,
                updatedat = now()
            WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to reset a password for a user.
    /// </summary>
    public const string ResetPasswordForUser =
        @"UPDATE mqttuser SET
                passwordhash = @PasswordHash,
                updatedat = now()
                WHERE id = @Id;";
}
