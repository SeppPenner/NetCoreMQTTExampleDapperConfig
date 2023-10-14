// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWhitelistRepository.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   An interface supporting the repository pattern to work with <see cref="BlacklistWhitelist" />s.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Repositories.Interfaces;

/// <summary>
///     An interface supporting the repository pattern to work with <see cref="BlacklistWhitelist" />s.
/// </summary>
public interface IWhitelistRepository
{
    /// <summary>
    ///     Gets a <see cref="List{T}" /> of all <see cref="BlacklistWhitelist" /> items.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    Task<List<BlacklistWhitelist>> GetAllWhitelistItems();

    /// <summary>
    ///     Gets a <see cref="BlacklistWhitelist" /> item by its id.
    /// </summary>
    /// <param name="whitelistItemId">The <see cref="BlacklistWhitelist" />'s id to query for.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    Task<BlacklistWhitelist> GetWhitelistItemById(Guid whitelistItemId);

    /// <summary>
    ///     Gets a <see cref="BlacklistWhitelist" /> item by its type.
    /// </summary>
    /// <param name="whitelistItemType">The <see cref="BlacklistWhitelistType" /> to query for.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    Task<BlacklistWhitelist> GetWhitelistItemByType(BlacklistWhitelistType whitelistItemType);

    /// <summary>
    ///     Gets a <see cref="BlacklistWhitelist" /> item by its type.
    /// </summary>
    /// <param name="whitelistItemId">The <see cref="BlacklistWhitelist" />'s id to query for.</param>
    /// <param name="whitelistItemType">The <see cref="BlacklistWhitelistType" /> to query for.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    Task<BlacklistWhitelist> GetWhitelistItemByIdAndType(Guid whitelistItemId, BlacklistWhitelistType whitelistItemType);

    /// <summary>
    ///     Sets the <see cref="BlacklistWhitelist" />'s state to deleted. (It will still be present in the database, but with
    ///     a deleted timestamp).
    ///     Returns the number of affected rows.
    /// </summary>
    /// <param name="whitelistItemId">The <see cref="BlacklistWhitelist" />'s id.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
    Task<bool> DeleteWhitelistItem(Guid whitelistItemId);

    /// <summary>
    ///     Deletes a <see cref="BlacklistWhitelist" /> item from the database.
    ///     Returns the number of affected rows.
    /// </summary>
    /// <param name="whitelistItemId">The <see cref="BlacklistWhitelist" />'s id.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
    Task<bool> DeleteWhitelistItemFromDatabase(Guid whitelistItemId);

    /// <summary>
    ///     Inserts a <see cref="BlacklistWhitelist" /> item to the database.
    /// </summary>
    /// <param name="whitelistItem">The <see cref="BlacklistWhitelist" /> item to insert.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    Task<bool> InsertWhitelistItem(BlacklistWhitelist whitelistItem);

    /// <summary>
    ///     Updates a <see cref="BlacklistWhitelist" /> item in the database.
    /// </summary>
    /// <param name="whitelistItem">The updated <see cref="BlacklistWhitelist" />.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    Task<bool> UpdateWhitelistItem(BlacklistWhitelist whitelistItem);
}
