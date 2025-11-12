using System;
using System.Collections;
using System.Collections.Generic;

namespace POEPROG7312Part1.Datastructures
{
    public class CategorySet : IEnumerable<string>
    {
        private List<string> _items = new();

        // Add category if it doesn't exist
        public void Add(string category)
        {
            if (!_items.Contains(category))
                _items.Add(category);
        }

        // Remove a category
        public bool Remove(string category) => _items.Remove(category);

        // Check if category exists
        public bool Contains(string category) => _items.Contains(category);

        // Get the number of categories
        public int Count => _items.Count;

        // Clear all categories
        public void Clear() => _items.Clear();

        // Enumerate
        public IEnumerator<string> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
//GeeksforGeeks, 2025. C# Data Structures. [online] Available at: https://www.geeksforgeeks.org/c-sharp/c-sharp-data-structures/ [Accessed 11 Oct. 2025].