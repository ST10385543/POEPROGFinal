using POEPROG7312Part1.Models;
using System.Collections.Generic;

namespace POEPROG7312Part1.Datastructures
{
    public class EventDictionary
    {
        // Dictionary storing events by category
        public Dictionary<string, List<Event>> EventsByCategory { get; private set; } = new();

        public void AddEvent(Event evt)
        {
            if (!EventsByCategory.ContainsKey(evt.Category))
                EventsByCategory[evt.Category] = new List<Event>();

            EventsByCategory[evt.Category].Add(evt);
        }
    }
}
//GeeksforGeeks, 2025. Dictionary in C#. [online] Available at: https://www.geeksforgeeks.org/c-sharp/dictionary-in-c-sharp [Accessed 11 Oct. 2025].