using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using POEPROG7312Part1.Datastructures;
using POEPROG7312Part1.Models;
using POEPROG7312Part1.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace POEPROG7312Part1.Controllers
{
    public class IssueController : Controller
    {
        private readonly MongoDbContext _context;

        public static CustomDictionary<string, Issue> _issueCache = new CustomDictionary<string, Issue>(50);

        public IssueController(MongoDbContext context)
        {
            _context = context;
        }

        public IActionResult ReportIssue() => View();


        [HttpPost]
        public async Task<IActionResult> ReportIssue(Issue issue, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var fileId = await _context.GridFS.UploadFromStreamAsync(file.FileName, stream);
                issue.AttachmentPath = fileId.ToString();
            }

            issue.Status = "New";
            _context.Issues.InsertOne(issue);

            if (!_issueCache.ContainsKey(issue.Id.ToString()))
                _issueCache.Add(issue.Id.ToString(), issue);

            TempData["Message"] = "Issue reported successfully!";

            return RedirectToAction("AllIssues");
        }


        public IActionResult ReportIssueConfirmation() => View();

        public IActionResult AllIssues()
        {
            var issues = _issueCache.ToList();

            if (issues.Count == 0)
            {
                issues = _context.Issues.Find(_ => true).ToList();
                foreach (var issue in issues)
                {
                    if (!_issueCache.ContainsKey(issue.Id.ToString()))
                        _issueCache.Add(issue.Id.ToString(), issue);
                }
            }

            return View(issues);
        }

        [HttpPost]
        public IActionResult UpdateStatus(string id, string status)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return Unauthorized();

            var filter = Builders<Issue>.Filter.Eq(i => i.Id, ObjectId.Parse(id));
            var update = Builders<Issue>.Update.Set(i => i.Status, status);
            _context.Issues.UpdateOne(filter, update);

            if (_issueCache.ContainsKey(id))
            {
                var cachedIssue = _issueCache.Get(id);
                cachedIssue.Status = status;
            }
            else
            {
                var issue = _context.Issues.Find(i => i.Id == ObjectId.Parse(id)).FirstOrDefault();
                if (issue != null)
                    _issueCache.Add(issue.Id.ToString(), issue);
            }

            var serviceRequestService = new ServiceRequestService();

            if (status == "Request Sent" && _issueCache.ContainsKey(id))
            {
                var issueObj = _issueCache.Get(id);
                var existingRequest = serviceRequestService.GetByIssueId(issueObj.Id.ToString());

                if (existingRequest == null)
                {
                    int newRequestID;
                    var rnd = new Random();
                    do
                    {
                        newRequestID = rnd.Next(1000, 9999);
                    } while (serviceRequestService.GetByRequestID(newRequestID) != null);

                    var newRequest = new ServiceRequest(
                        requestId: newRequestID,
                        issueId: issueObj.Id.ToString(),
                        location: issueObj.Location,
                        category: issueObj.Category,
                        description: issueObj.Description,
                        attachmentPath: issueObj.AttachmentPath,
                        status: "Acknowledged",
                        priority: 2
                    );

                    serviceRequestService.Add(newRequest);
                }
            }
            else if (status == "Resolved" && _issueCache.ContainsKey(id))
            {
                var issueObj = _issueCache.Get(id);
                var linkedRequest = serviceRequestService.GetByIssueId(issueObj.Id.ToString());
                if (linkedRequest != null)
                    serviceRequestService.UpdateStatus(linkedRequest.RequestID, "Resolved");
            }

            return RedirectToAction("AllIssues");
        }

        [HttpPost]
        public IActionResult DeleteIssue(string id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return Unauthorized();

            _context.Issues.DeleteOne(i => i.Id == ObjectId.Parse(id));
            _issueCache.Remove(id);

            TempData["Message"] = "Issue deleted.";
            return RedirectToAction("AllIssues");
        }

        public async Task<IActionResult> GetIssueAttachment(string fileId)
        {
            if (string.IsNullOrEmpty(fileId)) return NotFound();
            if (!ObjectId.TryParse(fileId, out ObjectId id)) return BadRequest("Invalid file ID");

            using var stream = new MemoryStream();
            await _context.GridFS.DownloadToStreamAsync(id, stream);
            stream.Position = 0;

            var fileInfo = await _context.GridFS.Find(Builders<GridFSFileInfo>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
            var fileName = fileInfo?.Filename ?? "attachment";

            return File(stream.ToArray(), "application/octet-stream", fileName);
        }
    }
}