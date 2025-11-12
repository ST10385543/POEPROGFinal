using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POEPROG7312Part1.Models
{
    public class Event
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Category")]
        public string Category { get; set; }

        [BsonElement("Date")]
        public DateTime Date { get; set; }

        [BsonElement("ImageUrl")]
        public string ImageUrl { get; set; }  

        [BsonElement("FileName")]
        public string FileName { get; set; }  
        public string ImagePath { get; set; }

        public int Priority { get; set; }

        public string? Location {  get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("EventType")]
        public string EventType { get; set; }

    }
}