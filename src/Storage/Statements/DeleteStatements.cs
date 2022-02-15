// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteStatements.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The SQL statements for deleting data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Statements;

/// <summary>
///     The SQL statements for deleting data.
/// </summary>
public class DeleteStatements
{
    /// <summary>
    ///     A SQL query string to delete a user from the database.
    /// </summary>
    public const string DeleteUser =
        @"DELETE FROM mqttuser WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to delete a database version from the database.
    /// </summary>
    public const string DeleteDatabaseVersion =
        @"DELETE FROM databaseversion WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to delete a blacklist item from the database.
    /// </summary>
    public const string DeleteBlacklistItem =
        @"DELETE FROM blacklist WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to delete a whitelist item from the database.
    /// </summary>
    public const string DeleteWhitelistItem =
        @"DELETE FROM whitelist WHERE id = @Id;";
}
