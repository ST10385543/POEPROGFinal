using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace POEPROG7312Part1.Models
{
    public class UserSearchHistory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // MongoDB document Id
        public string UserId { get; set; }
        public Dictionary<string, int> CategorySearches { get; set; } = new();
        public Dictionary<DateTime, int> DateSearches { get; set; } = new();

        public HashSet<string> ViewedEventIds { get; set; } = new();

        [BsonElement("LastClickedCategories")]
        public List<string> LastClickedCategories { get; set; } = new List<string>();

        // Old property: kept for backward compatibility
        [BsonIgnoreIfNull]
        [BsonElement("LastClickedCategory")]
        public string LastClickedCategory { get; set; }
    }
}