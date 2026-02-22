using System.ComponentModel.DataAnnotations;

namespace SimpleWebsite.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        public string Fullname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}