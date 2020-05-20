// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateStatements.cs" company="Haemmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The SQL statements for table creation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Statements
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     The SQL statements for table creation.
    /// </summary>
    public class CreateStatements
    {
        /// <summary>
        ///     A SQL query string to create the database.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string CreateDatabase = @"CREATE DATABASE @Database;";

        /// <summary>
        ///     A SQL query string to create the database version table.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string CreateDatabaseVersionTable =
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
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string CreateBlacklistTable =
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
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string CreateWhitelistTable =
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
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string CreateUserTable =
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
}
