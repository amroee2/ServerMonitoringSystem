namespace ServerMonitoringSystemServerStatisticsManagement.ServerStatisticsManagement
{
    public interface IMessageQueueService
    {
        void Publish(ServerStatistics serverStatistics);
        ServerStatistics GetMessage();
    }
}
