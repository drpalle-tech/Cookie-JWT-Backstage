using Identity.Data.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.ComponentModel.DataAnnotations;

namespace Identity.Pages.Account
{
    //Only logged in users after first step can go for second factor authentication
    [Authorize]
    public class AuthenticatorMFASetupModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        [BindProperty]
        public bool IsSuccess { get; set; } = false;

        [BindProperty]
        public AuthenticatorMFASetupViewModel AuthenticatorMFASetupViewModel { get; set; }

        public AuthenticatorMFASetupModel(UserManager<User> userManager)
        {
            AuthenticatorMFASetupViewModel = new AuthenticatorMFASetupViewModel();
            _userManager = userManager;
        }

        //This is the authenticator setup page
        //The setup will be called once per user and from the next logins he can simply enter code from authenticator app.
        //During setup, the user will be provided a key/QR code
        //The user scans this key/QR frm his mobile
        //With this the web app shares a key with his authenticator app
        //The authenticator will generate a code using the key and the current time
        //Once he clicks on submit after entering OTP, the web app receives it
        //The web app also generates a code using the same key it already has and the time combination
        //These two codes are compared for equality 
        //If these matches, the user will be treated as a valid user.
        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                //We need to give a new key everytime user reloads, so reset
                await _userManager.ResetAuthenticatorKeyAsync(user);
                //Generate key to be entered in the authenticator app
                var key = await _userManager.GetAuthenticatorKeyAsync(user);
                AuthenticatorMFASetupViewModel.Key = key ?? string.Empty;
                AuthenticatorMFASetupViewModel.QRCode = GenerateQRCode("Dp Identity App", key ?? string.Empty, user.Email ?? string.Empty);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.GetUserAsync(User);

            if (user != null && await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, AuthenticatorMFASetupViewModel.Code))
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                IsSuccess = true;
            }
            else
            {
                ModelState.AddModelError("Authenticator", "Hey black sheep, Get off!");
            }
            return Page();
        }

        //Cannot type in the long key in authenticator everytime on setup
        //Scan a QR
        private byte[] GenerateQRCode(string provider, string key, string userEmail)
        {
            var qrCodeGenerator = new QRCodeGenerator();

            var qrCodeData = qrCodeGenerator.CreateQrCode($"otpauth://totp/{provider}:{userEmail}?secret={key}&issuer={provider}", QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }

    public class AuthenticatorMFASetupViewModel
    {
        //Nullable, if not Key will be treated as required field onPostAsync.
        //This is needed only to enter in the authenticator app, not to post back to the web app.
        public string? Key { get; set; } = string.Empty;

        public byte[]? QRCode { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
