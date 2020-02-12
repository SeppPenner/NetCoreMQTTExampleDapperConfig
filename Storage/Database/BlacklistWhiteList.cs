using System;
using Storage.Enumerations;

namespace Storage.Database
{
    /// <summary>
    ///     The blacklist or whitelist class.
    /// </summary>
    public class BlacklistWhitelist
    {
        /// <summary>
        ///     Gets or sets the primary key.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        ///     Gets or sets the user id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     Gets or sets the blacklist or whitelist type.
        /// </summary>
        public BlacklistWhitelistType Type { get; set; }

        /// <summary>
        ///     Gets or sets the blacklist or whitelist value.
        /// </summary>
        public string Value { get; set; }

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
        ///     Returns a <see cref="string"></see> representation of the <see cref="BlacklistWhitelist" /> class.
        /// </summary>
        /// <returns>A <see cref="string"></see> representation of the <see cref="BlacklistWhitelist" /> class.</returns>
        public override string ToString()
        {
            return $"{{{nameof(Id)}: {Id}, {nameof(UserId)}: {UserId}, {nameof(Type)}: {Type}, {nameof(Value)}: {Value}, {nameof(CreatedAt)}: {CreatedAt}, {nameof(DeletedAt)}: {DeletedAt}, {nameof(UpdatedAt)}: {UpdatedAt}}}";
        }
    }
}
