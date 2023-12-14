using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace website.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "user"; 
        public string? Path { get; set; }

        [NotMapped]
        [JsonIgnore]
        public IFormFile ImageFile { get; set; } = null!;

        public User()
        {
            Role = "user";
        }
    }
}
