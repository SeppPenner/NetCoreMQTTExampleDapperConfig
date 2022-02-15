// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The main program class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleDapperConfig;

/// <summary>
///     The main program class.
/// </summary>
public static class Program
{
    /// <summary>
    ///     Defines the entry point of the application.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    public static Task Main(string[] args)
    {
        var currentLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File(
            Path.Combine(currentLocation, @"log\NetCoreMQTTExampleDapperConfig_.txt") ?? string.Empty,
            rollingInterval: RollingInterval.Day).CreateLogger();

        return CreateHostBuilder(args, currentLocation).Build().RunAsync();
    }

    /// <summary>
    /// Creates the host builder.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="currentLocation">The current assembly location.</param>
    /// <returns>A new <see cref="IHostBuilder"/>.</returns>
    private static IHostBuilder CreateHostBuilder(string[] args, string currentLocation) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(
                webBuilder =>
                    {
                        webBuilder.UseSerilog();
                        webBuilder.UseContentRoot(currentLocation);
                        webBuilder.UseStartup<Startup>();
                        webBuilder.UseKestrel(
                            o =>
                                {
                                    o.ListenAnyIP(1883, l => l.UseMqtt());
                                    o.ListenAnyIP(5000);
                                });
                    })
            .UseSerilog()
            .UseWindowsService()
            .UseSystemd();
}
