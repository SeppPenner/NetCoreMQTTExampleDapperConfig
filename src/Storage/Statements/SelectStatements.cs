// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectStatements.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The SQL statements for selecting data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Statements;

/// <summary>
///     The SQL statements for selecting data.
/// </summary>
public class SelectStatements
{
    /// <summary>
    ///     A SQL query string to select all users.
    /// </summary>
    public const string SelectAllUsers =
        @"SELECT id, username, passwordhash, clientidprefix, clientid, validateclientid, throttleuser, monthlybytelimit, createdat, updatedat, deletedat
            FROM mqttuser;";

    /// <summary>
    ///     A SQL query string to select all client id prefixes for all users.
    /// </summary>
    public const string SelectAllClientIdPrefixes =
        @"SELECT clientidprefix
            FROM mqttuser
            WHERE clientidprefix IS NOT NULL;";

    /// <summary>
    ///     A SQL query string to select the user by their identifier.
    /// </summary>
    public const string SelectUserById =
        @"SELECT id, username, passwordhash, clientidprefix, clientid, validateclientid, throttleuser, monthlybytelimit, createdat, updatedat, deletedat
            FROM mqttuser
            WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to select the user by their user name.
    /// </summary>
    public const string SelectUserByUserName =
        @"SELECT id, username, passwordhash, clientidprefix, clientid, validateclientid, throttleuser, monthlybytelimit, createdat, updatedat, deletedat
            FROM mqttuser
            WHERE username = @UserName;";

    /// <summary>
    ///     A SQL query string to select the user's name and identifier by their user name.
    /// </summary>
    public const string SelectUserNameAndIdByUserName =
        @"SELECT username, id
            FROM mqttuser
            WHERE username = @UserName;";

    /// <summary>
    ///     A SQL query string to select all database versions.
    /// </summary>
    public const string SelectAllDatabaseVersions =
        @"SELECT id, name, number, createdat, deletedat, updatedat
            FROM databaseversion;";

    /// <summary>
    ///     A SQL query string to select the database version by its identifier.
    /// </summary>
    public const string SelectDatabaseVersionById =
        @"SELECT id, name, number, createdat, deletedat, updatedat
            FROM databaseversion
            WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to select the database version by its name.
    /// </summary>
    public const string SelectDatabaseVersionByName =
        @"SELECT id, name, number, createdat, deletedat, updatedat
            FROM databaseversion
            WHERE name = @WidgetName;";

    /// <summary>
    ///     A SQL query string to select all whitelist items.
    /// </summary>
    public const string SelectAllWhitelistItems =
        @"SELECT id, userid, type, value, createdat, updatedat, deletedat
            FROM whitelist;";

    /// <summary>
    ///     A SQL query string to select a whitelist item by its id.
    /// </summary>
    public const string SelectWhitelistItemById =
        @"SELECT id, userid, type, value, createdat, updatedat, deletedat
            FROM whitelist
            WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to select a whitelist item by its id and type.
    /// </summary>
    public const string SelectWhitelistItemByIdAndType =
        @"SELECT id, userid, type, value, createdat, updatedat, deletedat
            FROM whitelist
            WHERE id = @Id
            AND type = @Type;";

    /// <summary>
    ///     A SQL query string to select a whitelist item by its type.
    /// </summary>
    public const string SelectWhitelistItemByType =
        @"SELECT id, userid, type, value, createdat, updatedat, deletedat
            FROM whitelist
            WHERE type = @Type;";

    /// <summary>
    ///     A SQL query string to select all blacklist items.
    /// </summary>
    public const string SelectAllBlacklistItems =
        @"SELECT id, userid, type, value, createdat, updatedat, deletedat
            FROM blacklist;";

    /// <summary>
    ///     A SQL query string to select a blacklist item by its id.
    /// </summary>
    public const string SelectBlacklistItemById =
        @"SELECT id, userid, type, value, createdat, updatedat, deletedat
            FROM blacklist
            WHERE id = @Id;";

    /// <summary>
    ///     A SQL query string to select a blacklist item by its id and type.
    /// </summary>
    public const string SelectBlacklistItemByIdAndType =
        @"SELECT id, userid, type, value, createdat, updatedat, deletedat
            FROM blacklist
            WHERE id = @Id
            AND type = @Type;";

    /// <summary>
    ///     A SQL query string to select a blacklist item by its type.
    /// </summary>
    public const string SelectBlacklistItemByType =
        @"SELECT id, userid, type, value, createdat, updatedat, deletedat
            FROM blacklist
            WHERE type = @Type;";

    /// <summary>
    ///     A SQL query string to select all blacklist items for a user.
    /// </summary>
    public const string SelectBlacklistItemsForUser =
        @"SELECT id, userid, type, value, createdat, updatedat, deletedat
            FROM blacklist
            WHERE userid = @UserId AND type = @Type;";

    /// <summary>
    ///     A SQL query string to select all whitelist items for a user.
    /// </summary>
    public const string SelectWhitelistItemsForUser =
        @"SELECT id, userid, type, value, createdat, updatedat, deletedat
            FROM whitelist
            WHERE userid = @UserId AND type = @Type;";
}
