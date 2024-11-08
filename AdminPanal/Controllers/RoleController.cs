using AdminPanel.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        #region Index
        // /Role/Index/Guid
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel roleVM)
        {
            if (ModelState.IsValid)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleVM.RoleName);
                if (roleExists)
                {
                    ModelState.AddModelError("Name", "This Role Is already exists");
                }
                else
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleVM.RoleName.Trim()));
                    return RedirectToAction("Index");
                }
            }
            return View(roleVM);
        }
        #endregion

        #region Details
        // /Role/Details/Guid
        public async Task<IActionResult> Details(string id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(id);

            if (role is null)
                return NotFound();


            return View(viewName, role);
        }
        #endregion

        #region Edit
        // /Role/Edit/Guid
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var mappedRole = new RoleViewModel()
            {
                RoleName = role.Name
            };
            return View(mappedRole);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, RoleViewModel roleVm)
        {
            if (id != roleVm.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleVm.RoleName);
                if (roleExists)
                {
                    ModelState.AddModelError("RoleName", "This Role is already Exists !");
                }
                else
                {
                    var role = await _roleManager.FindByIdAsync(roleVm.Id);
                    role.Name = roleVm.RoleName;
                    await _roleManager.UpdateAsync(role);
                    return RedirectToAction("Index");
                }
            }

            return View(roleVm);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id, "Delete");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);

                await _roleManager.DeleteAsync(role);

                return RedirectToAction("Index");
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
