namespace ServerMonitoringSystem.MessageQueueServices
{
    public interface IMessageQueue
    {
        void Publish(string topic, string message);
    }
}
