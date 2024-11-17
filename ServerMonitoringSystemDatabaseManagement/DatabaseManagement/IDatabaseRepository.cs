
using Models;

namespace ServerMonitoringSystemDatabaseManagement.DatabaseManagement
{
    public interface IDatabaseRepository
    {
        public Task InsertDocumentAsync(ServerStatistics document);
        public Task<ServerStatistics> GetLatestDocumentAsync();
        public Task<bool> IsEmpty();
    }
}
