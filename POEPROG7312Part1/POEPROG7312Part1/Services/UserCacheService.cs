using POEPROG7312Part1.Datastructures;
using POEPROG7312Part1.Models;

namespace POEPROG7312Part1.Services
{
    public class UserCacheService
    {
        private readonly CustomDictionary<string, User> _userCache;

        public UserCacheService()
        {
      
            _userCache = new CustomDictionary<string, User>(50);
        }

        public void AddUser(User user)
        {
            if (!_userCache.ContainsKey(user.Username))
            {
                _userCache.Add(user.Username, user);
            }
        }

        // Get a user from the cache
        public User GetUser(string username)
        {
            try
            {
                return _userCache.Get(username);
            }
            catch
            {
                // User not found in cache
                return null;
            }
        }

        // Check if a user exists in the cache
        public bool ContainsUser(string username)
        {
            return _userCache.ContainsKey(username);
        }
    }
}
