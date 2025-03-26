using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public sealed class LoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
