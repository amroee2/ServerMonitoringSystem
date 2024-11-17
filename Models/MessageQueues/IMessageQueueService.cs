using Models.StatisticsCollectors;

namespace Models.MessageQueues
{
    public interface IMessageQueueService
    {
        void Publish(ServerStatistics serverStatistics);
        ServerStatistics GetMessage();
    }
}
