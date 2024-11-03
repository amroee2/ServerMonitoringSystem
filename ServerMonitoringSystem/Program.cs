﻿using Newtonsoft.Json.Linq;
using ServerMonitoringSystemDatabaseManagement.DatabaseManagement;
using ServerMonitoringSystemMessageQueueServices.MessageQueueServices;
using ServerMonitoringSystemServerStatisticsManagement.ServerStatisticsManagement;
using ServerMonitoringSystemSignalRManagement.SignalRManagement;


await StartServerMonitoringAsync();

async Task StartServerMonitoringAsync()
{
    string url = GetHubUrl("appsettings.json");
    var server = new SignalRServer(url);
    await server.StartAsync();

    var client = new SignalRClient($"{url}/chatHub");
    await client.ConnectAsync();

    var config = LoadServerStatisticsConfig("appsettings.json");
    var anamolyThresholdConfig = LoadAnamolyThresholdConfig("appsettings.json");

    var mongoDBRepository = new MongoDBRepository();
    DatabaseRepository databaseRepository = new DatabaseRepository(mongoDBRepository);
    var serverStatisticsRepository = new ServerStatisticsRepository();
    RabbitMQService rabbitMQService = new RabbitMQService();
    var serverStatisticsPublisher = new ServerStatisticsPublisher(rabbitMQService);
    var serverStatisticsConsumer = new ServerStatisticsConsumer(rabbitMQService);
    var anamolyDetectionRepository = new AnamolyDetectionRepository(anamolyThresholdConfig, client);

    await RunStatisticsMonitoringLoopAsync(config, serverStatisticsRepository, serverStatisticsPublisher, databaseRepository, anamolyDetectionRepository, serverStatisticsConsumer);
}

async Task RunStatisticsMonitoringLoopAsync(ServerStatisticsConfig config, ServerStatisticsRepository serverStatisticsRepository, ServerStatisticsPublisher serverStatisticsPublisher,
    DatabaseRepository databaseRepository, AnamolyDetectionRepository anamolyDetectionRepository, ServerStatisticsConsumer serverStatisticsConsumer)
{

    while (true)
    {
        var statistics = serverStatisticsRepository.UpdateStatistics();
        statistics.ServerIdentifier = config.ServerIdentifier;

        serverStatisticsPublisher.PublishServerStatistics(statistics);

        if (await databaseRepository.IsEmpty())
        {
            await InitialStatisticsSetupAsync(statistics, config, databaseRepository);
            continue;
        }

        await PerformStatisticsAndAnomalyDetectionAsync(statistics, serverStatisticsPublisher, databaseRepository, anamolyDetectionRepository, serverStatisticsConsumer);
        Thread.Sleep(config.SamplingIntervalSeconds * 1000);
    }
}

async Task InitialStatisticsSetupAsync(ServerStatistics statistics, ServerStatisticsConfig config, DatabaseRepository databaseRepository)
{
    Thread.Sleep(config.SamplingIntervalSeconds * 1000);
    await databaseRepository.InsertDocumentAsync(statistics);
}

async Task PerformStatisticsAndAnomalyDetectionAsync(ServerStatistics statistics, ServerStatisticsPublisher serverStatisticsPublisher,
    DatabaseRepository databaseRepository, AnamolyDetectionRepository anamolyDetectionRepository, ServerStatisticsConsumer serverStatisticsConsumer)
{
    var previousStatistics = await databaseRepository.GetLatestDocumentAsync();
    var latestStatistics = serverStatisticsConsumer.GetMessage();

    anamolyDetectionRepository.DetectAnamoly(statistics, previousStatistics);
    anamolyDetectionRepository.DetectHighUsage(statistics, previousStatistics);

    await databaseRepository.InsertDocumentAsync(statistics);
}

ServerStatisticsConfig LoadServerStatisticsConfig(string filePath)
{
    JObject config = JObject.Parse(File.ReadAllText(filePath));

    return new ServerStatisticsConfig
    {
        SamplingIntervalSeconds = (int)config["ServerStatisticsConfig"]["SamplingIntervalSeconds"],
        ServerIdentifier = (string)config["ServerStatisticsConfig"]["ServerIdentifier"]
    };
}

AnamolyThresholdConfig LoadAnamolyThresholdConfig(string filePath)
{
    JObject config = JObject.Parse(File.ReadAllText(filePath));

    return new AnamolyThresholdConfig
    {
        CpuUsageAnomalyThresholdPercentage = (double)config["AnomalyDetectionConfig"]["CpuUsageAnomalyThresholdPercentage"],
        MemoryUsageAnomalyThresholdPercentage = (double)config["AnomalyDetectionConfig"]["MemoryUsageAnomalyThresholdPercentage"],
        MemoryUsageThresholdPercentage = (double)config["AnomalyDetectionConfig"]["MemoryUsageThresholdPercentage"],
        CpuUsageThresholdPercentage = (double)config["AnomalyDetectionConfig"]["CpuUsageThresholdPercentage"]
    };
}

string GetHubUrl(string filePath)
{
    JObject config = JObject.Parse(File.ReadAllText(filePath));
    return (string)config["SignalRConfig"]["SignalRUrl"];
}
