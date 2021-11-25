// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The startup class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleDapperConfig
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using MQTTnet.AspNetCore.Extensions;
    using MQTTnet.Protocol;
    using MQTTnet.Server;

    using Serilog;

    using Storage;
    using Storage.Database;
    using Storage.Enumerations;
    using Storage.Mappings;
    using Storage.Repositories.Implementation;
    using Storage.Repositories.Interfaces;

    using TopicCheck;

    /// <summary>
    ///     The startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILogger Logger = Log.ForContext<Startup>();

        /// <summary>
        ///     The <see cref="PasswordHasher{TUser}" />.
        /// </summary>
        private static readonly IPasswordHasher<User> Hasher = new PasswordHasher<User>();

        /// <summary>
        ///     Gets or sets the data limit cache for throttling for monthly data.
        /// </summary>
        private static readonly MemoryCache DataLimitCacheMonth = MemoryCache.Default;

        /// <summary>
        ///     The <see cref="IUserRepository" />.
        /// </summary>
        private IUserRepository userRepository;

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
        // ReSharper disable once UnusedMember.Global
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
            app.UseSwaggerUi3();

            // Use HTTPS.
            app.UseHttpsRedirection();
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

            // Add database helper
            services.AddSingleton<IDatabaseHelper>(r => new DatabaseHelper(databaseConnection));

            // Add repositories
            services.AddSingleton<IDatabaseVersionRepository>(r => new DatabaseVersionRepository(databaseConnection));
            services.AddSingleton<IBlacklistRepository>(r => new BlacklistRepository(databaseConnection));
            services.AddSingleton<IWhitelistRepository>(r => new WhitelistRepository(databaseConnection));
            services.AddSingleton<IUserRepository>(r => new UserRepository(databaseConnection));

            // Add local repositories already
            this.userRepository = new UserRepository(databaseConnection);

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
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var certificate = new X509Certificate2(
                // ReSharper disable once AssignNullToNotNullAttribute
                Path.Combine(currentPath, "certificate.pfx"),
                "test",
                X509KeyStorageFlags.Exportable);

            // Add MQTT stuff
            services.AddHostedMqttServer(
                builder => builder
#if DEBUG
                    .WithDefaultEndpoint().WithDefaultEndpointPort(1883)
#else
                    .WithoutDefaultEndpoint()
#endif
                    .WithEncryptedEndpoint().WithEncryptedEndpointPort(mqttSettings.Port)
                    .WithEncryptionCertificate(certificate.Export(X509ContentType.Pfx))
                    .WithEncryptionSslProtocol(SslProtocols.Tls12).WithConnectionValidator(this.ValidateConnection).WithSubscriptionInterceptor(this.ValidateSubscription).WithApplicationMessageInterceptor(this.ValidatePublish));

            services.AddMqttConnectionHandler();

            // Add the MVC stuff
            services.AddMvc();
        }

        /// <summary>
        ///     Logs the message from the MQTT subscription interceptor context.
        /// </summary>
        /// <param name="context">The MQTT subscription interceptor context.</param>
        /// <param name="successful">A <see cref="bool" /> value indicating whether the subscription was successful or not.</param>
        private static void LogMessage(MqttSubscriptionInterceptorContext context, bool successful)
        {
            if (context == null)
            {
                return;
            }

            Logger.Information(
                successful
                    ? "New subscription: ClientId = {@ClientId}, TopicFilter = {@TopicFilter}"
                    : "Subscription failed for clientId = {@ClientId}, TopicFilter = {@TopicFilter}",
                context.ClientId,
                context.TopicFilter);
        }

        /// <summary>
        ///     Logs the message from the MQTT message interceptor context.
        /// </summary>
        /// <param name="context">The MQTT message interceptor context.</param>
        private static void LogMessage(MqttApplicationMessageInterceptorContext context)
        {
            if (context == null)
            {
                return;
            }

            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);

            Logger.Information(
                "Message: ClientId = {@ClientId}, Topic = {@Topic}, Payload = {@Payload}, QoS = {@QoS}, Retain-Flag = {@RetainFlag}",
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain);
        }

        /// <summary>
        ///     Logs the message from the MQTT connection validation context.
        /// </summary>
        /// <param name="context">The MQTT connection validation context.</param>
        /// <param name="showPassword">A <see cref="bool" /> value indicating whether the password is written to the log or not.</param>
        private static void LogMessage(MqttConnectionValidatorContext context, bool showPassword)
        {
            if (context == null)
            {
                return;
            }

            if (showPassword)
            {
                Logger.Information(
                    "New connection: ClientId = {@ClientId}, Endpoint = {@Endpoint}, Username = {@UserName}, Password = {@Password}, CleanSession = {@CleanSession}",
                    context.ClientId,
                    context.Endpoint,
                    context.Username,
                    context.Password,
                    context.CleanSession);
            }
            else
            {
                Logger.Information(
                    "New connection: ClientId = {@ClientId}, Endpoint = {@Endpoint}, Username = {@UserName}, CleanSession = {@CleanSession}",
                    context.ClientId,
                    context.Endpoint,
                    context.Username,
                    context.CleanSession);
            }
        }

        /// <summary>
        ///     Checks whether a user has used the maximum of its publishing limit for the month or not.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="sizeInBytes">The message size in bytes.</param>
        /// <param name="monthlyByteLimit">The monthly byte limit.</param>
        /// <returns>A value indicating whether the user will be throttled or not.</returns>
        private static bool IsUserThrottled(string clientId, long sizeInBytes, long monthlyByteLimit)
        {
            var foundUserInCache = DataLimitCacheMonth.GetCacheItem(clientId);

            if (foundUserInCache == null)
            {
                DataLimitCacheMonth.Add(clientId, sizeInBytes, DateTimeOffset.Now.EndOfCurrentMonth());

                if (sizeInBytes < monthlyByteLimit)
                {
                    return false;
                }

                Logger.Information("The client with client id {@ClientId} is now locked until the end of this month because it already used its data limit.", clientId);
                return true;
            }

            try
            {
                var currentValue = Convert.ToInt64(foundUserInCache.Value);
                currentValue = checked(currentValue + sizeInBytes);
                DataLimitCacheMonth[clientId] = currentValue;

                if (currentValue >= monthlyByteLimit)
                {
                    Logger.Information("The client with client id {@ClientId} is now locked until the end of this month because it already used its data limit.", clientId);
                    return true;
                }
            }
            catch (OverflowException)
            {
                Logger.Information("OverflowException thrown.");
                Logger.Information("The client with client id {@ClientId} is now locked until the end of this month because it already used its data limit.", clientId);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Validates the message publication.
        /// </summary>
        /// <param name="context">The context.</param>
        private async void ValidatePublish(MqttApplicationMessageInterceptorContext context)
        {
            var clientIdPrefix = await this.GetClientIdPrefix(context.ClientId);
            User currentUser;
            bool userFound;

            if (string.IsNullOrWhiteSpace(clientIdPrefix))
            {
                userFound = context.SessionItems.TryGetValue(context.ClientId, out var currentUserObject);
                currentUser = currentUserObject as User;
            }
            else
            {
                userFound = context.SessionItems.TryGetValue(clientIdPrefix, out var currentUserObject);
                currentUser = currentUserObject as User;
            }

            if (!userFound || currentUser == null)
            {
                context.AcceptPublish = false;
                return;
            }

            var topic = context.ApplicationMessage.Topic;

            if (currentUser.ThrottleUser)
            {
                var payload = context.ApplicationMessage?.Payload;

                if (payload != null)
                {
                    if (currentUser.MonthlyByteLimit != null)
                    {
                        if (IsUserThrottled(context.ClientId, payload.Length, currentUser.MonthlyByteLimit.Value))
                        {
                            context.AcceptPublish = false;
                            return;
                        }
                    }
                }
            }

            // Get blacklist
            var publishBlackList = await this.userRepository.GetBlacklistItemsForUser(currentUser.Id, BlacklistWhitelistType.Publish);
            var blacklist = publishBlackList?.ToList() ?? new List<BlacklistWhitelist>();

            // Get whitelist
            var publishWhitelist = await this.userRepository.GetWhitelistItemsForUser(currentUser.Id, BlacklistWhitelistType.Publish);
            var whitelist = publishWhitelist?.ToList() ?? new List<BlacklistWhitelist>();

            // Check matches
            if (blacklist.Any(b => b.Value == topic))
            {
                context.AcceptPublish = false;
                return;
            }

            if (whitelist.Any(b => b.Value == topic))
            {
                context.AcceptPublish = true;
                LogMessage(context);
                return;
            }

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var forbiddenTopic in blacklist)
            {
                var doesTopicMatch = TopicChecker.Regex(forbiddenTopic.Value, topic);

                if (!doesTopicMatch)
                {
                    continue;
                }

                context.AcceptPublish = false;
                return;
            }

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var allowedTopic in whitelist)
            {
                var doesTopicMatch = TopicChecker.Regex(allowedTopic.Value, topic);

                if (!doesTopicMatch)
                {
                    continue;
                }

                context.AcceptPublish = true;
                LogMessage(context);
                return;
            }

            context.AcceptPublish = false;
        }

        /// <summary>
        ///     Validates the subscription.
        /// </summary>
        /// <param name="context">The context.</param>
        private async void ValidateSubscription(MqttSubscriptionInterceptorContext context)
        {
            var clientIdPrefix = await this.GetClientIdPrefix(context.ClientId);
            User currentUser;
            bool userFound;

            if (string.IsNullOrWhiteSpace(clientIdPrefix))
            {
                userFound = context.SessionItems.TryGetValue(context.ClientId, out var currentUserObject);
                currentUser = currentUserObject as User;
            }
            else
            {
                userFound = context.SessionItems.TryGetValue(clientIdPrefix, out var currentUserObject);
                currentUser = currentUserObject as User;
            }

            if (!userFound || currentUser == null)
            {
                context.AcceptSubscription = false;
                LogMessage(context, false);
                return;
            }

            var topic = context.TopicFilter.Topic;

            // Get blacklist
            var publishBlackList = await this.userRepository.GetBlacklistItemsForUser(currentUser.Id, BlacklistWhitelistType.Subscribe);
            var blacklist = publishBlackList?.ToList() ?? new List<BlacklistWhitelist>();

            // Get whitelist
            var publishWhitelist = await this.userRepository.GetWhitelistItemsForUser(currentUser.Id, BlacklistWhitelistType.Subscribe);
            var whitelist = publishWhitelist?.ToList() ?? new List<BlacklistWhitelist>();

            // Check matches
            if (blacklist.Any(b => b.Value == topic))
            {
                context.AcceptSubscription = false;
                LogMessage(context, false);
                return;
            }

            if (whitelist.Any(b => b.Value == topic))
            {
                context.AcceptSubscription = true;
                LogMessage(context, true);
                return;
            }

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var forbiddenTopic in blacklist)
            {
                var doesTopicMatch = TopicChecker.Regex(forbiddenTopic.Value, topic);

                if (!doesTopicMatch)
                {
                    continue;
                }

                context.AcceptSubscription = false;
                LogMessage(context, false);
                return;
            }

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var allowedTopic in whitelist)
            {
                var doesTopicMatch = TopicChecker.Regex(allowedTopic.Value, topic);

                if (!doesTopicMatch)
                {
                    continue;
                }

                context.AcceptSubscription = true;
                LogMessage(context, true);
                return;
            }

            context.AcceptSubscription = false;
            LogMessage(context, false);
        }

        /// <summary>
        ///     Validates the connection.
        /// </summary>
        /// <param name="context">The context.</param>
        private async void ValidateConnection(MqttConnectionValidatorContext context)
        {
            var currentUser = await this.userRepository.GetUserByName(context.Username).ConfigureAwait(false);

            if (currentUser == null)
            {
                context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                LogMessage(context, true);
                return;
            }

            if (context.Username != currentUser.UserName)
            {
                context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                LogMessage(context, true);
                return;
            }

            var hashingResult = Hasher.VerifyHashedPassword(
                currentUser,
                currentUser.PasswordHash,
                context.Password);

            if (hashingResult == PasswordVerificationResult.Failed)
            {
                context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                LogMessage(context, true);
                return;
            }

            if (!currentUser.ValidateClientId)
            {
                context.ReasonCode = MqttConnectReasonCode.Success;
                context.SessionItems.Add(context.ClientId, currentUser);
                LogMessage(context, false);
                return;
            }

            if (string.IsNullOrWhiteSpace(currentUser.ClientIdPrefix))
            {
                if (context.ClientId != currentUser.ClientId)
                {
                    context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    LogMessage(context, true);
                    return;
                }

                context.SessionItems.Add(currentUser.ClientId, currentUser);
            }
            else
            {
                context.SessionItems.Add(currentUser.ClientIdPrefix, currentUser);
            }

            context.ReasonCode = MqttConnectReasonCode.Success;
            LogMessage(context, false);
        }

        /// <summary>
        ///     Gets the client id prefix for a client id if there is one or <c>null</c> else.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns>The client id prefix for a client id if there is one or <c>null</c> else.</returns>
        private async Task<string> GetClientIdPrefix(string clientId)
        {
            var clientIdPrefixes = await this.userRepository.GetAllClientIdPrefixes();
            return clientIdPrefixes.FirstOrDefault(clientId.StartsWith);
        }
    }
}
