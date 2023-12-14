using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using website.Context;
using website.Models;
using website.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.IO;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace website.Controllers.Authentication
{

    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/auth")]
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

       [AllowAnonymous] // Đảm bảo endpoint không yêu cầu xác thực
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            try
            {
                Console.WriteLine($"Received user info: {user?.Name}, {user?.Email}, {user?.Role}");

                var result = await _authService.Authenticate(user);

                if (result.Success)
                {
                    return Ok(new { token = result.Token });
                }

                return Unauthorized(new { message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Received user info: {user?.Name}, {user?.Email}, {user?.Role}");
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }
    }

}
