using System.Collections.Generic;
using POEPROG7312Part1.Models;
using MongoDB.Driver;
using System.Linq;

namespace POEPROG7312Part1.Services
{
    public class ServiceRequestService
    {
        private readonly IMongoCollection<ServiceRequest> _requests;
        private readonly IMongoCollection<Issue> _issues;

        public ServiceRequestService()
        {
            var client = new MongoClient("mongodb+srv://st10385543:Kayden2903@cluster0.ec5clhx.mongodb.net/?retryWrites=true&w=majority");
            var database = client.GetDatabase("POE_DB");
            _requests = database.GetCollection<ServiceRequest>("ServiceRequests");
            _issues = database.GetCollection<Issue>("Issues");
        }

        public List<ServiceRequest> GetAll()
        {
            return _requests.Find(_ => true).ToList();
        }

        public ServiceRequest GetByRequestID(int requestId)
        {
            return _requests.Find(r => r.RequestID == requestId).FirstOrDefault();
        }

        public ServiceRequest GetByIssueId(string issueId)
        {
            return _requests.Find(r => r.IssueId == issueId).FirstOrDefault();
        }

        public void Add(ServiceRequest request)
        {
            _requests.InsertOne(request);
        }

        public int GenerateNewRequestID()
        {
            var lastRequest = _requests.Find(_ => true)
                                       .SortByDescending(r => r.RequestID)
                                       .FirstOrDefault();
            return lastRequest != null ? lastRequest.RequestID + 1 : 101;
        }

        public void UpdateStatus(int requestId, string newStatus)
        {
            var filter = Builders<ServiceRequest>.Filter.Eq(r => r.RequestID, requestId);
            var update = Builders<ServiceRequest>.Update.Set(r => r.Status, newStatus);
            _requests.UpdateOne(filter, update);

            var request = GetByRequestID(requestId);
            if (request != null && !string.IsNullOrEmpty(request.IssueId) && newStatus == "Resolved")
            {
                var issueFilter = Builders<Issue>.Filter.Eq(i => i.Id, new MongoDB.Bson.ObjectId(request.IssueId));
                var issueUpdate = Builders<Issue>.Update.Set(i => i.Status, "Resolved");
                _issues.UpdateOne(issueFilter, issueUpdate);
            }
        }

        public void UpdateStatusByIssueId(string issueId, string newStatus)
        {
            var filter = Builders<ServiceRequest>.Filter.Eq(r => r.IssueId, issueId);
            var update = Builders<ServiceRequest>.Update.Set(r => r.Status, newStatus);
            _requests.UpdateOne(filter, update);
        }

        public void Delete(int requestId)
        {
            _requests.DeleteOne(r => r.RequestID == requestId);
        }

        public void SeedSampleRequests()
        {
            if (_requests.CountDocuments(_ => true) > 0) return;

            var sampleRequests = new List<ServiceRequest>
            {
                new ServiceRequest(101, null, "Durban", "Water", "Leaking pipe", null, "Acknowledged", 1),
                new ServiceRequest(102, null, "Durban", "Electricity", "Power outage", null, "In Progress", 2),
            };

            _requests.InsertMany(sampleRequests);
        }

        public void CreateFromIssue(Issue issue)
        {
            if (issue == null || issue.Status != "Request Sent") return;

            var existing = GetByIssueId(issue.Id.ToString());
            if (existing != null) return;

            var newRequestId = GenerateNewRequestID();
            var serviceRequest = new ServiceRequest(
                newRequestId,
                issue.Id.ToString(),
                issue.Location,
                issue.Category,
                issue.Description,
                issue.AttachmentPath,
                "Acknowledged",
                1
            );

            Add(serviceRequest);


            var filter = Builders<Issue>.Filter.Eq(i => i.Id, issue.Id);
            var update = Builders<Issue>.Update.Set(i => i.Status, "Request Sent");
            _issues.UpdateOne(filter, update);
        }
    }
}