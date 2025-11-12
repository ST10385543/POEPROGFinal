using Microsoft.AspNetCore.Mvc;
using POEPROG7312Part1.Models;
using POEPROG7312Part1.Services;
using POEPROG7312Part1.Datastructures;
using System.Collections.Generic;
using System.Linq;

namespace POEPROG7312Part1.Controllers
{
    public class ServiceRequestController : Controller
    {
        // Services for handling Service Requests and Issues
        private readonly ServiceRequestService _service;
        private readonly IssueService _issueService;

        // Binary Search Tree to sort service requests
        private readonly BinarySearchTree _bst;

        public ServiceRequestController()
        {
            _service = new ServiceRequestService();
            _issueService = new IssueService();
            _bst = new BinarySearchTree();
        }

        // Main index page showing all service requests
        public IActionResult Index(string statusFilter)
        {
            _service.SeedSampleRequests(); // Seed sample data if empty
            var requests = _service.GetAll() ?? new List<ServiceRequest>();

            // Filter requests by status if provided
            if (!string.IsNullOrEmpty(statusFilter))
                requests = requests.Where(r => r.Status == statusFilter).ToList();

            // Insert each request into BST for sorting
            foreach (var req in requests)
                _bst.Insert(req.RequestID.ToString(), req.Status);

            // Get sorted requests from BST
            var sortedRequests = _bst.GetSortedKeys()
                .Select(k => _service.GetByRequestID(int.Parse(k)))
                .Where(r => r != null)
                .ToList();

            // Pass data to View
            ViewBag.StatusFilter = statusFilter;
            ViewBag.IsAdmin = HttpContext.Session.GetString("Role") == "Admin";

            return View(sortedRequests);
        }

        // Update the status of a service request (Admin only)
        [HttpPost]
        public IActionResult UpdateStatus(int requestId, string newStatus)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return Unauthorized();

            _service.UpdateStatus(requestId, newStatus);

            // If resolved, also update linked issue
            if (newStatus == "Resolved")
            {
                var request = _service.GetByRequestID(requestId);
                if (request != null && !string.IsNullOrEmpty(request.IssueId))
                {
                    _issueService.UpdateStatus(request.IssueId, "Resolved");

                    // Update cached issue if it exists
                    if (IssueController._issueCache.ContainsKey(request.IssueId))
                    {
                        IssueController._issueCache.Get(request.IssueId).Status = "Resolved";
                    }
                }
            }

            return RedirectToAction("Index");
        }

        // Delete a service request (Admin only)
        [HttpPost]
        public IActionResult Delete(int requestId)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return Unauthorized();

            _service.Delete(requestId);
            return RedirectToAction("Index");
        }

        // Display graph view of service requests grouped by category
        public IActionResult GraphView()
        {
            var requests = _service.GetAll() ?? new List<ServiceRequest>();

            var graph = new Graph();
            var addedEdges = new HashSet<string>();

            foreach (var req in requests)
            {
                string reqIdStr = req.RequestID.ToString();
                graph.AddNode(reqIdStr);

                // Connect requests with same category
                foreach (var other in requests)
                {
                    string otherIdStr = other.RequestID.ToString();
                    if (req.RequestID != other.RequestID && req.Category == other.Category)
                    {
                        string edgeKey = reqIdStr.CompareTo(otherIdStr) < 0
                            ? $"{reqIdStr}-{otherIdStr}"
                            : $"{otherIdStr}-{reqIdStr}";

                        if (!addedEdges.Contains(edgeKey))
                        {
                            graph.AddEdge(reqIdStr, otherIdStr);
                            addedEdges.Add(edgeKey);
                        }
                    }
                }
            }

            // Count requests per category for display
            var categoryCounts = requests
                .GroupBy(r => r.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.CategoryCounts = categoryCounts;
            ViewBag.Requests = requests;
            ViewBag.IsAdmin = HttpContext.Session.GetString("Role") == "Admin";

            return View(graph);
        }

        // Show details of a single service request
        public IActionResult Details(int requestId)
        {
            var request = _service.GetByRequestID(requestId);
            if (request == null)
                return NotFound();

            ViewBag.IsAdmin = HttpContext.Session.GetString("Role") == "Admin";
            return View(request);
        }
    }
}
// used these links to link classes
// Gómez-Martínez, M., Cervantes-Ojeda, J. & García-Nájera, A., 2021. Association and Aggregation Class Relationships: is there a Difference in Terms of Implementation? In: 9th International Conference on Software Engineering Research & Innovation (CONISOFT). pp. 10-18. DOI: 10.1109/CONISOFT52520.2021.00018.
//Al - Fedaghi, S., 2022.Conceptual Modeling of Aggregation: an Exploration. arXiv preprint arXiv:2208.11171.Available at: https://arxiv.org/abs/2208.11171 (Accessed: 12 Nov 2025).