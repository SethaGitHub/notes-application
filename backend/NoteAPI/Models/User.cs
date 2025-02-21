using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [Column(TypeName = "varchar(255)")]
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
