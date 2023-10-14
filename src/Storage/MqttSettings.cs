// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MqttSettings.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   This class contains the MQTT server settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage;

/// <summary>
///     This class contains the MQTT server settings.
/// </summary>
public class MqttSettings
{
    /// <summary>
    ///     Gets or sets the port.
    /// </summary>
    public int Port { get; set; } = 8883;

    /// <summary>
    /// Gets or sets the delay in milliseconds.
    /// </summary>
    public int DelayInMilliSeconds { get; set; } = 3000;

    /// <summary>
    /// Checks whether the service is configured properly.
    /// </summary>
    /// <returns>A value indicating whether the service is configured properly.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the port or the delay is invalid.</exception>
    public bool IsValid()
    {
        if (this.Port < 1 || this.Port > 65535)
        {
            throw new InvalidOperationException("The port is invalid.");
        }

        if (this.DelayInMilliSeconds < 0)
        {
            throw new InvalidOperationException("The delay is invalid.");
        }

        return true;
    }
}
