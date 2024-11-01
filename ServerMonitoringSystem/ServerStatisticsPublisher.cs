namespace ServerMonitoringSystem
{
    public class ServerStatisticsPublisher
    {
        private IMessageQueue MessageQueue { get; set; }

        public ServerStatisticsPublisher(IMessageQueue messageQueue)
        {
            MessageQueue = messageQueue;
        }

        public void PublishServerStatistics(string topic, ServerStatistics serverStatistics)
        {
            MessageQueue.Publish(topic, serverStatistics.ToString());
        }
    }
}
