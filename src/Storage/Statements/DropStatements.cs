// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DropStatements.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The SQL statements for table deletion.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Statements;

/// <summary>
///     The SQL statements for table deletion.
/// </summary>
public class DropStatements
{
    /// <summary>
    ///     A SQL query string to drop the database.
    /// </summary>
    public const string DropDatabase = @"DROP DATABASE IF EXISTS @Database;";

    /// <summary>
    ///     A SQL query string to delete the database version table.
    /// </summary>
    public const string DropDatabaseVersionTable = @"DROP TABLE IF EXISTS databaseversion;";

    /// <summary>
    ///     A SQL query string to delete the blacklist table.
    /// </summary>
    public const string DropBlacklistTable = @"DROP TABLE IF EXISTS blacklist;";

    /// <summary>
    ///     A SQL query string to delete the whitelist table.
    /// </summary>
    public const string DropWhitelistTable = @"DROP TABLE IF EXISTS whitelist;";

    /// <summary>
    ///     A SQL query string to delete the user table.
    /// </summary>
    public const string DropUserTable = @"DROP TABLE IF EXISTS mqttuser;";
}
