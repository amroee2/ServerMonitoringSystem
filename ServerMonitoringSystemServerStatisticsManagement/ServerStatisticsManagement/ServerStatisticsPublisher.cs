using Models.MessageQueues;
using Models.StatisticsCollectors;

namespace ServerMonitoringSystemServerStatisticsManagement.ServerStatisticsManagement
{
    public class ServerStatisticsPublisher
    {
        private IMessageQueueService _messageQueueService { get; set; }

        public ServerStatisticsPublisher(IMessageQueueService messageQueue)
        {
            _messageQueueService = messageQueue;
        }

        public void PublishServerStatistics(ServerStatistics serverStatistics)
        {
            _messageQueueService.Publish(serverStatistics);
        }
    }
}
