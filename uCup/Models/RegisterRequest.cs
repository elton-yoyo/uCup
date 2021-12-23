using System.ComponentModel.DataAnnotations;

namespace uCup.Models
{
    public class RegisterRequest
    {
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string NTUStudentId { get; set; }
        [Required]
        public string UniqueId { get; set; }
    }
}