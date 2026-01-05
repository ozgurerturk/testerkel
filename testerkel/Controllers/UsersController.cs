using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using testerkel.Data;
using testerkel.Models;
using testerkel.ViewModels.Auth;

namespace testerkel.Controllers
{
    [Authorize(Roles = "Admin")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class UsersController(ILogger<UsersController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ErkelErpDbContext context) : Controller
    {
        private readonly ILogger<UsersController> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly ErkelErpDbContext _context = context;

        public IActionResult Index()
        {
            var users = from u in _userManager.Users
                        join ur in _context.UserRoles on u.Id equals ur.UserId
                        join r in _roleManager.Roles on ur.RoleId equals r.Id
                        select new UserViewModel
                        {
                            Id = u.Id,
                            UserName = u.UserName ?? string.Empty,
                            Email = u.Email ?? string.Empty,
                            NormalizedEmail = u.NormalizedEmail ?? string.Empty,
                            EnabledNotifications = u.EnableNotifications,
                            Role = new RoleViewModel
                            {
                                Id = r.Id,
                                Name = r.Name ?? string.Empty,
                                NormalizedName = r.NormalizedName ?? string.Empty
                            }
                        };

            return View(users);
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Roles = _roleManager.Roles
                .Select(r => new RoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name ?? string.Empty,
                    NormalizedName = r.NormalizedName ?? string.Empty
                })
                .ToList();
            var user = new UserViewModel();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel user)
        {
            if (user == null)
                return Ok(new { Result = false, ErrorMessage = "UserViewModel parameter is null." });

            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Ok(new { Result = false, Errors = errorMessages });
            }

            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == user.Role.Id);

            if (role == null || string.IsNullOrWhiteSpace(role.Name))
            {
                return Ok(new
                {
                    Result = false,
                    Errors = new List<string> { "Role couldn't be found in the database" }
                });
            }

            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                var newUser = new ApplicationUser
                {
                    Email = user.Email,
                    UserName = user.Email,
                    EnableNotifications = user.EnabledNotifications
                };

                var createResult = await _userManager.CreateAsync(newUser, user.Password);

                if (!createResult.Succeeded)
                {
                    await tx.RollbackAsync();

                    return Ok(new
                    {
                        Result = false,
                        Errors = createResult.Errors.Select(e => $"{e.Code} - {e.Description}").ToList()
                    });
                }

                var roleResult = await _userManager.AddToRoleAsync(newUser, role.Name);

                if (!roleResult.Succeeded)
                {
                    await tx.RollbackAsync();
                    return Ok(new
                    {
                        Result = false,
                        Errors = roleResult.Errors.Select(e => $"{e.Code} - {e.Description}").ToList()
                    });
                }

                await tx.CommitAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();

                return Ok(new
                {
                    Result = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Ok(new
                {
                    Result = false,
                    Errors = new List<string> { "User Id received empty or null" }
                });
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Ok(new
                {
                    Result = false,
                    Errors = new List<string> { "User cannot be found" }
                });
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return Ok(new
                {
                    Result = false,
                    Errors = result.Errors.Select(e => $"{e.Code} - {e.Description}").ToList()
                });
            }

            return Ok(new
            {
                Result = true,
                Data = user.Email
            });
        }

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

                var changeResult = await _userManager.ResetPasswordAsync(user, token, "MyN3wP@ssw0rd");

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
