using global::POEPROG7312Part1.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace POEPROG7312Part1.Services
{
    public class IssueService
    {
        private readonly IMongoCollection<Issue> _issues;

        public IssueService()
        {
            // Fixed connection string
            var client = new MongoClient("mongodb+srv://st10385543:Kayden2903@cluster0.ec5clhx.mongodb.net/?retryWrites=true&w=majority");
            var database = client.GetDatabase("POE_DB");
            _issues = database.GetCollection<Issue>("Issues");
        }

        public void UpdateStatus(string issueId, string newStatus)
        {
            var filter = Builders<Issue>.Filter.Eq(i => i.Id, ObjectId.Parse(issueId));
            var update = Builders<Issue>.Update.Set(i => i.Status, newStatus);
            _issues.UpdateOne(filter, update);
        }
    }
}
