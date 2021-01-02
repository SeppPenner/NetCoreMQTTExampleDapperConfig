// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBlacklistRepository.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   An interface supporting the repository pattern to work with <see cref="BlacklistWhitelist" />s.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Storage.Database;
    using Storage.Enumerations;

    /// <summary>
    ///     An interface supporting the repository pattern to work with <see cref="BlacklistWhitelist" />s.
    /// </summary>
    public interface IBlacklistRepository
    {
        /// <summary>
        ///     Gets a <see cref="List{T}" /> of all <see cref="BlacklistWhitelist" /> items.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task<List<BlacklistWhitelist>> GetAllBlacklistItems();

        /// <summary>
        ///     Gets a <see cref="BlacklistWhitelist" /> item by its id.
        /// </summary>
        /// <param name="blacklistItemId">The <see cref="BlacklistWhitelist" />'s id to query for.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task<BlacklistWhitelist> GetBlacklistItemById(Guid blacklistItemId);

        /// <summary>
        ///     Gets a <see cref="BlacklistWhitelist" /> item by its type.
        /// </summary>
        /// <param name="blacklistItemType">The <see cref="BlacklistWhitelistType" /> to query for.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        // ReSharper disable once UnusedMember.Global
        Task<BlacklistWhitelist> GetBlacklistItemByType(BlacklistWhitelistType blacklistItemType);

        /// <summary>
        ///     Gets a <see cref="BlacklistWhitelist" /> item by its type.
        /// </summary>
        /// <param name="blacklistItemId">The <see cref="BlacklistWhitelist" />'s id to query for.</param>
        /// <param name="blacklistItemType">The <see cref="BlacklistWhitelistType" /> to query for.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task<BlacklistWhitelist> GetBlacklistItemByIdAndType(Guid blacklistItemId, BlacklistWhitelistType blacklistItemType);

        /// <summary>
        ///     Sets the <see cref="BlacklistWhitelist" />'s state to deleted. (It will still be present in the database, but with
        ///     a deleted timestamp).
        ///     Returns the number of affected rows.
        /// </summary>
        /// <param name="blacklistItemId">The <see cref="BlacklistWhitelist" />'s id.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
        Task<bool> DeleteBlacklistItem(Guid blacklistItemId);

        /// <summary>
        ///     Deletes a <see cref="BlacklistWhitelist" /> item from the database.
        ///     Returns the number of affected rows.
        /// </summary>
        /// <param name="blacklistItemId">The <see cref="BlacklistWhitelist" />'s id.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
        // ReSharper disable once UnusedMember.Global
        Task<bool> DeleteBlacklistItemFromDatabase(Guid blacklistItemId);

        /// <summary>
        ///     Inserts a <see cref="BlacklistWhitelist" /> item to the database.
        /// </summary>
        /// <param name="blacklistItem">The <see cref="BlacklistWhitelist" /> item to insert.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task<bool> InsertBlacklistItem(BlacklistWhitelist blacklistItem);

        /// <summary>
        ///     Updates a <see cref="BlacklistWhitelist" /> item in the database.
        /// </summary>
        /// <param name="blacklistItem">The updated <see cref="BlacklistWhitelist" />.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        // ReSharper disable once UnusedMember.Global
        Task<bool> UpdateBlacklistItem(BlacklistWhitelist blacklistItem);
    }
}
