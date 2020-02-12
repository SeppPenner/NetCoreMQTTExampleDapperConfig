using System;

namespace Storage.Dto
{
    /// <summary>
    ///     The user class to create or update a user.
    /// </summary>
    public class DtoCreateUpdateUser
    {
        /// <summary>
        ///     Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets the client identifier prefix.
        /// </summary>
        public string ClientIdPrefix { get; set; }

        /// <summary>
        ///     Gets or sets the client identifier.
        /// </summary>
        public string ClientId { get; set; }

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
        ///     Returns a <see cref="string"></see> representation of the <see cref="DtoCreateUpdateUser" /> class.
        /// </summary>
        /// <returns>A <see cref="string"></see> representation of the <see cref="DtoCreateUpdateUser" /> class.</returns>
        public override string ToString()
        {
            return
                $"{{{nameof(UserName)}: {UserName}, {nameof(ClientIdPrefix)}: {ClientIdPrefix}, {nameof(ClientId)}: {ClientId}, {nameof(ValidateClientId)}: {ValidateClientId}, {nameof(ClientId)}: {ClientId}, {nameof(ThrottleUser)}: {ThrottleUser}, {nameof(MonthlyByteLimit)}: {MonthlyByteLimit}, {nameof(CreatedAt)}: {CreatedAt}, {nameof(DeletedAt)}: {DeletedAt}, {nameof(UpdatedAt)}: {UpdatedAt}}}";
        }
    }
}
