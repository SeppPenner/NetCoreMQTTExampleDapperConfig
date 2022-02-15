// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlacklistRepository.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   An implementation supporting the repository pattern to work with <see cref="BlacklistWhitelist" />s.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Repositories.Implementation;

/// <inheritdoc cref="IBlacklistRepository" />
/// <summary>
///     An implementation supporting the repository pattern to work with <see cref="BlacklistWhitelist" />s.
/// </summary>
/// <seealso cref="IBlacklistRepository" />
public class BlacklistRepository : BaseRepository, IBlacklistRepository
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BlacklistRepository" /> class.
    /// </summary>
    /// <param name="connectionSettings">The connection settings to use.</param>
    public BlacklistRepository(DatabaseConnectionSettings connectionSettings) : base(connectionSettings)
    {
    }

    /// <inheritdoc cref="IBlacklistRepository" />
    /// <summary>
    ///     Gets a <see cref="List{T}" /> of all <see cref="BlacklistWhitelist" /> items.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IBlacklistRepository" />
    public async Task<List<BlacklistWhitelist>> GetAllBlacklistItems()
    {
        var connection = await this.GetDatabaseConnection();
        var blacklists = await connection.QueryAsync<BlacklistWhitelist>(SelectStatements.SelectAllBlacklistItems);
        return blacklists?.ToList() ?? new List<BlacklistWhitelist>();
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
        var connection = await this.GetDatabaseConnection();
        return await connection.QueryFirstOrDefaultAsync<BlacklistWhitelist>(
                   SelectStatements.SelectBlacklistItemById,
                   new { Id = blacklistItemId });
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
        var connection = await this.GetDatabaseConnection();
        return await connection.QueryFirstOrDefaultAsync<BlacklistWhitelist>(
                   SelectStatements.SelectBlacklistItemByIdAndType,
                   new { Id = blacklistItemId, Type = blacklistItemType });
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
        var connection = await this.GetDatabaseConnection();
        return await connection.QueryFirstOrDefaultAsync<BlacklistWhitelist>(
                   SelectStatements.SelectBlacklistItemByType,
                   new { Type = blacklistItemType });
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
        var connection = await this.GetDatabaseConnection();
        var result = await connection.ExecuteAsync(
                         UpdateStatements.MarkBlacklistItemAsDeleted,
                         new { Id = blacklistItemId });
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
        var connection = await this.GetDatabaseConnection();
        var result = await connection.ExecuteAsync(
                         DeleteStatements.DeleteBlacklistItem,
                         new { Id = blacklistItemId });
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
        var connection = await this.GetDatabaseConnection();
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
        var connection = await this.GetDatabaseConnection();
        var result = await connection.ExecuteAsync(UpdateStatements.UpdateBlacklistItem, blacklistItem);
        return result == 1;
    }
}
