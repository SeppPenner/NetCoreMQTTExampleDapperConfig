// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InsertStatements.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The SQL statements for inserting data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Statements;

/// <summary>
///     The SQL statements for inserting data.
/// </summary>
public class InsertStatements
{
    /// <summary>
    ///     A SQL query string to insert a user.
    /// </summary>
    public const string InsertUser =
        @"INSERT INTO mqttuser (id, username, passwordhash, clientidprefix, clientid, validateclientid, throttleuser, monthlybytelimit)
            VALUES (@Id, @UserName, @PasswordHash, ClientIdPrefix, ClientId, ValidateClientId, ThrottleUser, MonthlyByteLimit);";

    /// <summary>
    ///     A SQL query string to insert a database version.
    /// </summary>
    public const string InsertDatabaseVersion =
        @"INSERT INTO databaseversion (id, name, number)
            VALUES (@Id, @Name, @Number);";

    /// <summary>
    ///     A SQL query string to insert a blacklist item.
    /// </summary>
    public const string InsertBlacklistItem =
        @"INSERT INTO blacklist (id, userid, type, value)
            VALUES (@Id, @UserId, @Type, @Value);";

    /// <summary>
    ///     A SQL query string to insert a whitelist item.
    /// </summary>
    public const string InsertWhitelistItem =
        @"INSERT INTO whitelist (id, userid, type, value)
            VALUES (@Id, @UserId, @Type, @Value);";
}
