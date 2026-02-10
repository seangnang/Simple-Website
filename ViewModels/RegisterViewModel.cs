using System.ComponentModel.DataAnnotations;

namespace SimpleWebsite.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Name is required!")]
        public String Name { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress]
        public String Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [StringLength(40,MinimumLength =8, ErrorMessage ="the {0} must be at {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        [Compare("ComfirmPassword", ErrorMessage ="Password not match!")]
        public String  Password { get; set; }

        [Required(ErrorMessage = "Comfirm is required!")]
        [DataType(DataType.Password)]
        public String  ComfirmPassword { get; set; }
    }
}
