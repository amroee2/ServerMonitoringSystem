
using ServerMonitoringSystemServerStatisticsManagement.ServerStatisticsManagement;

namespace ServerMonitoringSystemDatabaseManagement.DatabaseManagement
{
    public class DatabaseRepository
    {

        private readonly IDatabaseRepository _databaseRepository;
        public DatabaseRepository(IDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        public async Task InsertDocumentAsync(ServerStatistics document)
        {
            await _databaseRepository.InsertDocumentAsync(document);
        }

        public async Task<ServerStatistics> GetLatestDocumentAsync()
        {
            return await _databaseRepository.GetLatestDocumentAsync();
        }

        public async Task<bool> IsEmpty()
        {
            return await _databaseRepository.IsEmpty();
        }
    }
}
