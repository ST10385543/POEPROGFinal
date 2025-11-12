
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::POEPROG7312Part1.Models;
    using MongoDB.Driver;
    using POEPROG7312Part1.Models;

    namespace POEPROG7312Part1.Services
    {
        public class AnnouncementService
        {
            private readonly IMongoCollection<Announcement> _announcements;

            public AnnouncementService(string connectionString, string dbName)
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(dbName);
                _announcements = database.GetCollection<Announcement>("Announcements");
            }

            public async Task AddAnnouncementAsync(Announcement announcement)
            {
                await _announcements.InsertOneAsync(announcement);
            }

            public async Task<List<Announcement>> GetAllAnnouncementsAsync()
            {
                return await _announcements.Find(_ => true).ToListAsync();
            }
        }
    }
