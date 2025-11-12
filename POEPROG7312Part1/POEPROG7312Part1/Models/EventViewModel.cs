using POEPROG7312Part1.Models;
using System;
using System.Collections.Generic;

namespace POEPROG7312Part1.ViewModels
{
    public class EventViewModel
    {
        public List<Event> Events { get; set; } = new();
        public List<Event> RecentEvents { get; set; } = new();
        public IEnumerable<string> Categories { get; set; }

        public HashSet<DateTime> EventDates { get; set; } = new();
        public IEnumerable<Event> RecommendedEvents { get; set; } = new List<Event>();

        public Event SelectedEvent { get; set; }
        public string ReturnUrl { get; set; }
    }
}
