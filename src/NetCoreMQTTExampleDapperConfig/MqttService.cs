// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MqttService.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main service class of the <see cref="MqttService" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleDapperConfig;

/// <inheritdoc cref="BackgroundService"/>
/// <summary>
///     The main service class of the <see cref="MqttService" />.
/// </summary>
public class MqttService : BackgroundService
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    /// The service name.
    /// </summary>
    private readonly string serviceName;

    /// <summary>
    /// The certificate.
    /// </summary>
    private readonly X509Certificate2 certificate;

    /// <summary>
    /// The user repository.
    /// </summary>
    private readonly UserRepository userRepository;

    /// <summary>
    /// The bytes divider. (Used to convert from bytes to kilobytes and so on).
    /// </summary>
    private static double BytesDivider => 1048576.0;

    /// <summary>
    /// The <see cref="PasswordHasher{TUser}" />.
    /// </summary>
    private static readonly IPasswordHasher<User> Hasher = new PasswordHasher<User>();

    /// <summary>
    /// The data limit cache for throttling for monthly data.
    /// </summary>
    private static readonly MemoryCache DataLimitCacheMonth = MemoryCache.Default;

    /// <summary>
    /// The client identifiers.
    /// </summary>
    private static readonly HashSet<string> clientIds = new();

    /// <summary>
    /// Gets or sets the MQTT service configuration.
    /// </summary>
    public MqttSettings MqttServiceConfiguration { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MqttService"/> class.
    /// </summary>
    /// <param name="mqttServiceConfiguration">The MQTT service configuration.</param>
    /// <param name="serviceName">The service name.</param>
    /// <param name="certificate">The certificate.</param>
    /// <param name="userRepository">The user repository.</param>
    public MqttService(MqttSettings mqttServiceConfiguration, string serviceName, X509Certificate2 certificate, UserRepository userRepository)
    {
        this.MqttServiceConfiguration = mqttServiceConfiguration;
        this.serviceName = serviceName;
        this.certificate = certificate;
        this.userRepository = userRepository;

        // Create the logger.
        this.logger = LoggerConfig.GetLoggerConfiguration(nameof(MqttService))
            .WriteTo.Sink((ILogEventSink)Log.Logger)
            .CreateLogger();
    }

    /// <inheritdoc cref="BackgroundService"/>
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!this.MqttServiceConfiguration.IsValid())
        {
            throw new Exception("The configuration is invalid");
        }

        this.logger.Information("Starting service");
        this.StartMqttServer();
        this.logger.Information("Service started");
        await base.StartAsync(cancellationToken);
    }

    /// <inheritdoc cref="BackgroundService"/>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
    }

    /// <inheritdoc cref="BackgroundService"/>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Log some memory information.
                this.LogMemoryInformation();
                await Task.Delay(this.MqttServiceConfiguration.DelayInMilliSeconds, cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.Error("An error occurred: {Exception}", ex);
            }
        }
    }

    /// <summary>
    ///     Checks whether a user has used the maximum of its publishing limit for the month or not.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="sizeInBytes">The message size in bytes.</param>
    /// <param name="monthlyByteLimit">The monthly byte limit.</param>
    /// <returns>A value indicating whether the user will be throttled or not.</returns>
    private bool IsUserThrottled(string clientId, long sizeInBytes, long monthlyByteLimit)
    {
        var foundUserInCache = DataLimitCacheMonth.GetCacheItem(clientId);

        if (foundUserInCache == null)
        {
            DataLimitCacheMonth.Add(clientId, sizeInBytes, DateTimeOffset.Now.EndOfCurrentMonth());

            if (sizeInBytes < monthlyByteLimit)
            {
                return false;
            }

            this.logger.Information("The client with client id {@ClientId} is now locked until the end of this month because it already used its data limit.", clientId);
            return true;
        }

        try
        {
            var currentValue = Convert.ToInt64(foundUserInCache.Value);
            currentValue = checked(currentValue + sizeInBytes);
            DataLimitCacheMonth[clientId] = currentValue;

            if (currentValue >= monthlyByteLimit)
            {
                this.logger.Information("The client with client id {@ClientId} is now locked until the end of this month because it already used its data limit.", clientId);
                return true;
            }
        }
        catch (OverflowException)
        {
            this.logger.Information("OverflowException thrown.");
            this.logger.Information("The client with client id {@ClientId} is now locked until the end of this month because it already used its data limit.", clientId);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Gets the client id prefix for a client id if there is one or <c>null</c> else.
    /// </summary>
    /// <param name="clientId">The client id.</param>
    /// <returns>The client id prefix for a client id if there is one or <c>null</c> else.</returns>
    private async Task<string?> GetClientIdPrefix(string clientId)
    {
        var clientIdPrefixes = await this.userRepository!.GetAllClientIdPrefixes();
        return clientIdPrefixes.FirstOrDefault(clientId.StartsWith);
    }

    /// <summary>
    /// Validates the MQTT connection.
    /// </summary>
    /// <param name="args">The arguments.</param>
    private async Task ValidateConnectionAsync(ValidatingConnectionEventArgs args)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(args.UserName))
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                return;
            }

            if (clientIds.TryGetValue(args.ClientId, out var _))
            {
                args.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                this.logger.Warning("A client with client id {ClientId} is already connected", args.ClientId);
                return;
            }

            var currentUser = await this.userRepository!.GetUserByName(args.UserName).ConfigureAwait(false);

            if (currentUser == null)
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                this.LogMessage(args, true);
                return;
            }

            if (args.UserName != currentUser.UserName)
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                this.LogMessage(args, true);
                return;
            }

            var hashingResult = Hasher.VerifyHashedPassword(
                currentUser,
                currentUser.PasswordHash,
                args.Password);

            if (hashingResult == PasswordVerificationResult.Failed)
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                this.LogMessage(args, true);
                return;
            }

            if (!currentUser.ValidateClientId)
            {
                args.ReasonCode = MqttConnectReasonCode.Success;
                args.SessionItems.Add(args.ClientId, currentUser);
                this.LogMessage(args, false);
                return;
            }

            if (string.IsNullOrWhiteSpace(currentUser.ClientIdPrefix))
            {
                if (args.ClientId != currentUser.ClientId)
                {
                    args.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                    this.LogMessage(args, true);
                    return;
                }

                args.SessionItems.Add(currentUser.ClientId, currentUser);
            }
            else
            {
                args.SessionItems.Add(currentUser.ClientIdPrefix, currentUser);
            }

            args.ReasonCode = MqttConnectReasonCode.Success;
            this.LogMessage(args, false);
        }
        catch (Exception ex)
        {
            this.logger.Error("An error occurred: {Exception}.", ex);
        }
    }

    /// <summary>
    /// Validates the MQTT subscriptions.
    /// </summary>
    /// <param name="args">The arguments.</param>
    private async Task InterceptSubscriptionAsync(InterceptingSubscriptionEventArgs args)
    {
        try
        {
            var clientIdPrefix = await this.GetClientIdPrefix(args.ClientId);
            User? currentUser = null;

            if (string.IsNullOrWhiteSpace(clientIdPrefix))
            {
                if (args.SessionItems.Contains(args.ClientId))
                {
                    currentUser = args.SessionItems[args.ClientId] as User;
                }
            }
            else
            {
                if (args.SessionItems.Contains(clientIdPrefix))
                {
                    currentUser = args.SessionItems[clientIdPrefix] as User;
                }
            }

            if (currentUser is null)
            {
                args.ProcessSubscription = false;
                this.LogMessage(args, false);
                return;
            }

            var topic = args.TopicFilter.Topic;

            // Get blacklist
            var publishBlackList = await this.userRepository!.GetBlacklistItemsForUser(currentUser.Id, BlacklistWhitelistType.Subscribe);
            var blacklist = publishBlackList?.ToList() ?? new List<BlacklistWhitelist>();

            // Get whitelist
            var publishWhitelist = await this.userRepository!.GetWhitelistItemsForUser(currentUser.Id, BlacklistWhitelistType.Subscribe);
            var whitelist = publishWhitelist?.ToList() ?? new List<BlacklistWhitelist>();

            // Check matches
            if (blacklist.Any(b => b.Value == topic))
            {
                args.ProcessSubscription = false;
                this.LogMessage(args, false);
                return;
            }

            if (whitelist.Any(b => b.Value == topic))
            {
                args.ProcessSubscription = true;
                this.LogMessage(args, true);
                return;
            }

            foreach (var forbiddenTopic in blacklist)
            {
                var doesTopicMatch = TopicChecker.Regex(forbiddenTopic.Value, topic);

                if (!doesTopicMatch)
                {
                    continue;
                }

                args.ProcessSubscription = false;
                this.LogMessage(args, false);
                return;
            }

            foreach (var allowedTopic in whitelist)
            {
                var doesTopicMatch = TopicChecker.Regex(allowedTopic.Value, topic);

                if (!doesTopicMatch)
                {
                    continue;
                }

                args.ProcessSubscription = true;
                this.LogMessage(args, true);
                return;
            }

            args.ProcessSubscription = false;
            this.LogMessage(args, false);
        }
        catch (Exception ex)
        {
            this.logger.Error("An error occurred: {Exception}.", ex);
        }
    }

    /// <summary>
    /// Validates the MQTT application messages.
    /// </summary>
    /// <param name="args">The arguments.</param>
    private async Task InterceptApplicationMessagePublishAsync(InterceptingPublishEventArgs args)
    {
        try
        {
            var clientIdPrefix = await this.GetClientIdPrefix(args.ClientId);
            User? currentUser = null;

            if (string.IsNullOrWhiteSpace(clientIdPrefix))
            {
                if (args.SessionItems.Contains(args.ClientId))
                {
                    currentUser = args.SessionItems[args.ClientId] as User;
                }
            }
            else
            {
                if (args.SessionItems.Contains(clientIdPrefix))
                {
                    currentUser = args.SessionItems[clientIdPrefix] as User;
                }
            }

            if (currentUser is null)
            {
                args.ProcessPublish = false;
                return;
            }

            var topic = args.ApplicationMessage.Topic;

            if (currentUser.ThrottleUser)
            {
                var payload = args.ApplicationMessage?.Payload;

                if (payload != null)
                {
                    if (currentUser.MonthlyByteLimit != null)
                    {
                        if (this.IsUserThrottled(args.ClientId, payload.Length, currentUser.MonthlyByteLimit.Value))
                        {
                            args.ProcessPublish = false;
                            return;
                        }
                    }
                }
            }

            // Get blacklist
            var publishBlackList = await this.userRepository!.GetBlacklistItemsForUser(currentUser.Id, BlacklistWhitelistType.Publish);
            var blacklist = publishBlackList?.ToList() ?? new List<BlacklistWhitelist>();

            // Get whitelist
            var publishWhitelist = await this.userRepository!.GetWhitelistItemsForUser(currentUser.Id, BlacklistWhitelistType.Publish);
            var whitelist = publishWhitelist?.ToList() ?? new List<BlacklistWhitelist>();

            // Check matches
            if (blacklist.Any(b => b.Value == topic))
            {
                args.ProcessPublish = false;
                return;
            }

            if (whitelist.Any(b => b.Value == topic))
            {
                args.ProcessPublish = true;
                this.LogMessage(args);
                return;
            }

            foreach (var forbiddenTopic in blacklist)
            {
                var doesTopicMatch = TopicChecker.Regex(forbiddenTopic.Value, topic);

                if (!doesTopicMatch)
                {
                    continue;
                }

                args.ProcessPublish = false;
                return;
            }

            foreach (var allowedTopic in whitelist)
            {
                var doesTopicMatch = TopicChecker.Regex(allowedTopic.Value, topic);

                if (!doesTopicMatch)
                {
                    continue;
                }

                args.ProcessPublish = true;
                this.LogMessage(args);
                return;
            }

            args.ProcessPublish = false;
        }
        catch (Exception ex)
        {
            this.logger.Error("An error occurred: {Exception}.", ex);
        }
    }

    /// <summary>
    /// Handles the client connected event.
    /// </summary>
    /// <param name="args">The arguments.</param>
    private async Task ClientDisconnectedAsync(ClientDisconnectedEventArgs args)
    {
        clientIds.Remove(args.ClientId);
        await Task.Delay(1);
    }

    /// <summary>
    /// Starts the MQTT server.
    /// </summary>
    private void StartMqttServer()
    {
        var optionsBuilder = new MqttServerOptionsBuilder()
#if DEBUG
            .WithDefaultEndpoint().WithDefaultEndpointPort(1883)
#else
            .WithoutDefaultEndpoint()
#endif
            .WithEncryptedEndpoint().WithEncryptedEndpointPort(this.MqttServiceConfiguration.Port)
            .WithEncryptionCertificate(this.certificate.Export(X509ContentType.Pfx))
            .WithEncryptionSslProtocol(SslProtocols.Tls12);

        var mqttServer = new MqttFactory().CreateMqttServer(optionsBuilder.Build());
        mqttServer.ValidatingConnectionAsync += this.ValidateConnectionAsync;
        mqttServer.InterceptingSubscriptionAsync += this.InterceptSubscriptionAsync;
        mqttServer.InterceptingPublishAsync += this.InterceptApplicationMessagePublishAsync;
        mqttServer.ClientDisconnectedAsync += this.ClientDisconnectedAsync;
        mqttServer.StartAsync();
    }

    /// <summary> 
    ///     Logs the message from the MQTT subscription interceptor context. 
    /// </summary> 
    /// <param name="args">The arguments.</param>
    /// <param name="successful">A <see cref="bool"/> value indicating whether the subscription was successful or not.</param>
    private void LogMessage(InterceptingSubscriptionEventArgs args, bool successful)
    {
#pragma warning disable Serilog004 // Constant MessageTemplate verifier
        this.logger.Information(
            successful
                ? "New subscription: ClientId = {ClientId}, TopicFilter = {TopicFilter}"
                : "Subscription failed for clientId = {ClientId}, TopicFilter = {TopicFilter}",
            args.ClientId,
            args.TopicFilter);
#pragma warning restore Serilog004 // Constant MessageTemplate verifier
    }

    /// <summary>
    ///     Logs the message from the MQTT message interceptor context.
    /// </summary>
    /// <param name="args">The arguments.</param>
    private void LogMessage(InterceptingPublishEventArgs args)
    {
        var payload = args.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(args.ApplicationMessage.Payload);

        this.logger.Information(
            "Message: ClientId = {ClientId}, Topic = {Topic}, Payload = {Payload}, QoS = {Qos}, Retain-Flag = {RetainFlag}",
            args.ClientId,
            args.ApplicationMessage?.Topic,
            payload,
            args.ApplicationMessage?.QualityOfServiceLevel,
            args.ApplicationMessage?.Retain);
    }

    /// <summary> 
    ///     Logs the message from the MQTT connection validation context. 
    /// </summary> 
    /// <param name="args">The arguments.</param>
    /// <param name="showPassword">A <see cref="bool"/> value indicating whether the password is written to the log or not.</param>
    private void LogMessage(ValidatingConnectionEventArgs args, bool showPassword)
    {
        if (showPassword)
        {
            this.logger.Information(
                "New connection: ClientId = {ClientId}, Endpoint = {Endpoint}, Username = {UserName}, Password = {Password}, CleanSession = {CleanSession}",
                args.ClientId,
                args.Endpoint,
                args.UserName,
                args.Password,
                args.CleanSession);
        }
        else
        {
            this.logger.Information(
                "New connection: ClientId = {ClientId}, Endpoint = {Endpoint}, Username = {UserName}, CleanSession = {CleanSession}",
                args.ClientId,
                args.Endpoint,
                args.UserName,
                args.CleanSession);
        }
    }

    /// <summary>
    /// Logs the heartbeat message with some memory information.
    /// </summary>
    private void LogMemoryInformation()
    {
        var totalMemory = GC.GetTotalMemory(false);
        var memoryInfo = GC.GetGCMemoryInfo();
        var divider = BytesDivider;
        Log.Information(
            "Heartbeat for service {ServiceName}: Total {Total}, heap size: {HeapSize}, memory load: {MemoryLoad}.",
            this.serviceName, $"{(totalMemory / divider):N3}", $"{(memoryInfo.HeapSizeBytes / divider):N3}", $"{(memoryInfo.MemoryLoadBytes / divider):N3}");
    }
}
