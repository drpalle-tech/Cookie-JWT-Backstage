using Identity.Data.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Identity.Pages.Account
{
    public class UserProfileModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        [BindProperty]
        public UserProfileViewModel UserProfileViewModel { get; set; }

        //Must be nullable
        //If not nullable, this will be considered as required during input submission of model.
        [BindProperty]
        public string? SuccessMessage { get; set; }

        public UserProfileModel(UserManager<User> userManager)
        {
            _userManager = userManager;
            UserProfileViewModel = new UserProfileViewModel();
        }

        public async Task<IActionResult> OnGet()
        {
            var (user, depClaim, posClaim) = await GetUserClaims();
            if (user != null)
            {
                UserProfileViewModel.Email = User.Identity?.Name ?? string.Empty;
                UserProfileViewModel.Department = depClaim?.Value ?? string.Empty;
                UserProfileViewModel.Position = posClaim?.Value ?? string.Empty;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
                return Page();

            var (user, depClaim, posClaim) = await GetUserClaims();
            try
            {
                if (user != null && depClaim != null)
                {
                    await _userManager.ReplaceClaimAsync(user, depClaim, new Claim(depClaim.Type, UserProfileViewModel.Department));
                }
                if (user != null && posClaim != null)
                {
                    await _userManager.ReplaceClaimAsync(user, posClaim, new Claim(posClaim.Type, UserProfileViewModel.Position));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("What re! black sheep? Trying to inject attacks or what?", ex.Message);
            }

            SuccessMessage = "Enjoy, Your updates are locked and loaded!";
            return Page();
        }

        private async Task<(User? user, Claim? depClaim, Claim? posClaim)> GetUserClaims()
        {
            //Return a tuple of user and claim combinations.
            var user = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);

            if (user != null)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                var depClaim = claims.FirstOrDefault(c => c.Type == "Dept");
                var posClaim = claims.FirstOrDefault(c => c.Type == "Pos");

                return (user, depClaim, posClaim);
            }
            else
            {
                return (null, null, null);
            }
        }
    }

    public class UserProfileViewModel
    {
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty;
    }
}
