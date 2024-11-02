using Newtonsoft.Json.Linq;
using ServerMonitoringSystem;
using ServerMonitoringSystem.DatabaseManagement;
using ServerMonitoringSystem.MessageQueueServices;
using ServerMonitoringSystem.ServerStatisticsManagement;
using ServerMonitoringSystem.SignalRManagement;

await StartServerMonitoringAsync();

async Task StartServerMonitoringAsync()
{
    var server = new SignalRServer();
    await server.StartAsync();

    var client = new SignalRClient();
    await client.ConnectAsync();

    var config = LoadServerStatisticsConfig("appsettings.json");
    var anamolyThresholdConfig = LoadAnamolyThresholdConfig("appsettings.json");

    var mongoDBRepository = new MongoDBRepository();
    DatabaseRepository databaseRepository = new DatabaseRepository(mongoDBRepository);
    var serverStatisticsRepository = new ServerStatisticsRepository();
    var serverStatisticsPublisher = new ServerStatisticsPublisher(new RabbitMQService());
    var anamolyDetectionRepository = new AnamolyDetectionRepository(anamolyThresholdConfig, client);

    await RunStatisticsMonitoringLoopAsync(config, serverStatisticsRepository, serverStatisticsPublisher, databaseRepository, anamolyDetectionRepository);
}

async Task RunStatisticsMonitoringLoopAsync(ServerStatisticsConfig config, ServerStatisticsRepository serverStatisticsRepository, ServerStatisticsPublisher serverStatisticsPublisher,
    DatabaseRepository databaseRepository, AnamolyDetectionRepository anamolyDetectionRepository)
{
    int iterations = 0;

    while (true)
    {
        var statistics = serverStatisticsRepository.UpdateStatistics();
        statistics.ServerIdentifier = config.ServerIdentifier;

        serverStatisticsPublisher.PublishServerStatistics(statistics);

        if (iterations == 0)
        {
            await InitialStatisticsSetupAsync(statistics, config, databaseRepository);
            iterations++;
            continue;
        }

        iterations++;
        await PerformStatisticsAndAnomalyDetectionAsync(statistics, serverStatisticsPublisher, databaseRepository, anamolyDetectionRepository);
        Thread.Sleep(config.SamplingIntervalSeconds * 1000);
    }
}

async Task InitialStatisticsSetupAsync(ServerStatistics statistics, ServerStatisticsConfig config, DatabaseRepository databaseRepository)
{
    Thread.Sleep(config.SamplingIntervalSeconds * 1000);
    await databaseRepository.InsertDocumentAsync(statistics);
}

async Task PerformStatisticsAndAnomalyDetectionAsync(ServerStatistics statistics, ServerStatisticsPublisher serverStatisticsPublisher,
    DatabaseRepository databaseRepository, AnamolyDetectionRepository anamolyDetectionRepository)
{
    var previousStatistics = await databaseRepository.GetLatestDocumentAsync();
    var latestStatistics = serverStatisticsPublisher.GetMessage();

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
