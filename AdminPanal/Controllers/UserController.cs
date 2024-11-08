using AdminPanel.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities.Identity;

namespace AdminPanel.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        #region Index
        // /User/Index/Guid
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.Select(U => new UserViewModel()
            {
                Id = U.Id,
                UserName = U.UserName,
                Email = U.Email,
                DisplayName = U.DisplayName,
                PhoneNumber = U.PhoneNumber,
                Roles = _userManager.GetRolesAsync(U).Result
            }).ToListAsync();
            return View(users);
        }
        #endregion


        #region Details
        // /User/Details/Guid
        // [HttpGet]
        public async Task<IActionResult> Details(string id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            var mappedUser = new UserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = _userManager.GetRolesAsync(user).Result
            };

            return View(viewName, mappedUser);
        }
        #endregion


        #region Edit
        // /User/Edit/Guid
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var allRoles = await _roleManager.Roles.ToListAsync();
            var viewModel = new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.DisplayName,
                Roles = allRoles.Select(R => new RoleViewModel
                {
                    Id = R.Id,
                    RoleName = R.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, R.Name).Result
                }).ToList()
            };

            return View(viewModel);
        }


        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, UserRolesViewModel userRolesVM)
        {
            if (id != userRolesVM.UserId)
                return BadRequest();


            var user = await _userManager.FindByIdAsync(userRolesVM.UserId);
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRolesVM.Roles)
            {
                if (userRoles.Any(R => R == role.RoleName) && !role.IsSelected)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.RoleName);
                }
                if (!userRoles.Any(R => R == role.RoleName) && role.IsSelected)
                {
                    await _userManager.AddToRoleAsync(user, role.RoleName);
                }
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion


        #region Delete
        // /User/Delete/Guid
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id, "Delete");
        }

        //[HttpPost(Name = "Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                await _userManager.DeleteAsync(user);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }
        #endregion
    }
}
