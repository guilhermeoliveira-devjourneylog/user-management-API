using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } 

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Department { get; set; }

        public string Role { get; set; } // Adicionada para validação do JWT

        public User()
        {
            Username = string.Empty;
            Password = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Department = string.Empty;
            Role = "User";
        }
    }
}
