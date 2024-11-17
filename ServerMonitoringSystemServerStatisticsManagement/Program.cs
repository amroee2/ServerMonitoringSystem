using Models.MessageQueues;
using Models.StatisticsCollectors;
using ServerMonitoringSystemServerStatisticsManagement.ServerStatisticsManagement;
using System.Runtime.InteropServices;

var config = LoadServerStatisticsConfig();

var rabbitMQService = new RabbitMQService();
var serverStatisticsPublisher = new ServerStatisticsPublisher(rabbitMQService);
while (true)
{
    ServerStatistics statistics;
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        WindowsServerStatisticsRepository serverStatisticsRepository = new WindowsServerStatisticsRepository();
        statistics = serverStatisticsRepository.UpdateStatistics();
    }
    else
    {
        LinuxServerStatisticsRepository serverStatisticsRepository = new LinuxServerStatisticsRepository();
        statistics = serverStatisticsRepository.UpdateStatistics();
    }
    statistics.ServerIdentifier = config.ServerIdentifier;

    serverStatisticsPublisher.PublishServerStatistics(statistics);

    Thread.Sleep(config.SamplingIntervalSeconds * 1000);
}
static ServerStatisticsConfig LoadServerStatisticsConfig()
{
    return new ServerStatisticsConfig
    {
        SamplingIntervalSeconds = int.Parse(Environment.GetEnvironmentVariable("SAMPLING_INTERVAL_SECONDS")),
        ServerIdentifier = Environment.GetEnvironmentVariable("SERVER_IDENTIFIER")
    };
}