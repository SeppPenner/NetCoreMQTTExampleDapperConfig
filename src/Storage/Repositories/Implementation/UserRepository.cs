// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserRepository.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   An implementation supporting the repository pattern to work with <see cref="User" />s.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Repositories.Implementation;

/// <inheritdoc cref="IUserRepository" />
/// <summary>
///     An implementation supporting the repository pattern to work with <see cref="User" />s.
/// </summary>
/// <seealso cref="IUserRepository" />
public class UserRepository : BaseRepository, IUserRepository
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UserRepository" /> class.
    /// </summary>
    /// <param name="connectionSettings">The connection settings to use.</param>
    public UserRepository(DatabaseConnectionSettings connectionSettings) : base(connectionSettings)
    {
    }

    /// <inheritdoc cref="IUserRepository" />
    /// <summary>
    ///     Gets a <see cref="List{T}" /> of all <see cref="User" />s.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IUserRepository" />
    public async Task<List<User>> GetUsers()
    {
        var connection = await this.GetDatabaseConnection();
        var users = await connection.QueryAsync<User>(SelectStatements.SelectAllUsers);
        return users?.ToList() ?? new List<User>();
    }

    /// <inheritdoc cref="IUserRepository" />
    /// <summary>
    ///     Gets a <see cref="User" /> by their id.
    /// </summary>
    /// <param name="userId">The <see cref="User" />'s id to query for.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IUserRepository" />
    public async Task<User> GetUserById(Guid userId)
    {
        var connection = await this.GetDatabaseConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
                   SelectStatements.SelectUserById,
                   new { Id = userId });
    }

    /// <inheritdoc cref="IUserRepository" />
    /// <summary>
    ///     Gets a <see cref="User" /> by their user name.
    /// </summary>
    /// <param name="userName">The <see cref="User" />'s name to query for.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IUserRepository" />
    public async Task<User> GetUserByName(string userName)
    {
        var connection = await this.GetDatabaseConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
                   SelectStatements.SelectUserByUserName,
                   new { UserName = userName });
    }

    /// <summary>
    ///     Gets a <see cref="User" />'s name and identifier by their user name.
    /// </summary>
    /// <param name="userName">The <see cref="User" />'s name to query for.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    public async Task<(string, Guid)> GetUserNameAndUserIdByName(string userName)
    {
        var connection = await this.GetDatabaseConnection();
        return await connection.QueryFirstOrDefaultAsync<(string, Guid)>(
                   SelectStatements.SelectUserNameAndIdByUserName,
                   new { UserName = userName });
    }

    /// <inheritdoc cref="IUserRepository" />
    /// <summary>
    ///     Resets the password for a <see cref="User" />.
    /// </summary>
    /// <param name="userId">The user identifier to query for.</param>
    /// <param name="hashedPassword">The hashed password to set.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IUserRepository" />
    public async Task<bool> ResetPassword(Guid userId, string hashedPassword)
    {
        var connection = await this.GetDatabaseConnection();
        var result = await connection.ExecuteAsync(
                         UpdateStatements.ResetPasswordForUser,
                         new { Id = userId, PasswordHash = hashedPassword });
        return result == 1;
    }

    /// <inheritdoc cref="IUserRepository" />
    /// <summary>
    ///     Gets a <see cref="bool" /> value indicating whether the user name already exists or not.
    /// </summary>
    /// <param name="userName">The user name to query for.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IUserRepository" />
    public async Task<bool> UserNameExists(string userName)
    {
        var connection = await this.GetDatabaseConnection();
        return await connection.QueryFirstOrDefaultAsync<bool>(
                   ExistsStatements.UserNameExists,
                   new { UserName = userName });
    }

    /// <inheritdoc cref="IUserRepository" />
    /// <summary>
    ///     Sets the <see cref="User" />'s state to deleted. (It will still be present in the database, but with a deleted
    ///     timestamp).
    /// </summary>
    /// <param name="userId">The <see cref="User" />'s id.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
    /// <seealso cref="IUserRepository" />
    public async Task<bool> DeleteUser(Guid userId)
    {
        var connection = await this.GetDatabaseConnection();
        var result = await connection.ExecuteAsync(UpdateStatements.MarkUserAsDeleted, new { Id = userId });
        return result == 1;
    }

    /// <inheritdoc cref="IUserRepository" />
    /// <summary>
    ///     Deletes a <see cref="User" /> from the database.
    /// </summary>
    /// <param name="userId">The <see cref="User" />'s id.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation providing the number of affected rows.</returns>
    /// <seealso cref="IUserRepository" />
    public async Task<bool> DeleteUserFromDatabase(Guid userId)
    {
        var connection = await this.GetDatabaseConnection();
        var result = await connection.ExecuteAsync(DeleteStatements.DeleteUser, new { Id = userId });
        return result == 1;
    }

    /// <inheritdoc cref="IUserRepository" />
    /// <summary>
    ///     Inserts a <see cref="User" /> to the database.
    /// </summary>
    /// <param name="user">The <see cref="User" /> to insert.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IUserRepository" />
    public async Task<bool> InsertUser(User user)
    {
        var connection = await this.GetDatabaseConnection();
        var result = await connection.ExecuteAsync(InsertStatements.InsertUser, user);
        return result == 1;
    }

    /// <inheritdoc cref="IUserRepository" />
    /// <summary>
    ///     Updates a <see cref="User" /> in the database.
    /// </summary>
    /// <param name="user">The updated <see cref="User" />.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IUserRepository" />
    public async Task<bool> UpdateUser(User user)
    {
        var connection = await this.GetDatabaseConnection();
        var result = await connection.ExecuteAsync(UpdateStatements.UpdateUser, user);
        return result == 1;
    }

    /// <inheritdoc cref="IUserRepository" />
    /// <summary>
    ///     Gets the blacklist items for a <see cref="User" />.
    /// </summary>
    /// <param name="userId">The user identifier to query for.</param>
    /// <param name="type">The <see cref="BlacklistWhitelistType" />.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IUserRepository" />
    public async Task<IEnumerable<BlacklistWhitelist>> GetBlacklistItemsForUser(Guid userId, BlacklistWhitelistType type)
    {
        var connection = await this.GetDatabaseConnection();
        return await connection.QueryAsync<BlacklistWhitelist>(
                   SelectStatements.SelectBlacklistItemsForUser,
                   new { UserId = userId, Type = type });
    }

    /// <inheritdoc cref="IUserRepository" />
    /// <summary>
    ///     Gets the whitelist items for a <see cref="User" />.
    /// </summary>
    /// <param name="userId">The user identifier to query for.</param>
    /// <param name="type">The <see cref="BlacklistWhitelistType" />.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IUserRepository" />
    public async Task<IEnumerable<BlacklistWhitelist>> GetWhitelistItemsForUser(Guid userId, BlacklistWhitelistType type)
    {
        var connection = await this.GetDatabaseConnection();
        return await connection.QueryAsync<BlacklistWhitelist>(
                   SelectStatements.SelectWhitelistItemsForUser,
                   new { UserId = userId, Type = type });
    }

    /// <inheritdoc cref="IUserRepository" />
    /// <summary>
    ///     Gets the client id prefixes for all <see cref="User" />s.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <seealso cref="IUserRepository" />
    public async Task<IEnumerable<string>> GetAllClientIdPrefixes()
    {
        var connection = await this.GetDatabaseConnection();
        return await connection.QueryAsync<string>(SelectStatements.SelectAllClientIdPrefixes);
    }
}
