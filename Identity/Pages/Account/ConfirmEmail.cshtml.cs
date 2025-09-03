using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        public string Message { get; set; } = string.Empty;
        private UserManager<IdentityUser> _userManager;
        public ConfirmEmailModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGet(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if(user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded) {
                    Message = "You've proved you're a human! Proceed to login.";
                    return Page();
                }
            }
            Message = "Hey black sheep!!! How are ya?";
            return Page();
        }
    }
}
