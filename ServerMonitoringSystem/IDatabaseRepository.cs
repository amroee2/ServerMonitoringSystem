using ServerMonitoringSystem.ServerStatisticsManagement;

namespace ServerMonitoringSystem
{
    public interface IDatabaseRepository
    {
        Task InsertDocumentAsync(ServerStatistics document);
    }
}
