using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace POEPROG7312Part1.Models
{
    public class WorkshopIdea
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserName { get; set; }
        public string IdeaText { get; set; }
        public int Votes { get; set; } = 0; 
        public string Status { get; set; } = "Pending"; // Pending / Accepted / Rejected
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}