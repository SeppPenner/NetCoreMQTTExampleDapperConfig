// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllerBaseExtensions.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   Some extension methods for the <see cref="ControllerBase"></see> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleDapperConfig.Controllers.Extensions;

/// <summary>
///     Some extension methods for the <see cref="ControllerBase"></see> class.
/// </summary>
public static class ControllerBaseExtensions
{
    /// <summary>
    ///     Returns a 500 internal server error.
    /// </summary>
    /// <param name="controllerBase">The controller base.</param>
    /// <param name="ex">The exception.</param>
    /// <returns>A <seealso cref="ObjectResult" />.</returns>
    public static ObjectResult InternalServerError(this ControllerBase controllerBase, Exception ex)
    {
        return controllerBase.StatusCode(500, $"{ex.Message}{ex.StackTrace}");
    }
}
