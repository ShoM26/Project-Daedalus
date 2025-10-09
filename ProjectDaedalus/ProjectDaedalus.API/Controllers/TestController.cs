using Microsoft.AspNetCore.Mvc;
namespace ProjectDaedalus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // Issue: Synchronous method in async context
        [HttpGet("users/{id}")]
        public IActionResult GetUser(int id)
        {
            var user = Database.Users.FirstOrDefault(u => u.Id == id);
            
            // Issue: No null check
            return Ok(user.Name);
        }

        // Issue: Async method with blocking call
        [HttpPost("process")]
        public async Task<IActionResult> ProcessData(string data)
        {
            // Issue: Blocking in async method
            var result = SomeService.Process(data).Result;
            
            // Issue: Magic strings
            if (result == "SUCCESS")
            {
                return Ok();
            }
            
            return BadRequest("Failed");
        }

        // Issue: Not thread-safe Random usage
        [HttpGet("random")]
        public IActionResult GetRandomNumber()
        {
            Random random = new Random();
            var number = random.Next(1, 100);
            
            return Ok(number);
        }

        // Issue: SQL Injection vulnerability
        [HttpGet("search")]
        public IActionResult SearchUsers(string query)
        {
            var sql = "SELECT * FROM Users WHERE Name = '" + query + "'";
            var results = Database.ExecuteQuery(sql);
            
            return Ok(results);
        }

        // Issue: No async/await but marked async
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = Database.Users.Find(id);
            Database.Users.Remove(user);
            
            return NoContent();
        }

        // Issue: Exception swallowing
        [HttpPut("update")]
        public IActionResult UpdateUser(User user)
        {
            try
            {
                Database.Users.Update(user);
                Database.SaveChanges();
            }
            catch
            {
                // Issue: Empty catch block
            }
            
            return Ok();
        }

        // Issue: Multiple issues in one method
        [HttpGet("report")]
        public IActionResult GenerateReport(int userId)
        {
            // Issue: Unnecessary nested if
            if (userId > 0)
            {
                if (userId < 1000)
                {
                    var user = Database.Users.FirstOrDefault(u => u.Id == userId);
                    
                    // Issue: Potential NullReferenceException
                    var report = new Report
                    {
                        UserName = user.Name,
                        Email = user.Email,
                        CreatedDate = DateTime.Now
                    };
                    
                    // Issue: Magic number
                    if (user.Age > 18)
                    {
                        report.IsAdult = true;
                    }
                    
                    return Ok(report);
                }
            }
            
            return BadRequest();
        }

        // Issue: Memory leak potential - not disposing
        [HttpPost("upload")]
        public IActionResult UploadFile()
        {
            var stream = new System.IO.FileStream("temp.txt", System.IO.FileMode.Open);
            var reader = new System.IO.StreamReader(stream);
            var content = reader.ReadToEnd();
            
            // Issue: Never disposed
            return Ok(content);
        }

        // Issue: Inefficient database queries
        [HttpGet("users")]
        public IActionResult GetAllUsersWithDetails()
        {
            var users = Database.Users.ToList();
            
            // Issue: N+1 query problem
            foreach (var user in users)
            {
                user.Orders = Database.Orders.Where(o => o.UserId == user.Id).ToList();
                user.Profile = Database.Profiles.FirstOrDefault(p => p.UserId == user.Id);
            }
            
            return Ok(users);
        }

        // Issue: Hardcoded credentials
        [HttpPost("connect")]
        public IActionResult ConnectToDatabase()
        {
            var connectionString = "Server=localhost;Database=MyDb;User=admin;Password=admin123;";
            // Issue: Hardcoded password
            
            return Ok();
        }
    }

    // Mock classes for compilation
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public List<Order> Orders { get; set; }
        public Profile Profile { get; set; }
    }

    public class Order
    {
        public int UserId { get; set; }
    }

    public class Profile
    {
        public int UserId { get; set; }
    }

    public class Report
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsAdult { get; set; }
    }

    public static class Database
    {
        public static List<User> Users = new List<User>();
        public static List<Order> Orders = new List<Order>();
        public static List<Profile> Profiles = new List<Profile>();
        
        public static void SaveChanges() { }
        public static List<object> ExecuteQuery(string sql) { return new List<object>(); }
    }

    public static class SomeService
    {
        public static Task<string> Process(string data) { return Task.FromResult("SUCCESS"); }
    }
}