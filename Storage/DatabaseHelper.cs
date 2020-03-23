using System;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Serilog;
using Storage.Statements;

namespace Storage
{
    /// <summary>
    ///     An implementation to work with the database.
    /// </summary>
    public class DatabaseHelper : IDatabaseHelper
    {
        /// <summary>
        ///     The connection settings to use.
        /// </summary>
        private readonly DatabaseConnectionSettings _connectionSettings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseHelper" /> class.
        /// </summary>
        /// <param name="connectionSettings">The connection settings to use.</param>
        public DatabaseHelper(DatabaseConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;
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
            await using var connection = new NpgsqlConnection(_connectionSettings.ToAdminConnectionString());
            await connection.OpenAsync();
            var checkDatabaseExists = connection.ExecuteScalar(ExistsStatements.CheckDatabaseExists, new {_connectionSettings.Database});

            if (Convert.ToBoolean(checkDatabaseExists) == false)
            {
                Log.Information("The database doesn't exist. I'm creating it.");
                var sql = CreateStatements.CreateDatabase.Replace("@Database", database);
                await connection.ExecuteAsync(sql);
                Log.Information("Created database.");
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
            await using var connection = new NpgsqlConnection(_connectionSettings.ToAdminConnectionString());
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
            await CreateDatabaseVersionTable(forceDelete);
            await CreateUserTable(forceDelete);
            await CreateWhitelistTable(forceDelete);
            await CreateBlacklistTable(forceDelete);
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
            await using var connection = new NpgsqlConnection(_connectionSettings.ToConnectionString());
            await connection.OpenAsync();

            if (forceDelete)
            {
                Log.Information("Force delete the database version table.");
                await connection.ExecuteAsync(DropStatements.DropDatabaseVersionTable);
                Log.Information("Deleted database version table.");
                await connection.ExecuteAsync(CreateStatements.CreateDatabaseVersionTable);
                Log.Information("Created database version table.");
            }
            else
            {
                var checkTableExistsResult = connection.ExecuteScalar(ExistsStatements.CheckDatabaseVersionTableExists);

                if (Convert.ToBoolean(checkTableExistsResult) == false)
                {
                    Log.Information("The database version table doesn't exist. I'm creating it.");
                    await connection.ExecuteAsync(CreateStatements.CreateDatabaseVersionTable);
                    Log.Information("Created database version table.");
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
            await using var connection = new NpgsqlConnection(_connectionSettings.ToConnectionString());
            await connection.OpenAsync();

            if (forceDelete)
            {
                Log.Information("Force delete the whitelist table.");
                await connection.ExecuteAsync(DropStatements.DropWhitelistTable);
                Log.Information("Deleted whitelist table.");
                await connection.ExecuteAsync(CreateStatements.CreateWhitelistTable);
                Log.Information("Created whitelist table.");
            }
            else
            {
                var checkTableExistsResult = connection.ExecuteScalar(ExistsStatements.CheckWhitelistTableExists);

                if (Convert.ToBoolean(checkTableExistsResult) == false)
                {
                    Log.Information("The whitelist table doesn't exist. I'm creating it.");
                    await connection.ExecuteAsync(CreateStatements.CreateWhitelistTable);
                    Log.Information("Created whitelist table.");
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
            await using var connection = new NpgsqlConnection(_connectionSettings.ToConnectionString());
            await connection.OpenAsync();

            if (forceDelete)
            {
                Log.Information("Force delete the blacklist table.");
                await connection.ExecuteAsync(DropStatements.DropBlacklistTable);
                Log.Information("Deleted blacklist table.");
                await connection.ExecuteAsync(CreateStatements.CreateBlacklistTable);
                Log.Information("Created blacklist table.");
            }
            else
            {
                var checkTableExistsResult = connection.ExecuteScalar(ExistsStatements.CheckBlacklistTableExists);

                if (Convert.ToBoolean(checkTableExistsResult) == false)
                {
                    Log.Information("The blacklist table doesn't exist. I'm creating it.");
                    await connection.ExecuteAsync(CreateStatements.CreateBlacklistTable);
                    Log.Information("Created blacklist table.");
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
            await using var connection = new NpgsqlConnection(_connectionSettings.ToConnectionString());
            await connection.OpenAsync();

            if (forceDelete)
            {
                Log.Information("Force delete the user table.");
                await connection.ExecuteAsync(DropStatements.DropUserTable);
                Log.Information("Deleted user table.");
                await connection.ExecuteAsync(CreateStatements.CreateUserTable);
                Log.Information("Created user table.");
            }
            else
            {
                var checkTableExistsResult = connection.ExecuteScalar(ExistsStatements.CheckUserTableExists);

                if (Convert.ToBoolean(checkTableExistsResult) == false)
                {
                    Log.Information("The user table doesn't exist. I'm creating it.");
                    await connection.ExecuteAsync(CreateStatements.CreateUserTable);
                    Log.Information("Created user table.");
                }
            }
        }
    }
}
