using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Identity.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginViewModel LoginViewModel { get; set; } = new LoginViewModel();

        private readonly ILogger<LoginModel> _logger;
        //This is internal class offered by Identity nuget to have signup related actions like logging in, logging out.
        private SignInManager<IdentityUser> _signInManager;

        public LoginModel(ILogger<LoginModel> logger, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            var result = await _signInManager.PasswordSignInAsync(LoginViewModel.Email, LoginViewModel.Password, LoginViewModel.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Locked out", "Wait ma, what is your hurry!");
                }
                else
                {
                    ModelState.AddModelError("Login", "Black sheep, haha!!");
                }
            }
            
            return Page();
        }
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Get off bot!!")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password {  get; set; } = string.Empty;

        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; } = false;
    }

}
