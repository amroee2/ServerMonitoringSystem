namespace ServerMonitoringSystem.MessageQueueServices
{
    public interface IMessageQueueService
    {
        void Publish(string topic, string message);
    }
}
