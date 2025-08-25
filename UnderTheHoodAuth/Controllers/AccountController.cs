using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Security.Claims;
using UnderTheHoodAuth.Models;

namespace UnderTheHoodAuth.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (!ModelState.IsValid) 
                    return RedirectToAction("Error", "Home");

            if(model.UserName == "s" && model.Password == "s")
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, model.UserName),
                    new Claim("admin", "true"),
                    new Claim("DOJ", "2021-01-01")
                };
                var identity = new ClaimsIdentity(claims, "DpCookie");

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                var authProperty = new AuthenticationProperties()
                {
                    //Indicates that the cookie lifetime is valid, even if the browser or application is closed and re opened.
                    IsPersistent = model.RememberMe
                };

                //Expects cookie name, prinicipal (containing data of logged in user claims), properties of cookie.
                // Here's where the magic happens
                await HttpContext.SignInAsync("DpCookie", claimsPrincipal, authProperty);

                return RedirectToAction("Index", "Home");

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("DpCookie");
            return RedirectToAction("Index", "Home");
        }
    }
}
