namespace Storage.Enumerations
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// An enumeration representing the available claims.
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