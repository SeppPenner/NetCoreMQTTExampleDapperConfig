// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Haemmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The main program class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleDapperConfig
{
    using System.IO;
    using System.Reflection;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    using Serilog;

    /// <summary>
    ///     The main program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            var currentLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File(
                // ReSharper disable once AssignNullToNotNullAttribute
                Path.Combine(currentLocation, @"log\NetCoreMQTTExampleDapperConfig_.txt"),
                rollingInterval: RollingInterval.Day).CreateLogger();

            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        ///     Creates the web host builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The <see cref="IWebHostBuilder" />.</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}
