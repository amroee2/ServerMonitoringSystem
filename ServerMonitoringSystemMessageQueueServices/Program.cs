using Models.DatabaseManagement;
using Models.MessageQueues;
using ServerMonitoringSystemMessageQueueServices.MessageQueueServices;
using ServerMonitoringSystemServerStatisticsManagement.ServerStatisticsManagement;
using ServerMonitoringSystemSignalRManagement.SignalRManagement;


AnamolyThresholdConfig config = LoadAnamolyThresholdConfig();
var rabbitMQService = new RabbitMQService();
Console.WriteLine(Environment.GetEnvironmentVariable("SIGNALR_CLIENT_HUB_URL"));
SignalRClient signalRClient = new SignalRClient(Environment.GetEnvironmentVariable("SIGNALR_CLIENT_HUB_URL"));
var anamolyDetectionRepository = new AnamolyDetectionRepository(config, signalRClient);
var serverStatisticsConsumer = new ServerStatisticsConsumer(rabbitMQService);
signalRClient.ConnectAsync().Wait();

signalRClient.SendMessageAsync("Amro", "ServerMonitoringSystemMessageQueueServices is connected").Wait();
var databaseRepository = new DatabaseRepository(new MongoDBRepository());
while (true)
{
    var currentServerStatistics = serverStatisticsConsumer.GetMessage();
    if (currentServerStatistics == null)
    {
        Thread.Sleep(5 * 1000);
        continue;
    }
    if (databaseRepository.IsEmpty().Result)
    {
        Console.WriteLine("Database is empty");
        await databaseRepository.InsertDocumentAsync(currentServerStatistics);
        Thread.Sleep(60 * 1000);
        continue;
    }
    var previousServerStatistics = databaseRepository.GetLatestDocumentAsync().Result;
    Console.WriteLine($"Current Statistics: {currentServerStatistics}");
    Console.WriteLine($"Previous Statistics {previousServerStatistics}");
    try
    {
        anamolyDetectionRepository.DetectAnamoly(currentServerStatistics, previousServerStatistics);
        anamolyDetectionRepository.DetectHighUsage(currentServerStatistics);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

    await databaseRepository.InsertDocumentAsync(currentServerStatistics);
    Thread.Sleep(60 * 1000);
}
AnamolyThresholdConfig LoadAnamolyThresholdConfig()
{
    return new AnamolyThresholdConfig
    {
        CpuUsageAnomalyThresholdPercentage = double.Parse(Environment.GetEnvironmentVariable("CPU_USAGE_ANOMALY_THRESHOLD_PERCENTAGE")),
        MemoryUsageAnomalyThresholdPercentage = double.Parse(Environment.GetEnvironmentVariable("MEMORY_USAGE_ANOMALY_THRESHOLD_PERCENTAGE")),
        MemoryUsageThresholdPercentage = double.Parse(Environment.GetEnvironmentVariable("MEMORY_USAGE_THRESHOLD_PERCENTAGE")),
        CpuUsageThresholdPercentage = double.Parse(Environment.GetEnvironmentVariable("CPU_USAGE_THRESHOLD_PERCENTAGE"))
    };
}