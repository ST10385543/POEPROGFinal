using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using POEPROG7312Part1.Models;
using POEPROG7312Part1.Services;

namespace POEPROG7312Part1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMongoCollection<User> _users; // MongoDB collection for users
        private readonly UserCacheService _userCache;  // In-memory cache for users

        public AccountController(MongoDbContext context, UserCacheService userCache)
        {
            _users = context.User; // Initialize user collection
            _userCache = userCache; // Initialize cache service
        }

        public IActionResult Register() => View(); // Show registration form

        [HttpPost]
        public IActionResult Register(User user)
        {
            // Validate required fields
            if (user == null || string.IsNullOrWhiteSpace(user.Username)
                || string.IsNullOrWhiteSpace(user.Password)
                || string.IsNullOrWhiteSpace(user.Email))
            {
                ViewBag.Error = "Username, Email, and Password are required.";
                return View();
            }

            // Check if username or email already exists
            var existingUser = _users.Find(u => u.Username == user.Username || u.Email == user.Email).FirstOrDefault();
            if (existingUser != null)
            {
                ViewBag.Error = "Username or Email already taken.";
                return View();
            }

            user.Role = "User"; // Set default role

            _users.InsertOne(user); // Save user to database

            if (!_userCache.ContainsUser(user.Username))
                _userCache.AddUser(user); // Add user to cache

            TempData["Message"] = "Registration successful! Please log in.";
            return RedirectToAction("Login"); // Redirect to login
        }

        public IActionResult Login() => View(); // Show login form

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter username and password.";
                return View();
            }

            // Check cache first
            var user = _userCache.GetUser(username);

            // If not in cache, check database
            if (user == null)
            {
                user = _users.Find(u => u.Username == username && u.Password == password).FirstOrDefault();
                if (user != null && !_userCache.ContainsUser(user.Username))
                    _userCache.AddUser(user); // Add to cache
            }

            // Verify password
            if (user != null && user.Password == password)
            {
                HttpContext.Session.SetString("User", user.Username); // Store username in session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Role", user.Role);     // Store role in session
                return RedirectToAction("Index", "Home"); // Redirect to home
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear session
            return RedirectToAction("Login"); // Redirect to login
        }

        public IActionResult ForgotPassword() => View(); // Show forgot password form

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Message = "Please enter an email.";
                return View();
            }

            // Find user by email
            var user = _users.Find(u => u.Email == email).FirstOrDefault();
            if (user == null)
            {
                ViewBag.Message = "No account found with that email.";
                return View();
            }

            if (!_userCache.ContainsUser(user.Username))
                _userCache.AddUser(user); // Add to cache if not exists

            return RedirectToAction("ResetPassword", new { email = email }); // Redirect to reset password
        }

        public IActionResult ResetPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Message"] = "Invalid password reset request.";
                return RedirectToAction("ForgotPassword");
            }

            ViewBag.Email = email; // Pass email to view
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string email, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Message"] = "Invalid password reset request.";
                return RedirectToAction("ForgotPassword");
            }

            // Validate new password fields
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewBag.Error = "Please enter and confirm your new password.";
                ViewBag.Email = email;
                return View();
            }

            // Check if passwords match
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                ViewBag.Email = email;
                return View();
            }

            // Update password in database
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var update = Builders<User>.Update.Set(u => u.Password, newPassword);
            var result = _users.UpdateOne(filter, update);

            if (result.ModifiedCount > 0)
            {
                // Update password in cache
                var user = _users.Find(u => u.Email == email).FirstOrDefault();
                if (user != null)
                {
                    var cachedUser = _userCache.GetUser(user.Username);
                    if (cachedUser != null)
                        cachedUser.Password = newPassword;
                }

                TempData["Message"] = "Password reset successfully. Please log in.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Failed to reset password. Please try again.";
            ViewBag.Email = email;
            return View();
        }
    }
}
