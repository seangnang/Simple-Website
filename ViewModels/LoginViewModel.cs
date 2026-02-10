using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SimpleWebsite.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress]
        public String Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
