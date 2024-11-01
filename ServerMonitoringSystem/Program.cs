using Newtonsoft.Json.Linq;
using ServerMonitoringSystem;
using ServerMonitoringSystem.MessageQueueServices;
using ServerMonitoringSystem.ServerStatisticsManagement;
using System.Text.Json;

ServerStatisticsRepository serverStatisticsRepository = new();
ServerStatisticsConfig config = ReadConfig("appsettings.json");
ServerStatisticsPublisher serverStatisticsPublisher = new(new RabbitMQService());
ServerStatistics statistics = new();
while (true)
{
    statistics = serverStatisticsRepository.UpdateStatistics();
    serverStatisticsRepository.ShowServerStatistics();
    serverStatisticsPublisher.PublishServerStatistics(config.ServerIdentifier, statistics);
    Thread.Sleep(config.SamplingIntervalSeconds * 1000);
}

static ServerStatisticsConfig ReadConfig(string filePath)
{
    JObject config = JObject.Parse(File.ReadAllText(filePath));
    int samplingIntervalSeconds = (int)config["ServerStatisticsConfig"]["SamplingIntervalSeconds"];
    string serverIdentifier = (string)config["ServerStatisticsConfig"]["ServerIdentifier"];

    return new ServerStatisticsConfig
    {
        SamplingIntervalSeconds = samplingIntervalSeconds,
        ServerIdentifier = serverIdentifier
    };
}