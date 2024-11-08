using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities.Identity;

namespace AdminPanel.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public IActionResult Login()
        {
            return View();
        }

        #region SignIn
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (isPasswordCorrect)
                    {
                        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                        if (!result.Succeeded || !await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            ModelState.AddModelError(string.Empty, "You are not Authorized !");
                            return RedirectToAction("Login");
                        }
                        else
                            return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError(string.Empty, "Invalid Password");
                }
                ModelState.AddModelError(string.Empty, "Email is not Existed");
            }
            return View(model);
        }
        #endregion


        #region Sign Out
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        #endregion
    }
}
