// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MqttSettings.cs" company="Haemmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   This class contains the MQTT server settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage
{
    /// <summary>
    ///     This class contains the MQTT server settings.
    /// </summary>
    public class MqttSettings
    {
        /// <summary>
        ///     Gets or sets the port.
        /// </summary>
        public int Port { get; set; } = 8883;
    }
}
