using MongoDB.Driver.Core.Servers;
using ServerMonitoringSystem.ServerStatisticsManagement;

namespace ServerMonitoringSystem.MessageQueueServices
{
    public interface IMessageQueueService
    {
        void Publish(ServerStatistics serverStatistics);
        void GetMessage();
    }
}
