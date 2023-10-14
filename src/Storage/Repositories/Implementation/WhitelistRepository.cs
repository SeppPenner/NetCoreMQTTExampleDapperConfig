// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhitelistRepository.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   An implementation supporting the repository pattern to work with <see cref="BlacklistWhitelist" />s.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Repositories.Implementation;

/// <inheritdoc cref="IWhitelistRepository" />
/// <summary>
///     An implementation supporting the repository pattern to work with <see cref="BlacklistWhitelist" />s.
/// </summary>
/// <seealso cref="IWhitelistRepository" />
public class WhitelistRepository : BaseRepository, IWhitelistRepository
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WhitelistRepository" /> class.
    /// </summary>
    /// <param name="connectionSettings">The connection settings to use.</param>
    public WhitelistRepository(DatabaseConnectionSettings connectionSettings) : base(connectionSettings)
    {
    }

    /// <inheritdoc cref="IWhitelistRepository" />
    /// <summary>
    ///     Gets a <see cref="List{T}" /> of all <see cref="BlacklistWhitelist" /> items.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IWhitelistRepository" />
    public async Task<List<BlacklistWhitelist>> GetAllWhitelistItems()
    {
        var connection = await this.GetDatabaseConnection();
        var whiteLists = await connection.QueryAsync<BlacklistWhitelist>(SelectStatements.SelectAllWhitelistItems);
        return whiteLists?.ToList() ?? new List<BlacklistWhitelist>();
    }

    /// <inheritdoc cref="IWhitelistRepository" />
    /// <summary>
    ///     Gets a <see cref="BlacklistWhitelist" /> item by its id.
    /// </summary>
    /// <param name="whitelistItemId">The <see cref="BlacklistWhitelist" />'s id to query for.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IWhitelistRepository" />
    public async Task<BlacklistWhitelist> GetWhitelistItemById(Guid whitelistItemId)
    {
        var connection = await this.GetDatabaseConnection();
        return await connection.QueryFirstOrDefaultAsync<BlacklistWhitelist>(
                   SelectStatements.SelectWhitelistItemById,
                   new { Id = whitelistItemId });
    }

    /// <inheritdoc cref="IBlacklistRepository" />
    /// <summary>
    ///     Gets a <see cref="BlacklistWhitelist" /> item by its type.
    /// </summary>
    /// <param name="whitelistItemId">The <see cref="BlacklistWhitelist" />'s id to query for.</param>
    /// <param name="whitelistItemType">The <see cref="BlacklistWhitelistType" /> to query for.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IBlacklistRepository" />
    public async Task<BlacklistWhitelist> GetWhitelistItemByIdAndType(Guid whitelistItemId, BlacklistWhitelistType whitelistItemType)
    {
        var connection = await this.GetDatabaseConnection();
        return await connection.QueryFirstOrDefaultAsync<BlacklistWhitelist>(
                   SelectStatements.SelectWhitelistItemByIdAndType,
                   new { Id = whitelistItemId, Type = whitelistItemType });
    }

    /// <inheritdoc cref="IWhitelistRepository" />
    /// <summary>
    ///     Gets a <see cref="BlacklistWhitelist" /> item by its type.
    /// </summary>
    /// <param name="whitelistItemType">The <see cref="BlacklistWhitelist" />'s type to query for.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IWhitelistRepository" />
    public async Task<BlacklistWhitelist> GetWhitelistItemByType(BlacklistWhitelistType whitelistItemType)
    {
        var connection = await this.GetDatabaseConnection();
        return await connection.QueryFirstOrDefaultAsync<BlacklistWhitelist>(
                   SelectStatements.SelectWhitelistItemByType,
                   new { Type = whitelistItemType });
    }

    /// <inheritdoc cref="IWhitelistRepository" />
    /// <summary>
    ///     Sets the <see cref="BlacklistWhitelist" />'s state to deleted. (It will still be present in the database, but with
    ///     a deleted timestamp).
    ///     Returns the number of affected rows.
    /// </summary>
    /// <param name="whitelistItemId">The <see cref="BlacklistWhitelist" />'s id.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
    /// <seealso cref="IWhitelistRepository" />
    public async Task<bool> DeleteWhitelistItem(Guid whitelistItemId)
    {
        var connection = await this.GetDatabaseConnection();
        var result = await connection.ExecuteAsync(
                         UpdateStatements.MarkWhitelistItemAsDeleted,
                         new { Id = whitelistItemId });
        return result == 1;
    }

    /// <inheritdoc cref="IWhitelistRepository" />
    /// <summary>
    ///     Deletes a <see cref="BlacklistWhitelist" /> item from the database.
    ///     Returns the number of affected rows.
    /// </summary>
    /// <param name="whitelistItemId">The <see cref="BlacklistWhitelist" />'s id.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
    /// <seealso cref="IWhitelistRepository" />
    public async Task<bool> DeleteWhitelistItemFromDatabase(Guid whitelistItemId)
    {
        var connection = await this.GetDatabaseConnection();
        var result = await connection.ExecuteAsync(
                         DeleteStatements.DeleteWhitelistItem,
                         new { Id = whitelistItemId });
        return result == 1;
    }

    /// <inheritdoc cref="IWhitelistRepository" />
    /// <summary>
    ///     Inserts a <see cref="BlacklistWhitelist" /> item to the database.
    /// </summary>
    /// <param name="whitelistItem">The <see cref="BlacklistWhitelist" /> item to insert.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IWhitelistRepository" />
    public async Task<bool> InsertWhitelistItem(BlacklistWhitelist whitelistItem)
    {
        var connection = await this.GetDatabaseConnection();
        var result = await connection.ExecuteAsync(InsertStatements.InsertWhitelistItem, whitelistItem);
        return result == 1;
    }

    /// <inheritdoc cref="IWhitelistRepository" />
    /// <summary>
    ///     Updates a <see cref="BlacklistWhitelist" /> item in the database.
    /// </summary>
    /// <param name="whitelistItem">The updated <see cref="BlacklistWhitelist" />.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IWhitelistRepository" />
    public async Task<bool> UpdateWhitelistItem(BlacklistWhitelist whitelistItem)
    {
        var connection = await this.GetDatabaseConnection();
        var result = await connection.ExecuteAsync(UpdateStatements.UpdateWhitelistItem, whitelistItem);
        return result == 1;
    }
}
