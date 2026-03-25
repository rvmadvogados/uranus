using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Uranus.AuthApi.Helpers;
using Uranus.AuthApi.Models;

namespace Uranus.AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MigrationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public MigrationController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("create-legacy-user")]
        public async Task<IActionResult> CreateLegacyUser([FromBody] MigrationUserModel model)
        {
            try
            {
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null)
                {
                    return BadRequest($"Usu�rio '{model.UserName}' j� existe.");
                }

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email ?? $"{model.UserName}@legacyuser.com",
                    LegacyMd5Hash = model.LegacyMd5Hash,
                    LockoutEnabled = !model.IsActive
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                if (!string.IsNullOrEmpty(model.Role))
                {
                    var role = await _roleManager.FindByIdAsync(model.Role);
                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                }

                return Ok(new { message = "usuario legacy criado com sucesso", userId = user.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPost("batch-create-legacy-users")]
        public async Task<IActionResult> BatchCreateLegacyUsers([FromBody] BatchMigrationModel model)
        {
            var results = new List<object>();
            var successful = 0;
            var failed = 0;

            foreach (var userModel in model.Users)
            {
                try
                {
                    var existingUser = await _userManager.FindByNameAsync(userModel.UserName);
                    if (existingUser != null)
                    {
                        results.Add(new { 
                            userName = userModel.UserName, 
                            status = "error", 
                            message = "Usuario jaexiste" 
                        });
                        failed++;
                        continue;
                    }

                    var user = new ApplicationUser
                    {
                        UserName = userModel.UserName,
                  //      Email = userModel.Email ?? $"{userModel.UserName}@legacyuser.com",
                        LegacyMd5Hash = userModel.LegacyMd5Hash,
                        LockoutEnabled = !userModel.IsActive
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        results.Add(new { 
                            userName = userModel.UserName, 
                            status = "error", 
                            message = string.Join(", ", result.Errors.Select(e => e.Description)) 
                        });
                        failed++;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(userModel.Role))
                    {
                        var role = await _roleManager.FindByIdAsync(userModel.Role);
                        if (role != null)
                        {
                            await _userManager.AddToRoleAsync(user, role.Name);
                        }
                    }

                    results.Add(new { 
                        userName = userModel.UserName, 
                        status = "success", 
                        message = "Usuario criado com sucesso",
                        userId = user.Id
                    });
                    successful++;
                }
                catch (Exception ex)
                {
                    results.Add(new { 
                        userName = userModel.UserName, 
                        status = "error", 
                        message = ex.Message 
                    });
                    failed++;
                }
            }

            return Ok(new { 
                message = $"Migracao concluida. {successful} sucessos, {failed} falhas.", 
                successful, 
                failed, 
                results 
            });
        }

        [HttpPost("migrate-user-password")]
        public async Task<IActionResult> MigrateUserPassword([FromBody] MigratePasswordModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return NotFound("Usuario nao encontrado.");
                }

                if (string.IsNullOrEmpty(user.LegacyMd5Hash))
                {
                    return BadRequest("Usuario ja foi migrado ou nao possui hash legacy.");
                }

                
                if (!Md5Helper.VerificarHashMd5(model.Password, user.LegacyMd5Hash))
                {
                    return BadRequest("Senha nao confere com o hash legacy.");
                }

                var hasPassword = await _userManager.HasPasswordAsync(user);
                if (hasPassword)
                {
                    var removeResult = await _userManager.RemovePasswordAsync(user);
                    if (!removeResult.Succeeded)
                    {
                        return BadRequest(removeResult.Errors);
                    }
                }

                var addResult = await _userManager.AddPasswordAsync(user, model.Password);
                if (!addResult.Succeeded)
                {
                    return BadRequest(addResult.Errors);
                }

                user.LegacyMd5Hash = null;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return BadRequest(updateResult.Errors);
                }

                return Ok(new { message = "Senha migrada com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
    }

    public class MigratePasswordModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}