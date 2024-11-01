using ServerMonitoringSystem.ServerStatisticsManagement;

namespace ServerMonitoringSystem.DatabaseManagement
{
    public interface IDatabaseRepository
    {
        Task InsertDocumentAsync(ServerStatistics document);
    }
}
