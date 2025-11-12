using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using POEPROG7312Part1.Models;
using POEPROG7312Part1.Services;
using POEPROG7312Part1.ViewModels;
using POEPROG7312Part1.Datastructures;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace POEPROG7312Part1.Controllers
{
    public class EventsController : Controller
    {
        private readonly MongoDbContext _context;
        private readonly EventService _eventService;

        private static List<Event> _eventCache = new();
        private static CategorySet _categories = new();          
        private static HashSet<DateTime> _eventDates = new();     

        // Connect to MongoDB
        public EventsController(MongoDbContext context)
        {
            _context = context;
            _eventService = new EventService(
                "mongodb+srv://new_user1:5uH4zYrcRZjDUwc2@cluster0.ec5clhx.mongodb.net/?retryWrites=true&w=majority",
                "POE_DB"
            );
        }

        // Index - handles search, filtering, and recent events
        public async Task<IActionResult> Index(string searchCategory = null, string keyword = null, DateTime? searchDate = null)
        {
            var userId = HttpContext.Session.GetString("UserId");
            await ReloadEventCache();

            IEnumerable<Event> filteredEvents = _eventCache;

            if (!string.IsNullOrEmpty(searchCategory))
                filteredEvents = filteredEvents.Where(e => e.Category.Contains(searchCategory, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(keyword))
            {
                filteredEvents = filteredEvents.Where(e =>
                    (!string.IsNullOrEmpty(e.Title) && e.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(e.Description) && e.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(e.Category) && e.Category.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                );

                if (!string.IsNullOrEmpty(userId))
                {
                    await TrackUserKeywordSearchAsync(userId, keyword);

                    var matchedCategories = _eventCache
                        .Where(e => !string.IsNullOrEmpty(e.Category) && e.Category.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        .Select(e => e.Category)
                        .Distinct();

                    foreach (var cat in matchedCategories)
                        await TrackUserCategoryClickAsync(userId, cat);
                }
            }

            if (searchDate != null)
                filteredEvents = filteredEvents.Where(e => e.Date.Date == searchDate.Value.Date);

            var recentQueue = new EventPriorityQueue();
            foreach (var evt in _eventCache)
            {
                evt.Priority = -(int)(DateTime.UtcNow - evt.CreatedAt).TotalSeconds;
                recentQueue.Enqueue(evt);
            }

            var viewModel = new EventViewModel
            {
                SelectedEvent = null,
                Events = filteredEvents.ToList(),
                RecentEvents = recentQueue.GetAll().Take(3).ToList(),
                Categories = _categories,      
                EventDates = _eventDates,
                RecommendedEvents = userId != null ? await GetUserRecommendationsAsync(userId) : new List<Event>()
            };

            return View(viewModel);
        }

        // Dashboard - general event overview
        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetString("UserId");
            await ReloadEventCache();

            var recentQueue = new EventPriorityQueue();
            foreach (var evt in _eventCache)
            {
                evt.Priority = -(int)(DateTime.UtcNow - evt.CreatedAt).TotalSeconds;
                recentQueue.Enqueue(evt);
            }

            var viewModel = new EventViewModel
            {
                SelectedEvent = null,
                Events = _eventCache.OrderBy(e => e.Date).ToList(),
                RecentEvents = recentQueue.GetAll().Take(3).ToList(),
                Categories = _categories,    
                EventDates = _eventDates,
                RecommendedEvents = userId != null ? await GetUserRecommendationsAsync(userId) : new List<Event>()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ViewEvent(string eventId, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(eventId)) return RedirectToAction("Index");

            var userId = HttpContext.Session.GetString("UserId");
            var evt = _eventCache.FirstOrDefault(e => e.Id == eventId);
            if (evt == null) return NotFound();

            if (!string.IsNullOrEmpty(userId))
                await TrackUserCategoryClickAsync(userId, evt.Category);

            var recommendedEvents = userId != null ? await GetUserRecommendationsAsync(userId, evt.Id) : new List<Event>();

            var viewModel = new EventViewModel
            {
                SelectedEvent = evt,
                Events = _eventCache,
                RecentEvents = _eventCache
                    .OrderByDescending(e => e.CreatedAt)
                    .Take(3)
                    .ToList(),
                Categories = _categories,
                EventDates = _eventDates,
                RecommendedEvents = recommendedEvents,
                ReturnUrl = returnUrl ?? Url.Action("Index", "Events")
            };

            return View("EventDetails", viewModel);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Event evt, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var fileId = await _context.GridFS.UploadFromStreamAsync(imageFile.FileName, stream);

                evt.ImagePath = fileId.ToString();
                evt.FileName = imageFile.FileName;
                evt.ImageUrl = $"/Events/GetImage?fileId={fileId}";
            }

            evt.Date = evt.Date == default ? DateTime.Now : evt.Date;
            evt.Priority = CalculatePriority(evt.Date);
            evt.CreatedAt = DateTime.UtcNow;

            await _eventService.AddEventAsync(evt);
            await ReloadEventCache();

            return RedirectToAction("Index");
        }

        // Retrieve image from MongoDB GridFS
        public async Task<IActionResult> GetImage(string fileId)
        {
            if (string.IsNullOrEmpty(fileId)) return NotFound();

            using var stream = new MemoryStream();
            await _context.GridFS.DownloadToStreamAsync(new ObjectId(fileId), stream);
            stream.Position = 0;

            var fileInfo = await _context.GridFS
                .Find(Builders<GridFSFileInfo>.Filter.Eq("_id", new ObjectId(fileId)))
                .FirstOrDefaultAsync();

            return File(stream.ToArray(), "application/octet-stream", fileInfo?.Filename ?? "image");
        }

        // Refresh cache and rebuild category/date sets
        private async Task ReloadEventCache()
        {
            _eventCache = await _eventService.GetAllEventsAsync();

            _categories.Clear();
            _eventDates.Clear();
            foreach (var evt in _eventCache)
            {
                _categories.Add(evt.Category);
                _eventDates.Add(evt.Date.Date);
            }
        }

        // Calculate event priority
        private int CalculatePriority(DateTime eventDate)
        {
            int priority = (int)(eventDate - DateTime.Now).TotalDays;
            return priority < 0 ? int.MaxValue : priority;
        }

        // Track user interactions with categories
        private async Task TrackUserCategoryClickAsync(string userId, string category)
        {
            var filter = Builders<UserSearchHistory>.Filter.Eq(u => u.UserId, userId);
            var history = await _context.UserSearchHistories.Find(filter).FirstOrDefaultAsync();

            if (history == null)
            {
                var newHistory = new UserSearchHistory
                {
                    UserId = userId,
                    LastClickedCategories = new List<string> { category }
                };
                await _context.UserSearchHistories.InsertOneAsync(newHistory);
            }
            else
            {
                if (history.LastClickedCategories == null)
                    history.LastClickedCategories = new List<string>();

                history.LastClickedCategories.Remove(category);
                history.LastClickedCategories.Insert(0, category);
                if (history.LastClickedCategories.Count > 3)
                    history.LastClickedCategories = history.LastClickedCategories.Take(3).ToList();

                var update = Builders<UserSearchHistory>.Update
                    .Set(u => u.LastClickedCategories, history.LastClickedCategories);

                await _context.UserSearchHistories.UpdateOneAsync(filter, update);
            }
        }

        // Track keyword searches
        private async Task TrackUserKeywordSearchAsync(string userId, string keyword)
        {
            var filter = Builders<UserSearchHistory>.Filter.Eq(u => u.UserId, userId);
            var history = await _context.UserSearchHistories.Find(filter).FirstOrDefaultAsync();

            if (history == null)
            {
                var newHistory = new UserSearchHistory
                {
                    UserId = userId,
                    CategorySearches = new Dictionary<string, int>(),
                    DateSearches = new Dictionary<DateTime, int>()
                };
                await _context.UserSearchHistories.InsertOneAsync(newHistory);
                history = newHistory;
            }

            if (history.CategorySearches.ContainsKey(keyword))
                history.CategorySearches[keyword]++;
            else
                history.CategorySearches[keyword] = 1;

            var update = Builders<UserSearchHistory>.Update
                .Set(u => u.CategorySearches, history.CategorySearches);

            await _context.UserSearchHistories.UpdateOneAsync(filter, update);
        }

        // Generate recommended events
        private async Task<List<Event>> GetUserRecommendationsAsync(string userId, string excludeEventId = null)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Event>();

            var filter = Builders<UserSearchHistory>.Filter.Eq(u => u.UserId, userId);
            var history = await _context.UserSearchHistories.Find(filter).FirstOrDefaultAsync();

            if (history == null || history.LastClickedCategories == null || !history.LastClickedCategories.Any())
                return new List<Event>();

            var pq = new EventPriorityQueue();

            foreach (var cat in history.LastClickedCategories)
            {
                foreach (var evt in _eventCache.Where(e => e.Category == cat && e.Id != excludeEventId))
                    pq.Enqueue(evt);
            }

            return pq.GetAll().Take(3).ToList();
        }
    }
}
