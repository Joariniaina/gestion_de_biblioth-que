using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Web.DTOs
{
    public class RegisterDto
    {
        [Required] 
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress] 
        public string Email { get; set; } = string.Empty;

        [Required] 
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "User"; // Valides : User, Librarian, Admin

        // Champs supplémentaires facultatifs
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
