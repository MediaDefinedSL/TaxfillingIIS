using System.ComponentModel.DataAnnotations;

namespace TaxFiling.Web.Models.User
{
    public class ResetPasswordModel
    {
        public string Email { get; set; }
        public string Token { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
