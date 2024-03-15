// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The startup class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleDapperConfig;

/// <summary>
///     The startup class.
/// </summary>
public class Startup
{
    /// <summary>
    /// The service name.
    /// </summary>
    private readonly AssemblyName serviceName = Assembly.GetExecutingAssembly().GetName();

    /// <summary>
    ///     Initializes a new instance of the <see cref="Startup" /> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    /// <summary>
    ///     Gets the configuration.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    ///     Configures the specified application.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="env">The env.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.EnvironmentName == Environments.Development)
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        // Use authentication.
        app.UseAuthentication();

        // Use response compression.
        app.UseResponseCompression();

        // Use swagger stuff
        app.UseOpenApi();
        app.UseSwaggerUi();

        // Use HTTPS.
        app.UseHttpsRedirection();

        _ = app.ApplicationServices.GetService<MqttService>();
    }

    /// <summary>
    ///     Configures the services.
    /// </summary>
    /// <param name="services">The services.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        // Added the custom configuration options
        services.Configure<DatabaseConnectionSettings>(this.Configuration.GetSection("DatabaseConnectionSettings"));
        services.Configure<MqttSettings>(this.Configuration.GetSection("MqttSettings"));

        // Load database connection settings
        var databaseConnection =
            this.Configuration.GetSection("DatabaseConnectionSettings").Get<DatabaseConnectionSettings>()
            ?? new DatabaseConnectionSettings();

        // Load MQTT configuration settings
        var mqttSettings = this.Configuration.GetSection("MqttSettings").Get<MqttSettings>();

        if (mqttSettings is null)
        {
            throw new ArgumentNullException(nameof(mqttSettings));
        }

        // Add database helper
        services.AddSingleton<IDatabaseHelper>(r => new DatabaseHelper(databaseConnection));

        // Add repositories
        services.AddSingleton<IDatabaseVersionRepository>(r => new DatabaseVersionRepository(databaseConnection));
        services.AddSingleton<IBlacklistRepository>(r => new BlacklistRepository(databaseConnection));
        services.AddSingleton<IWhitelistRepository>(r => new WhitelistRepository(databaseConnection));
        services.AddSingleton<IUserRepository>(r => new UserRepository(databaseConnection));

        // Add local repositories already
        var userRepository = new UserRepository(databaseConnection);

        // Add identity stuff
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

        // Add logger
        services.AddSingleton(Log.Logger);

        // Add response compression
        services.AddResponseCompression();

        // Add AutoMapper
        services.AddAutoMapper(typeof(UserProfile));

        // Add swagger
        // Add swagger document for the API
        services.AddOpenApiDocument(
            config =>
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                config.DocumentName = $"NetCoreMQTTExampleDapperConfig {version}";
                config.PostProcess = document =>
                {
                    document.Info.Version = $"{version}";
                    document.Info.Title = "NetCoreMQTTExampleDapperConfig";
                    document.Info.Description = "NetCoreMQTTExampleDapperConfig";
                };
            });

        // Read certificate
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var certificate = new X509Certificate2(
            Path.Combine(currentPath, "certificate.pfx"),
            "test",
            X509KeyStorageFlags.Exportable);

        // Workaround to have a hosted background service available by DI.
        services.AddSingleton(_ => new MqttService(mqttSettings, this.serviceName.Name ?? "MqttService", certificate, userRepository));
        services.AddSingleton<IHostedService>(p => p.GetRequiredService<MqttService>());

        services.AddMqttConnectionHandler();

        // Add the MVC stuff
        services.AddMvc();
    }
}
