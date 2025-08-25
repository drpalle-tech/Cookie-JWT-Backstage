using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UnderTheHoodAuth.Models
{
    public class LoginInputModel
    {
        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DisplayName("Password")]
        public string Password { get; set; } = string.Empty;

        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; } = false;
    }
}
