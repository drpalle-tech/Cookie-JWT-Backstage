using Identity.Data.Account;
using Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Identity.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class SignupModel : PageModel
    {
        [BindProperty]
        public SignupViewModel SignupViewModel { get; set; } = new SignupViewModel();

        private readonly ILogger<SignupModel> _logger;
        //This is internal class offered by Identity nuget to have user related actions like creating, deleting etc.,
        private readonly UserManager<User> _userManager;

        private readonly IEmailService _emailService;

        public SignupModel(ILogger<SignupModel> logger, UserManager<User> userManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _emailService = emailService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //Validation of email done in Program.cs using AddIdentity

            //Create user
            var user = new User
            {
                Email = SignupViewModel.Email,
                UserName = SignupViewModel.Email,
                //For Email
                //TwoFactorEnabled = true,
            };
            
            //Create custom claims for the user.
            //Could be dept or pos claim.
            var depClaim = new Claim("Dept", SignupViewModel.Department);
            var posClaim = new Claim("Pos", SignupViewModel.Position);

            //Direct user creation to the identity package.
            //Dont take any headache.
            var result = await _userManager.CreateAsync(user, SignupViewModel.Password);

            if (result.Succeeded)
            {
                //Add claims to the user.
                await _userManager.AddClaimAsync(user, depClaim);
                await _userManager.AddClaimAsync(user, posClaim);

                //To validate the user entered mail
                //Generate a token (similar to JWT) and keep it with Identity
                //User goes to mail and clicks on link to confirm.
                //Once user clicks confirm, our web app server receives a token
                //Similar to JWT validation, these 2 tokens are compared for equality
                //If equal, the user's email is confirmed, as the source of new request is from link click in email.
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //For now write a new page to click confirm email instead of going to email
                return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values: new { userId = user.Id, token = confirmationToken }) ?? string.Empty);

                //If you really want to send email and take confirmation
                //Uses Brevo as Mail sender 3rd party service 
                //Details of Brevo login to be provided in appsettings
                //Once link is clicked in Email, your confirmEMail page will be displayed as redirect.

                var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values: new { userId = user.Id, token = confirmationToken });
                await _emailService.SendEmailAsync(user.Email, "Are you a human for sure?", $"Please prove you're a human {confirmationLink}");

                return RedirectToPage("/Account/Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    //For this errors to reflect, the HTML must have asp-validation-summary="All"
                    ModelState.AddModelError("Register User Error", error.Description);
                }
            }
            return Page();
        }
    }

    public class SignupViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Get off bot!!")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password {  get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty;
    }

}
