// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExistsStatements.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The SQL statements for checking if a table exists.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Statements;

/// <summary>
///     The SQL statements for checking if a table exists.
/// </summary>
public class ExistsStatements
{
    /// <summary>
    ///     A SQL query string to check whether the database exists.
    /// </summary>
    public const string CheckDatabaseExists =
        @"SELECT EXISTS (
                SELECT datname
                FROM pg_catalog.pg_database
                WHERE datname = '@Database'
            );";

    /// <summary>
    ///     A SQL query string to check whether the database version table exists.
    /// </summary>
    public const string CheckDatabaseVersionTableExists =
        @"SELECT EXISTS (
                SELECT 1
                FROM   information_schema.tables 
                WHERE  table_schema = 'public'
                AND    table_name = 'databaseversion'
            );";

    /// <summary>
    ///     A SQL query string to check whether the blacklist table exists.
    /// </summary>
    public const string CheckBlacklistTableExists =
        @"SELECT EXISTS (
                SELECT 1
                FROM   information_schema.tables 
                WHERE  table_schema = 'public'
                AND    table_name = 'blacklist'
            );";

    /// <summary>
    ///     A SQL query string to check whether the whitelist table exists.
    /// </summary>
    public const string CheckWhitelistTableExists =
        @"SELECT EXISTS (
                SELECT 1
                FROM   information_schema.tables 
                WHERE  table_schema = 'public'
                AND    table_name = 'whitelist'
            );";

    /// <summary>
    ///     A SQL query string to check whether the user table exists.
    /// </summary>
    public const string CheckUserTableExists =
        @"SELECT EXISTS (
                SELECT 1
                FROM   information_schema.tables 
                WHERE  table_schema = 'public'
                AND    table_name = 'mqttuser'
            );";

    /// <summary>
    ///     A SQL query string to select whether a user name already exists or not.
    /// </summary>
    public const string UserNameExists =
        @"SELECT EXISTS (
                SELECT username FROM mqttuser
                WHERE username = @UserName
            );";
}
