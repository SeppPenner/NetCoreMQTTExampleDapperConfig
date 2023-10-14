// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseVersionRepository.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   An interface supporting the repository pattern to work with <see cref="DatabaseVersion" />s.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Repositories.Interfaces;

/// <summary>
///     An interface supporting the repository pattern to work with <see cref="DatabaseVersion" />s.
/// </summary>
public interface IDatabaseVersionRepository
{
    /// <summary>
    ///     Gets a <see cref="List{T}" /> of all <see cref="DatabaseVersion" />s.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    Task<List<DatabaseVersion>> GetDatabaseVersions();

    /// <summary>
    ///     Gets a <see cref="DatabaseVersion" /> by its id.
    /// </summary>
    /// <param name="databaseVersionId">The The <see cref="DatabaseVersion" />'s id to query for.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    Task<DatabaseVersion> GetDatabaseVersionById(Guid databaseVersionId);

    /// <summary>
    ///     Gets a <see cref="DatabaseVersion" /> by its name.
    /// </summary>
    /// <param name="databaseVersionName">The <see cref="DatabaseVersion" />'s name to query for.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    Task<DatabaseVersion> GetDatabaseVersionByName(string databaseVersionName);

    /// <summary>
    ///     Inserts a <see cref="DatabaseVersion" /> to the database.
    /// </summary>
    /// <param name="databaseVersion">The <see cref="DatabaseVersion" /> to insert.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    Task<bool> InsertDatabaseVersion(DatabaseVersion databaseVersion);

    /// <summary>
    ///     Sets the <see cref="DatabaseVersion" />'s state to deleted. (It will still be present in the database, but with a
    ///     deleted timestamp).
    ///     Returns the number of affected rows.
    /// </summary>
    /// <param name="databaseVersionId">The <see cref="DatabaseVersion" />'s id.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
    Task<bool> DeleteDatabaseVersion(Guid databaseVersionId);

    /// <summary>
    ///     Deletes a <see cref="DatabaseVersion" /> from the database.
    ///     Returns the number of affected rows.
    /// </summary>
    /// <param name="databaseVersionId">The <see cref="DatabaseVersion" />'s id.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
    Task<bool> DeleteDatabaseVersionFromDatabase(Guid databaseVersionId);

    /// <summary>
    ///     Updates a <see cref="DatabaseVersion" /> in the database.
    /// </summary>
    /// <param name="databaseVersion">The updated <see cref="DatabaseVersion" />.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    Task<bool> UpdateDatabaseVersion(DatabaseVersion databaseVersion);
}
