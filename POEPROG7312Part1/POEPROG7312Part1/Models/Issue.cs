using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace POEPROG7312Part1.Models
{
    public class Issue
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("location")]
        public string Location { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("attachmentPath")]
        public string AttachmentPath { get; set; }

        public string Status { get; set; } = "New";

        public int? RequestID { get; set; }
    }
}