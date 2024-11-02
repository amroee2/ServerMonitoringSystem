using MongoDB.Bson;
using MongoDB.Driver;
using ServerMonitoringSystem.ServerStatisticsManagement;

namespace ServerMonitoringSystem.DatabaseManagement
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

        public async Task<ServerStatistics> GetLatestDocumentAsync()
        {
            var collection = _database.GetCollection<ServerStatistics>("Messages");
            return await collection.Find(new BsonDocument())
                                   .Project<ServerStatistics>(Builders<ServerStatistics>.Projection.Exclude("_id"))
                                   .Sort(Builders<ServerStatistics>.Sort.Descending("Timestamp"))
                                   .Limit(1)
                                   .FirstOrDefaultAsync();
        }

        public async Task<bool> IsEmpty()
        {
            var collection = _database.GetCollection<ServerStatistics>("Messages");
            return await collection.CountDocumentsAsync(new BsonDocument()) == 0;
        }
    }
}
