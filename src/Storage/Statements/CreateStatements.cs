// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateStatements.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The SQL statements for table creation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Statements;

/// <summary>
///     The SQL statements for table creation.
/// </summary>
public class CreateStatements
{
    /// <summary>
    ///     A SQL query string to create the database.
    /// </summary>
    public const string CreateDatabase = @"CREATE DATABASE @Database;";

    /// <summary>
    ///     A SQL query string to create the database version table.
    /// </summary>
    public const string CreateDatabaseVersionTable =
        @"CREATE TABLE IF NOT EXISTS databaseversion (
            id                      UUID            NOT NULL PRIMARY KEY,
            name                    TEXT            NOT NULL,
            number                  BIGINT          NOT NULL,
            createdat               TIMESTAMPTZ     DEFAULT now(),
            updatedat               TIMESTAMPTZ     NULL,
            deletedat               TIMESTAMPTZ     NULL
        );";

    /// <summary>
    ///     A SQL query string to create the blacklist table.
    /// </summary>
    public const string CreateBlacklistTable =
        @"CREATE TABLE IF NOT EXISTS blacklist (
            id                      UUID            NOT NULL PRIMARY KEY,
            userid                  UUID            NOT NULL                REFERENCES mqttuser(id),
            type                    INTEGER         NOT NULL,
            value                   TEXT            NOT NULL,
            createdat               TIMESTAMPTZ     DEFAULT now(),
            updatedat               TIMESTAMPTZ     NULL,
            deletedat               TIMESTAMPTZ     NULL
        );";

    /// <summary>
    ///     A SQL query string to create the whitelist table.
    /// </summary>
    public const string CreateWhitelistTable =
        @"CREATE TABLE IF NOT EXISTS whitelist (
            id                      UUID            NOT NULL PRIMARY KEY,
            userid                  UUID            NOT NULL                REFERENCES mqttuser(id),
            type                    INTEGER         NOT NULL,
            value                   TEXT            NOT NULL,
            createdat               TIMESTAMPTZ     DEFAULT now(),
            updatedat               TIMESTAMPTZ     NULL,
            deletedat               TIMESTAMPTZ     NULL
        );";

    /// <summary>
    ///     A SQL query string to create the user table.
    /// </summary>
    public const string CreateUserTable =
        @"CREATE TABLE IF NOT EXISTS mqttuser (
            id                                      UUID            NOT NULL PRIMARY KEY,
            username                                TEXT            NOT NULL UNIQUE,
            passwordhash                            TEXT            NOT NULL,
            clientidprefix                          TEXT            NULL,
            clientid                                TEXT            NULL,
            validateclientid                        BOOLEAN         DEFAULT false,
            throttleuser                            BOOLEAN         DEFAULT false,
            monthlybytelimit                        BIGINT          NULL,
            createdat                               TIMESTAMPTZ     DEFAULT now(),
            updatedat                               TIMESTAMPTZ     NULL,
            deletedat                               TIMESTAMPTZ     NULL
        );";
}
