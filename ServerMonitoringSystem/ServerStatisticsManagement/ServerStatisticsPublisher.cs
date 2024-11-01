using ServerMonitoringSystem.MessageQueueServices;

namespace ServerMonitoringSystem.ServerStatisticsManagement
{
    public class ServerStatisticsPublisher
    {
        private IMessageQueueService _messageQueueService { get; set; }

        public ServerStatisticsPublisher(IMessageQueueService messageQueue)
        {
            _messageQueueService = messageQueue;
        }

        public void PublishServerStatistics(string topic, ServerStatistics serverStatistics)
        {
            _messageQueueService.Publish(topic, serverStatistics.ToString());
        }
    }
}
