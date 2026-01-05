using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using testerkel.Models;
using testerkel.ViewModels.Auth;

namespace testerkel.Controllers
{
    public class AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Şifre veya email yanlış");
                }
            }

            return View(model);
        }

        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [HttpGet]
        public async Task<IActionResult> Details(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            // Get user role
            var userRoles = await _userManager.GetRolesAsync(user);

            var roleName = userRoles.FirstOrDefault();

            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

            if (role == null)
            {
                return RedirectToAction("Index");
            }

            return View(new UserViewModel()
            {
                Email = !string.IsNullOrEmpty(user.Email) ? user.Email : string.Empty,
                Id = Id,
                Role = new RoleViewModel() { Id = role.Id, Name = role.Name!, NormalizedName = role.NormalizedName! },
                EnabledNotifications = user.EnableNotifications,
                NormalizedEmail = !string.IsNullOrEmpty(user.NormalizedEmail) ? user.NormalizedEmail : string.Empty
            });
        }

        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeVm param)
        {
            if (param == null)
            {
                return Ok(new { Result = false, Errors = new List<string> { "param received is null" } });
            }

            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Ok(new { Result = false, Errors = errorMessages });
            }

            if (!param.NewPassword.Equals(param.NewPasswordConfirm))
            {
                return Ok(new { Result = false, Errors = new List<string> { "Yeni şifreler eşleşmiyor." } });
            }

            try
            {
                var user = _userManager.Users.FirstOrDefault(rec => rec.Id == param.UserId);

                if (user is null)
                {
                    return Ok(new { Result = false, Errors = new List<string> { "Kullanıcı bulunamadı." } });
                }

                var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, param.CurrentPassword);

                if (result == PasswordVerificationResult.Failed)
                {
                    return Ok(new { Result = false, Errors = new List<string> { "Girilen şifre eski şifre ile uyuşmuyor!" } });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var changeResult = await _userManager.ResetPasswordAsync(user, token, param.NewPassword);

                if (!changeResult.Succeeded)
                {
                    return Ok(new { Result = false, Errors = changeResult.Errors.Select(e => $"{e.Code} - {e.Description}").ToList() });
                }

                return Ok(new { Result = true, Data = user.Email });
            }
            catch (Exception ex)
            {
                return Ok(
                    new
                    {
                        Result = false,
                        Errors = new StringBuilder()
                            .AppendLine(ex.Message)
                            .AppendLine(ex.InnerException?.Message ?? string.Empty)
                            .ToString()
                    });
            }
        }
    }
}
