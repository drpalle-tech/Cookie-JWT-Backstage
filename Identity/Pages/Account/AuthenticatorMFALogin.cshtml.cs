using Identity.Data.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Identity.Pages.Account
{
    public class AuthenticatorMFALoginModel : PageModel
    {
        [BindProperty]
        public AuthenticatorMFALoginViewModel AuthenticatorMFALoginViewModel { get; set; }

        private SignInManager<User> _signInManager;

        public AuthenticatorMFALoginModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            AuthenticatorMFALoginViewModel = new AuthenticatorMFALoginViewModel();
        }

        public void OnGet(bool rememberMe)
        {
            AuthenticatorMFALoginViewModel.RememberMe = rememberMe;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(AuthenticatorMFALoginViewModel.SecurityCode, AuthenticatorMFALoginViewModel.RememberMe, false);

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

    public class AuthenticatorMFALoginViewModel
    {
        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }
}
