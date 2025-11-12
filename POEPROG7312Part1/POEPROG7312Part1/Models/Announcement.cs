
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System;

    namespace POEPROG7312Part1.Models
    {
        public class Announcement
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string? Id { get; set; }

            public string Title { get; set; }
            public string Message { get; set; }
            public DateTime CreatedAt { get; set; }

            public string? ImagePath { get; set; }
            public string? FileName { get; set; }
            public string? ImageUrl { get; set; }
        }
    }
