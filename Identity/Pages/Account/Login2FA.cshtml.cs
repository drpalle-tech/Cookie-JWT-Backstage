using Identity.Data.Account;
using Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Identity.Pages.Account
{
    public class Login2FAModel : PageModel
    {
        private UserManager<User> _userManager;

        private SignInManager<User> _signInManager;

        private IEmailService _emailService;

        [BindProperty]
        public Login2FAViewModel Login2FAViewModel { get; set; }

        public Login2FAModel(UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            Login2FAViewModel = new Login2FAViewModel();
        }

        public async Task OnGet(string email, bool rememberMe)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user != null)
            {
                //Generare OTP using signInManager
                var verificationCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                //Send OTP to his mail
                await _emailService.SendEmailAsync(email, "Hey King! Here's your OTP!", $"Dont worry, we won't loot your bank balance :p {verificationCode}");

                Login2FAViewModel.RememberMe = rememberMe;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Validate the submitted OTP
            //Compare the submitted OTP and the previously generated cookie with verification code 
            //Once the above equality check successes, you are valid user.
            var result = await _signInManager.TwoFactorSignInAsync("Email", Login2FAViewModel.SecurityCode, Login2FAViewModel.RememberMe, false);

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

    public class Login2FAViewModel
    {
        [Required]
        public string SecurityCode { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }
}
