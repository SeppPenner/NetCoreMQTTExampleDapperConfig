using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Storage.Database;
using Storage.Enumerations;
using Storage.Repositories.Interfaces;
using Storage.Statements;

namespace Storage.Repositories.Implementation
{
    /// <inheritdoc cref="IBlacklistRepository" />
    /// <summary>
    ///     An implementation supporting the repository pattern to work with <see cref="BlacklistWhitelist" />s.
    /// </summary>
    /// <seealso cref="IBlacklistRepository" />
    public class BlacklistRepository : IBlacklistRepository
    {
        /// <summary>
        ///     The connection settings to use.
        /// </summary>
        private readonly DatabaseConnectionSettings _connectionSettings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlacklistRepository" /> class.
        /// </summary>
        /// <param name="connectionSettings">The connection settings to use.</param>
        public BlacklistRepository(DatabaseConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;
        }

        /// <inheritdoc cref="IBlacklistRepository" />
        /// <summary>
        ///     Gets a <see cref="List{T}" /> of all <see cref="BlacklistWhitelist" /> items.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <seealso cref="IBlacklistRepository" />
        public async Task<IEnumerable<BlacklistWhitelist>> GetAllBlacklistItems()
        {
            await using var connection = new NpgsqlConnection(_connectionSettings.ToConnectionString());
            await connection.OpenAsync();
            return await connection.QueryAsync<BlacklistWhitelist>(SelectStatements.SelectAllBlacklistItems);
        }

        /// <inheritdoc cref="IBlacklistRepository" />
        /// <summary>
        ///     Gets a <see cref="BlacklistWhitelist" /> item by its id.
        /// </summary>
        /// <param name="blacklistItemId">The <see cref="BlacklistWhitelist" />'s id to query for.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <seealso cref="IBlacklistRepository" />
        public async Task<BlacklistWhitelist> GetBlacklistItemById(Guid blacklistItemId)
        {
            await using var connection = new NpgsqlConnection(_connectionSettings.ToConnectionString());
            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<BlacklistWhitelist>(SelectStatements.SelectBlacklistItemById, new {Id = blacklistItemId});
        }

        /// <inheritdoc cref="IBlacklistRepository" />
        /// <summary>
        ///     Gets a <see cref="BlacklistWhitelist" /> item by its type.
        /// </summary>
        /// <param name="blacklistItemId">The <see cref="BlacklistWhitelist" />'s id to query for.</param>
        /// <param name="blacklistItemType">The <see cref="BlacklistWhitelistType" /> to query for.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <seealso cref="IBlacklistRepository" />
        public async Task<BlacklistWhitelist> GetBlacklistItemByIdAndType(Guid blacklistItemId, BlacklistWhitelistType blacklistItemType)
        {
            await using var connection = new NpgsqlConnection(_connectionSettings.ToConnectionString());
            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<BlacklistWhitelist>(SelectStatements.SelectBlacklistItemByIdAndType, new {Id = blacklistItemId, Type = blacklistItemType});
        }

        /// <inheritdoc cref="IBlacklistRepository" />
        /// <summary>
        ///     Gets a <see cref="BlacklistWhitelist" /> item by its type.
        /// </summary>
        /// <param name="blacklistItemType">The <see cref="BlacklistWhitelist" />'s type to query for.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <seealso cref="IBlacklistRepository" />
        public async Task<BlacklistWhitelist> GetBlacklistItemByType(BlacklistWhitelistType blacklistItemType)
        {
            await using var connection = new NpgsqlConnection(_connectionSettings.ToConnectionString());
            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<BlacklistWhitelist>(SelectStatements.SelectBlacklistItemByType, new {Type = blacklistItemType});
        }

        /// <inheritdoc cref="IBlacklistRepository" />
        /// <summary>
        ///     Sets the <see cref="BlacklistWhitelist" />'s state to deleted. (It will still be present in the database, but with
        ///     a deleted timestamp).
        ///     Returns the number of affected rows.
        /// </summary>
        /// <param name="blacklistItemId">The <see cref="BlacklistWhitelist" />'s id.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
        /// <seealso cref="IBlacklistRepository" />
        public async Task<bool> DeleteBlacklistItem(Guid blacklistItemId)
        {
            await using var connection = new NpgsqlConnection(_connectionSettings.ToConnectionString());
            await connection.OpenAsync();
            var result = await connection.ExecuteAsync(UpdateStatements.MarkBlacklistItemAsDeleted, new {Id = blacklistItemId});
            return result == 1;
        }

        /// <inheritdoc cref="IBlacklistRepository" />
        /// <summary>
        ///     Deletes a <see cref="BlacklistWhitelist" /> item from the database.
        ///     Returns the number of affected rows.
        /// </summary>
        /// <param name="blacklistItemId">The <see cref="BlacklistWhitelist" />'s id.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
        /// <seealso cref="IBlacklistRepository" />
        public async Task<bool> DeleteBlacklistItemFromDatabase(Guid blacklistItemId)
        {
            await using var connection = new NpgsqlConnection(_connectionSettings.ToConnectionString());
            await connection.OpenAsync();
            var result = await connection.ExecuteAsync(DeleteStatements.DeleteBlacklistItem, new {Id = blacklistItemId});
            return result == 1;
        }

        /// <inheritdoc cref="IBlacklistRepository" />
        /// <summary>
        ///     Inserts a <see cref="BlacklistWhitelist" /> item to the database.
        /// </summary>
        /// <param name="blacklistItem">The <see cref="BlacklistWhitelist" /> item to insert.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <seealso cref="IBlacklistRepository" />
        public async Task<bool> InsertBlacklistItem(BlacklistWhitelist blacklistItem)
        {
            await using var connection = new NpgsqlConnection(_connectionSettings.ToConnectionString());
            await connection.OpenAsync();
            var result = await connection.ExecuteAsync(InsertStatements.InsertBlacklistItem, blacklistItem);
            return result == 1;
        }

        /// <inheritdoc cref="IBlacklistRepository" />
        /// <summary>
        ///     Updates a <see cref="BlacklistWhitelist" /> item in the database.
        /// </summary>
        /// <param name="blacklistItem">The updated <see cref="BlacklistWhitelist" />.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <seealso cref="IBlacklistRepository" />
        public async Task<bool> UpdateBlacklistItem(BlacklistWhitelist blacklistItem)
        {
            await using var connection = new NpgsqlConnection(_connectionSettings.ToConnectionString());
            await connection.OpenAsync();
            var result = await connection.ExecuteAsync(UpdateStatements.UpdateBlacklistItem, blacklistItem);
            return result == 1;
        }
    }
}
