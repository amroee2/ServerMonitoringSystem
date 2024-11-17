using Models.StatisticsCollectors;

namespace Models.DatabaseManagement
{
    public interface IDatabaseRepository
    {
        public Task InsertDocumentAsync(ServerStatistics document);
        public Task<ServerStatistics> GetLatestDocumentAsync();
        public Task<bool> IsEmpty();
    }
}
