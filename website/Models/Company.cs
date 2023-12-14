using System.ComponentModel.DataAnnotations;

namespace website.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Path { get; set; }
    }
}
