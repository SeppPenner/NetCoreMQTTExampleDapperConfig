// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseHelper.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   An implementation to work with the database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage
{
    using System;
    using System.Threading.Tasks;

    using Dapper;

    using Npgsql;

    using Serilog;

    using Storage.Statements;

    /// <summary>
    ///     An implementation to work with the database.
    /// </summary>
    public class DatabaseHelper : IDatabaseHelper
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger = Log.ForContext<DatabaseHelper>();

        /// <summary>
        ///     The connection settings to use.
        /// </summary>
        private readonly DatabaseConnectionSettings connectionSettings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseHelper" /> class.
        /// </summary>
        /// <param name="connectionSettings">The connection settings to use.</param>
        public DatabaseHelper(DatabaseConnectionSettings connectionSettings)
        {
            this.connectionSettings = connectionSettings;
        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        ///     Creates the database.
        /// </summary>
        /// <param name="database">The database to create.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
        /// <seealso cref="IDatabaseHelper" />
        public async Task CreateDatabase(string database)
        {
            await using var connection = new NpgsqlConnection(this.connectionSettings.ToAdminConnectionString());
            await connection.OpenAsync();
            var checkDatabaseExists = await connection.ExecuteScalarAsync(ExistsStatements.CheckDatabaseExists, new {this.connectionSettings.Database});

            if (Convert.ToBoolean(checkDatabaseExists) == false)
            {
                this.logger.Information("The database doesn't exist. I'm creating it.");
                var sql = CreateStatements.CreateDatabase.Replace("@Database", database);
                await connection.ExecuteAsync(sql);
                this.logger.Information("Created database.");
            }

        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        ///     Deletes the database.
        /// </summary>
        /// <param name="database">The database to delete.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
        /// <seealso cref="IDatabaseHelper" />
        public async Task DeleteDatabase(string database)
        {
            await using var connection = new NpgsqlConnection(this.connectionSettings.ToAdminConnectionString());
            await connection.OpenAsync();
            var sql = DropStatements.DropDatabase.Replace("@Database", database);
            await connection.ExecuteAsync(sql);
        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        ///     Creates all tables.
        /// </summary>
        /// <param name="forceDelete">A <see cref="bool" /> value to force the deletion of the table.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
        /// <seealso cref="IDatabaseHelper" />
        public async Task CreateAllTables(bool forceDelete)
        {
            await this.CreateDatabaseVersionTable(forceDelete);
            await this.CreateUserTable(forceDelete);
            await this.CreateWhitelistTable(forceDelete);
            await this.CreateBlacklistTable(forceDelete);
        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        ///     Creates the database version table.
        /// </summary>
        /// <param name="forceDelete">A <see cref="bool" /> value to force the deletion of the table.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <seealso cref="IDatabaseHelper" />
        public async Task CreateDatabaseVersionTable(bool forceDelete)
        {
            await using var connection = new NpgsqlConnection(this.connectionSettings.ToConnectionString());
            await connection.OpenAsync();

            if (forceDelete)
            {
                this.logger.Information("Force delete the database version table.");
                await connection.ExecuteAsync(DropStatements.DropDatabaseVersionTable);
                this.logger.Information("Deleted database version table.");
                await connection.ExecuteAsync(CreateStatements.CreateDatabaseVersionTable);
                this.logger.Information("Created database version table.");
            }
            else
            {
                var checkTableExistsResult = await connection.ExecuteScalarAsync(ExistsStatements.CheckDatabaseVersionTableExists);

                if (Convert.ToBoolean(checkTableExistsResult) == false)
                {
                    this.logger.Information("The database version table doesn't exist. I'm creating it.");
                    await connection.ExecuteAsync(CreateStatements.CreateDatabaseVersionTable);
                    this.logger.Information("Created database version table.");
                }
            }
        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        ///     Creates the whitelist table.
        /// </summary>
        /// <param name="forceDelete">A <see cref="bool" /> value to force the deletion of the table.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <seealso cref="IDatabaseHelper" />
        public async Task CreateWhitelistTable(bool forceDelete)
        {
            await using var connection = new NpgsqlConnection(this.connectionSettings.ToConnectionString());
            await connection.OpenAsync();

            if (forceDelete)
            {
                this.logger.Information("Force delete the whitelist table.");
                await connection.ExecuteAsync(DropStatements.DropWhitelistTable);
                this.logger.Information("Deleted whitelist table.");
                await connection.ExecuteAsync(CreateStatements.CreateWhitelistTable);
                this.logger.Information("Created whitelist table.");
            }
            else
            {
                var checkTableExistsResult = await connection.ExecuteScalarAsync(ExistsStatements.CheckWhitelistTableExists);

                if (Convert.ToBoolean(checkTableExistsResult) == false)
                {
                    this.logger.Information("The whitelist table doesn't exist. I'm creating it.");
                    await connection.ExecuteAsync(CreateStatements.CreateWhitelistTable);
                    this.logger.Information("Created whitelist table.");
                }
            }
        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        ///     Creates the blacklist table.
        /// </summary>
        /// <param name="forceDelete">A <see cref="bool" /> value to force the deletion of the table.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <seealso cref="IDatabaseHelper" />
        public async Task CreateBlacklistTable(bool forceDelete)
        {
            await using var connection = new NpgsqlConnection(this.connectionSettings.ToConnectionString());
            await connection.OpenAsync();

            if (forceDelete)
            {
                this.logger.Information("Force delete the blacklist table.");
                await connection.ExecuteAsync(DropStatements.DropBlacklistTable);
                this.logger.Information("Deleted blacklist table.");
                await connection.ExecuteAsync(CreateStatements.CreateBlacklistTable);
                this.logger.Information("Created blacklist table.");
            }
            else
            {
                var checkTableExistsResult = await connection.ExecuteScalarAsync(ExistsStatements.CheckBlacklistTableExists);

                if (Convert.ToBoolean(checkTableExistsResult) == false)
                {
                    this.logger.Information("The blacklist table doesn't exist. I'm creating it.");
                    await connection.ExecuteAsync(CreateStatements.CreateBlacklistTable);
                    this.logger.Information("Created blacklist table.");
                }
            }
        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        ///     Creates the user table.
        /// </summary>
        /// <param name="forceDelete">A <see cref="bool" /> value to force the deletion of the table.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <seealso cref="IDatabaseHelper" />
        public async Task CreateUserTable(bool forceDelete)
        {
            await using var connection = new NpgsqlConnection(this.connectionSettings.ToConnectionString());
            await connection.OpenAsync();

            if (forceDelete)
            {
                this.logger.Information("Force delete the user table.");
                await connection.ExecuteAsync(DropStatements.DropUserTable);
                this.logger.Information("Deleted user table.");
                await connection.ExecuteAsync(CreateStatements.CreateUserTable);
                this.logger.Information("Created user table.");
            }
            else
            {
                var checkTableExistsResult = await connection.ExecuteScalarAsync(ExistsStatements.CheckUserTableExists);

                if (Convert.ToBoolean(checkTableExistsResult) == false)
                {
                    this.logger.Information("The user table doesn't exist. I'm creating it.");
                    await connection.ExecuteAsync(CreateStatements.CreateUserTable);
                    this.logger.Information("Created user table.");
                }
            }
        }
    }
}
