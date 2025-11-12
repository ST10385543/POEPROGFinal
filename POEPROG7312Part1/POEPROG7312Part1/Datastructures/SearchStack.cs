namespace POEPROG7312Part1.Datastructures
{
    public class SearchStack
    {
        private Stack<string> recentSearches = new();
        public void Push(string searchTerm) => recentSearches.Push(searchTerm);
        public IEnumerable<string> GetAll() => recentSearches;
    }
}

//GeeksforGeeks, 2025. C# Data Structures. [online] Available at: https://www.geeksforgeeks.org/c-sharp/c-sharp-data-structures/ [Accessed 11 Oct. 2025].