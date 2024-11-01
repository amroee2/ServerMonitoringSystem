using MongoDB.Driver;
using ServerMonitoringSystem.ServerStatisticsManagement;

namespace ServerMonitoringSystem
{
    public class MongoDBRepository : IDatabaseRepository
    {
        private readonly IMongoDatabase _database;

        public MongoDBRepository()
        {
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("ServerMonitoringSystem");
        }

        public async Task InsertDocumentAsync(ServerStatistics document)
        {
            var collection = _database.GetCollection<ServerStatistics>("Messages");
            await collection.InsertOneAsync(document);
        }
    }
}
