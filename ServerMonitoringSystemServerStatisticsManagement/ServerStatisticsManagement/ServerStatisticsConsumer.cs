namespace ServerMonitoringSystemServerStatisticsManagement.ServerStatisticsManagement
{
    public class ServerStatisticsConsumer
    {

        private IMessageQueueService _messageQueueService { get; set; }

        public ServerStatisticsConsumer(IMessageQueueService messageQueue)
        {
            _messageQueueService = messageQueue;
        }
        public ServerStatistics GetMessage()
        {
            return _messageQueueService.GetMessage();
        }
    }
}
