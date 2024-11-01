namespace ServerMonitoringSystem
{
    public interface IMessageQueue
    {
        void Publish(string topic, string message);
    }
}
