using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using POEPROG7312Part1.Models;
using POEPROG7312Part1.Services;
using POEPROG7312Part1.Datastructures;

namespace POEPROG7312Part1.Controllers
{
    public class WorkshopController : Controller
    {
        private readonly MongoDbContext _context;

        // Optional in-memory cache for WorkshopIdeas
        private static CustomDictionary<string, WorkshopIdea> _ideaCache = new CustomDictionary<string, WorkshopIdea>(50);

        public WorkshopController(MongoDbContext context)
        {
            _context = context;
        }

        // Show all ideas
        public IActionResult Index()
        {
            var ideas = _ideaCache.ToList();
            if (ideas.Count == 0)
            {
                ideas = _context.WorkshopIdeas.Find(_ => true).ToList();
                foreach (var idea in ideas)
                {
                    if (!_ideaCache.ContainsKey(idea.Id))
                        _ideaCache.Add(idea.Id, idea);
                }
            }

            return View(ideas);
        }

        // Submit a new idea
        [HttpPost]
        public IActionResult SubmitIdea(string ideaText)
        {
            if (string.IsNullOrWhiteSpace(ideaText))
            {
                TempData["Error"] = "Idea cannot be empty!";
                return RedirectToAction("Index");
            }

            var idea = new WorkshopIdea
            {
                UserName = HttpContext.Session.GetString("Username") ?? "Anonymous",
                IdeaText = ideaText
            };

            _context.WorkshopIdeas.InsertOne(idea);

         
            if (!_ideaCache.ContainsKey(idea.Id))
                _ideaCache.Add(idea.Id, idea);

            TempData["Message"] = "Thank you for your idea!";
            return RedirectToAction("Index");
        }

        
        [HttpPost]
        public IActionResult Vote(string id, int value)
        {
            var filter = Builders<WorkshopIdea>.Filter.Eq(i => i.Id, id);
            var update = Builders<WorkshopIdea>.Update.Inc(i => i.Votes, value);
            _context.WorkshopIdeas.UpdateOne(filter, update);

            // Update cache if exists
            if (_ideaCache.ContainsKey(id))
            {
                var cachedIdea = _ideaCache.Get(id);
                cachedIdea.Votes += value;
            }

            return RedirectToAction("Index");
        }

        // Admin updates status
        [HttpPost]
        public IActionResult UpdateStatus(string id, string status)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return Unauthorized();

            var filter = Builders<WorkshopIdea>.Filter.Eq(i => i.Id, id);
            var update = Builders<WorkshopIdea>.Update.Set(i => i.Status, status);
            _context.WorkshopIdeas.UpdateOne(filter, update);

            // Update cache if exists
            if (_ideaCache.ContainsKey(id))
            {
                var cachedIdea = _ideaCache.Get(id);
                cachedIdea.Status = status;
            }
            else
            {
                var idea = _context.WorkshopIdeas.Find(i => i.Id == id).FirstOrDefault();
                if (idea != null)
                    _ideaCache.Add(idea.Id, idea);
            }

            TempData["Message"] = $"Status updated to {status}.";
            return RedirectToAction("Index");
        }
    }
}
