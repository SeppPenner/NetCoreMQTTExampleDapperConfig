// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseRepository.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The base repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Repositories.Implementation;

/// <summary>
/// The base repository.
/// </summary>
public class BaseRepository
{
    /// <summary>
    ///     The connection settings to use.
    /// </summary>
    protected readonly DatabaseConnectionSettings ConnectionSettings;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseRepository" /> class.
    /// </summary>
    /// <param name="connectionSettings">The connection settings to use.</param>
    public BaseRepository(DatabaseConnectionSettings connectionSettings)
    {
        this.ConnectionSettings = connectionSettings;
    }

    /// <summary>
    /// Gets a new database connection.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected async Task<NpgsqlConnection> GetDatabaseConnection()
    {
        await using var connection = new NpgsqlConnection(this.ConnectionSettings.ToConnectionString());
        await connection.OpenAsync();
        return connection;
    }
}
