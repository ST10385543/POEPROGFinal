using MongoDB.Driver;
using POEPROG7312Part1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POEPROG7312Part1.Services
{
    public class EventService
    {
        private readonly IMongoCollection<Event> _events;

        public EventService(string connectionString, string dbName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            _events = database.GetCollection<Event>("Events");
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            var events = await _events.Find(_ => true).ToListAsync();

            foreach (var e in events)
            {
                if (e.Priority == 0)
                {
                    e.Priority = (int)(e.Date - DateTime.Now).TotalDays;
                    if (e.Priority < 0) e.Priority = int.MaxValue;
                }

                if (e.CreatedAt == default)
                    e.CreatedAt = DateTime.UtcNow;
            }

            return events;
        }

        public async Task<List<Event>> GetRecentEventsAsync(int count = 3)
        {
            return await _events.Find(_ => true)
                                .SortByDescending(e => e.CreatedAt)
                                .Limit(count)
                                .ToListAsync();
        }

        public async Task<List<Event>> SearchEventsAsync(string category, DateTime? date)
        {
            var filter = Builders<Event>.Filter.Empty;

            if (!string.IsNullOrEmpty(category))
                filter &= Builders<Event>.Filter.Where(e => e.Category.ToLower().Contains(category.ToLower()));

            if (date.HasValue)
                filter &= Builders<Event>.Filter.Eq(e => e.Date.Date, date.Value.Date);

            var events = await _events.Find(filter).ToListAsync();

            foreach (var e in events)
            {
                if (e.Priority == 0)
                {
                    e.Priority = (int)(e.Date - DateTime.Now).TotalDays;
                    if (e.Priority < 0) e.Priority = int.MaxValue;
                }
            }

            return events;
        }

        public async Task AddEventAsync(Event evt)
        {
            if (evt.CreatedAt == default)
                evt.CreatedAt = DateTime.UtcNow;

            if (evt.Priority == 0)
            {
                evt.Priority = (int)(evt.Date - DateTime.Now).TotalDays;
                if (evt.Priority < 0) evt.Priority = int.MaxValue;
            }

            await _events.InsertOneAsync(evt);
        }

        public async Task UpdateEventAsync(string id, Event evt)
        {
            if (evt.Priority == 0)
            {
                evt.Priority = (int)(evt.Date - DateTime.Now).TotalDays;
                if (evt.Priority < 0) evt.Priority = int.MaxValue;
            }

            await _events.ReplaceOneAsync(e => e.Id == id, evt);
        }

        public async Task DeleteEventAsync(string id) =>
            await _events.DeleteOneAsync(e => e.Id == id);

        public async Task<Event> GetEventByIdAsync(string id) =>
            await _events.Find(e => e.Id == id).FirstOrDefaultAsync();
    }
}
