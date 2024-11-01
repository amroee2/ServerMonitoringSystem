using ServerMonitoringSystem.ServerStatisticsManagement;

namespace ServerMonitoringSystem.MessageQueueServices
{
    public interface IMessageQueueService
    {
        void Publish(ServerStatistics serverStatistics);
        ServerStatistics GetMessage();
    }
}
