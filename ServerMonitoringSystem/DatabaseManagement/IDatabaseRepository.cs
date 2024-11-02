using ServerMonitoringSystem.ServerStatisticsManagement;

namespace ServerMonitoringSystem.DatabaseManagement
{
    public interface IDatabaseRepository
    {
        public Task InsertDocumentAsync(ServerStatistics document);
        public Task<ServerStatistics> GetLatestDocumentAsync();
    }
}
