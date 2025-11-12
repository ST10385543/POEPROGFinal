using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using POEPROG7312Part1.Models;

namespace POEPROG7312Part1.Services
{
   
        public class MongoDbContext
        {
            private readonly IMongoDatabase _database;
            public GridFSBucket GridFS { get; }

            public MongoDbContext(string connectionString)
            {
                var client = new MongoClient(connectionString);
                _database = client.GetDatabase("POE_DB"); 
                GridFS = new GridFSBucket(_database);
            }

          
            public IMongoCollection<User> User => _database.GetCollection<User>("Users");
            public IMongoCollection<Issue> Issues => _database.GetCollection<Issue>("Issues");

        public IMongoCollection<WorkshopIdea> WorkshopIdeas => _database.GetCollection<WorkshopIdea>("WorkshopIdeas");

        public IMongoCollection<Event> Events => _database.GetCollection<Event>("Events");

        public IMongoCollection<UserSearchHistory> UserSearchHistories => _database.GetCollection<UserSearchHistory>("UserSearchHistories");

        public IMongoCollection<ServiceRequest> ServiceRequests => _database.GetCollection<ServiceRequest>("ServiceRequests");


    }
}

