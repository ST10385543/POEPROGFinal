using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using POEPROG7312Part1.Models;

namespace POEPROG7312Part1.Controllers
{
    public class AnnouncementsController : Controller
    {
        // MongoDB collection for storing events (including announcements)
        private readonly IMongoCollection<Event> _eventsCollection;

        // Provides access to the web root for file uploads
        private readonly IWebHostEnvironment _env;

        // Constructor initializes MongoDB connection and sets up events collection
        public AnnouncementsController(IWebHostEnvironment env)
        {
            _env = env;

            // Connect to local MongoDB instance and get database/collection
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("MunicipalCitizenDB");
            _eventsCollection = database.GetCollection<Event>("Events");
        }

        // GET: /Announcements
        // Fetches all announcements sorted by creation date (descending)
        public async Task<IActionResult> Index()
        {
            var announcements = await _eventsCollection
                .Find(e => e.Category == "Announcement") // Filter only announcements
                .SortByDescending(e => e.CreatedAt)      // Show newest first
                .ToListAsync();

            return View(announcements); // Pass to the view
        }

        // GET: /Announcements/Create
        // Displays the form to create a new announcement
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Announcements/Create
        // Handles the form submission for creating an announcement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event model, IFormFile? imageFile)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Description))
                {
                    ModelState.AddModelError("", "Title and Description are required.");
                    return View(model);
                }

                // Handle image upload if provided
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                    // Generate unique filename to prevent conflicts
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file to disk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Store relative path in model for display in views
                    model.ImagePath = "/uploads/" + uniqueFileName;
                }

                // Set category and timestamp
                model.Category = "Announcement";
                model.CreatedAt = DateTime.UtcNow;

                // Insert into MongoDB
                await _eventsCollection.InsertOneAsync(model);

                TempData["Message"] = " Announcement created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Handle errors during creation
                ModelState.AddModelError("", "An error occurred while creating the announcement: " + ex.Message);
                return View(model);
            }
        }

        // GET: /Announcements/Details/{id}
        // Shows details of a single announcement
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var announcement = await _eventsCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
            if (announcement == null)
                return NotFound();

            return View(announcement);
        }

        // POST: /Announcements/Delete/{id}
        // Deletes an announcement by its ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            await _eventsCollection.DeleteOneAsync(e => e.Id == id); // Remove from MongoDB
            TempData["Message"] = " Announcement deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
