using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using website.Context;
using website.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.IO;


namespace website.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        //Get all users
        [HttpGet]
        public IActionResult GetUsers()
        {
            List<User> users = _context.Users.ToList();
            return Ok(users);
        }

        //Get user by Id
        [HttpGet("{id}")]
        public IActionResult GetUsersById(int id)
        {
            try
            {
                var result = _context.Users.FirstOrDefault(x => x.Id == id);
                if (result == null)
                {
                    return NotFound(new {Message = "User not found" } ); 
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        // POST: Users/Create
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateUser([FromForm] User userInput)
        {
            try
            {
                Console.WriteLine($"User Name: {userInput.Name}");
                Console.WriteLine($"User Email: {userInput.Email}");
                Console.WriteLine($"File Name: {userInput.ImageFile?.FileName}");

                if (userInput.ImageFile != null && userInput.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "avatars");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(userInput.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await userInput.ImageFile.CopyToAsync(fileStream);
                    }

                    userInput.Path = "avatars/" + uniqueFileName;
                }

                _context.Add(userInput);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "User added successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        // PUT: Users/Edit/{id}
        [HttpPut("edit/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditUser(int id, [FromForm] User updatedInput)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(id);

                if (existingUser == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                existingUser.Name = updatedInput.Name;
                existingUser.Email = updatedInput.Email;
                existingUser.Role = updatedInput.Role;

                if (updatedInput.ImageFile != null && updatedInput.ImageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingUser.Path))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingUser.Path.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "avatars");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(updatedInput.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await updatedInput.ImageFile.CopyToAsync(fileStream);
                    }

                    existingUser.Path = "/avatars/" + uniqueFileName;
                }

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        // POST: Users/Delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var userToDelete = _context.Users.Find(id);
                if (userToDelete == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                _context.Users.Remove(userToDelete);
                _context.SaveChanges();
                return Ok(new { Message = "User is deleted successfully!!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
            }
        }


    }
}