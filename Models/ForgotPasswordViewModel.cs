using System.ComponentModel.DataAnnotations;

namespace MovieWebsite.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email {  get; set; }
    }
}
