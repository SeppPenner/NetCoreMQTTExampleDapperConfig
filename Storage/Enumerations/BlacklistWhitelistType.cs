using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Storage.Enumerations
{
    /// <summary>
    ///     An enumeration representing the available blacklist or whitelist types.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BlacklistWhitelistType
    {
        /// <summary>
        ///     The subscribe blacklist or whitelist type.
        /// </summary>
        Subscribe,

        /// <summary>
        ///     The publish blacklist or whitelist type.
        /// </summary>
        Publish
    }
}
