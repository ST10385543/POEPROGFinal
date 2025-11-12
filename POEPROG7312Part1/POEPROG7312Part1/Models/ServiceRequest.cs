using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace POEPROG7312Part1.Models
{
    [BsonIgnoreExtraElements] // <--- Ignore fields like 'username' that are not in the class
    public class ServiceRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }              // Mongo _id

        [BsonElement("RequestID")]
        public int RequestID { get; set; }          // numeric request id (your own)

        // Link to originating Issue (store Issue._id.ToString())
        [BsonElement("IssueId")]
        public string IssueId { get; set; }

        [BsonElement("Location")]
        public string Location { get; set; }

        [BsonElement("Category")]
        public string Category { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("AttachmentPath")]
        public string AttachmentPath { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; } // Acknowledged, In Progress, Resolved

        [BsonElement("Priority")]
        public int Priority { get; set; } // 1 = High, 3 = Low

        // optional created date
        [BsonElement("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ServiceRequest() { } // parameterless for deserialization

        public ServiceRequest(int requestId, string issueId, string location, string category, string description, string attachmentPath, string status, int priority)
        {
            RequestID = requestId;
            IssueId = issueId;
            Location = location;
            Category = category;
            Description = description;
            AttachmentPath = attachmentPath;
            Status = status;
            Priority = priority;
            CreatedDate = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"RequestID: {RequestID} | IssueId: {IssueId} | Category: {Category} | Status: {Status} | Priority: {Priority}";
        }
    }
}
