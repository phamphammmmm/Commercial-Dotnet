using System.Threading.Tasks;
using website.Models;

namespace website.Services
{
    public interface IAuthService
    {
        Task<AuthenticationResult> Authenticate (User user);
    }

    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string Token { get; set; } = null!;
    }
}
