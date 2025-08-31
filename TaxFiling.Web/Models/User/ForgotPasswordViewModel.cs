using System.ComponentModel.DataAnnotations;

namespace TaxFiling.Web.Models.User
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
