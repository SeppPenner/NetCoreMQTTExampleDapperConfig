// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DtoReadUser.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The user class returned from the controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage.Dto;

/// <summary>
///     The user class returned from the controller.
/// </summary>
public class DtoReadUser
{
    /// <summary>
    ///     Gets or sets the primary key.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    ///     Gets or sets the user name.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the client identifier prefix.
    /// </summary>
    public string ClientIdPrefix { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the client identifier.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets a value indicating whether the client id is validated or not.
    /// </summary>
    public bool ValidateClientId { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user is throttled after a certain limit or not.
    /// </summary>
    public bool ThrottleUser { get; set; }

    /// <summary>
    ///     Gets or sets a user's monthly limit in byte.
    /// </summary>
    public long? MonthlyByteLimit { get; set; }

    /// <summary>
    ///     Gets or sets the created at timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the deleted at timestamp.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    ///     Gets or sets the updated at timestamp.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; } = null;

    /// <summary>
    ///     Returns a <see cref="string"></see> representation of the <see cref="DtoReadUser" /> class.
    /// </summary>
    /// <returns>A <see cref="string"></see> representation of the <see cref="DtoReadUser" /> class.</returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
