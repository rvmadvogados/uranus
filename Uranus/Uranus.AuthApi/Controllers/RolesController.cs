using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using Uranus.AuthApi.Data;
using Uranus.AuthApi.DTOs;
using Uranus.AuthApi.Models;

namespace Uranus.AuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class RolesController : ControllerBase
    {

        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesController(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = _roleManager.Roles
                //      .Where(r => r.IsActive)
                .ToList();

            var result = new List<object>();

            foreach (var role in roles)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                
                // Buscar usuários associados a este perfil com seus nomes
                var usersInRole = await _context.UserRoles
                    .Where(ur => ur.RoleId == role.Id)
                    .Join(_context.Users, ur => ur.UserId, u => u.Id, (ur, u) => u.UserName)
                    .ToListAsync();

                result.Add(new
                {
                    role.Id,
                    Role = role.Name,
                    Claims = claims.Select(c => $"{c.Type}:{c.Value}").ToList(),
                    role.IsActive,
                    UsersCount = usersInRole.Count,
                    UsersNames = usersInRole,  // Lista de nomes para tooltip
                    CreatedAt = role.Id 
                });
            }

            return Ok(result);
        }

        [HttpGet("roles-ativos")]
        public async Task<IActionResult> GetRolesAtivos()
        {
            var roles = _roleManager.Roles
                .Where(r => r.IsActive)
                .ToList();

            var result = new List<object>();

            foreach (var role in roles)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                result.Add(new
                {
                    role.Id,
                    Role = role.Name,
                    Claims = claims.Select(c => $"{c.Type}:{c.Value}").ToList(),
                    role.IsActive
                });
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            var claims = await _roleManager.GetClaimsAsync(role);
            var availableClaims = await _context.AvailableClaims.ToListAsync();

            var selectedClaimIds = availableClaims
                .Where(ac => claims.Any(rc => rc.Type == ac.Type && rc.Value == ac.Value))
                .Select(ac => ac.Id)
                .ToList();

            return Ok(new
            {
                role.Id,
                role.Name,
                Claims = claims.Select(c => new { c.Type, c.Value }),
                SelectedClaimIds = selectedClaimIds
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Role))
                return BadRequest("Nome do perfil é obrigatório.");
            var result = await _roleManager.CreateAsync(new ApplicationRole { Name = model.Role, IsActive = true });

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var role = await _roleManager.FindByNameAsync(model.Role);
            if (model.Claims != null)
            {
                var availableClaims = await _context.AvailableClaims
                    .Select(c => new { c.Type, c.Value })
                    .ToListAsync();

                foreach (var claim in model.Claims)
                {
                    if (!availableClaims.Any(ac => ac.Type == claim.Type && ac.Value == claim.Value))
                        return BadRequest($"Claim inválida: {claim.Type} - {claim.Value}");

                    await _roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));
                }
            }
            return Ok(new { role.Id, role.Name, Claims = model.Claims });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleModel model)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(model.Role) && role.Name != model.Role)
            {
                role.Name = model.Role;
                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }

            if (model.Claims != null)
            {
                var availableClaims = await _context.AvailableClaims
                    .Select(c => new { c.Type, c.Value })
                    .ToListAsync();

                foreach (var claim in model.Claims)
                {
                    if (!availableClaims.Any(ac => ac.Type == claim.Type && ac.Value == claim.Value))
                        return BadRequest($"Claim inválida: {claim.Type} - {claim.Value}");
                }

                var currentClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var claim in currentClaims)
                {
                    if (!model.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                        await _roleManager.RemoveClaimAsync(role, claim);
                }
                foreach (var claim in model.Claims)
                {
                    if (!currentClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                        await _roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));
                }
            }
            var updatedClaims = await _roleManager.GetClaimsAsync(role);
            return Ok(new
            {
                role.Id,
                role.Name,
                Claims = updatedClaims.Select(c => new { c.Type, c.Value }).ToList()
            });
        }

        // DELETE: api/roles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            role.IsActive = false;
            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }


        // GET: api/roles/available-claims
        [HttpGet("available-claims")]
        public async Task<IActionResult> GetAvailableClaims()
        {
            var claims = await _context.AvailableClaims
                .Include(c => c.ParentClaim)
                .Select(c => new AvailableClaimDto
                {
                    Id = c.Id,
                    Type = c.Type,
                    Value = c.Value,
                    Description = c.Description,
                    ParentClaimId = c.ParentClaimId,
                    //ParentType = c.ParentClaim != null ? c.ParentClaim.Type : null,
                    //ParentValue = c.ParentClaim != null ? c.ParentClaim.Value : null,
                    //ParentDescription = c.ParentClaim != null ? c.ParentClaim.Description : null
                })
                .ToListAsync();

            return Ok(claims);
        }



        [HttpPost("{id}/claims")]
        public async Task<IActionResult> AddClaimToRole(string id, [FromBody] ClaimModel claim)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            var exists = await _context.AvailableClaims
                .AnyAsync(c => c.Type == claim.Type && c.Value == claim.Value);
            if (!exists)
                return BadRequest("Claim não cadastrada como disponível.");

            // Já existe na role?
            var currentClaims = await _roleManager.GetClaimsAsync(role);
            if (currentClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                return BadRequest("Claim já associada à role.");

            await _roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));
            return Ok();
        }

        [HttpDelete("{id}/claims")]
        public async Task<IActionResult> RemoveClaimFromRole(string id, [FromBody] ClaimModel claim)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            var currentClaims = await _roleManager.GetClaimsAsync(role);
            var toRemove = currentClaims.FirstOrDefault(c => c.Type == claim.Type && c.Value == claim.Value);
            if (toRemove == null)
                return BadRequest("Claim não está associada à role.");

            await _roleManager.RemoveClaimAsync(role, toRemove);
            return Ok();
        }


        [HttpGet("{id}/claims")]
        public async Task<IActionResult> GetClaimsForRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            var claims = await _roleManager.GetClaimsAsync(role);
            return Ok(claims.Select(c => new { c.Type, c.Value }).ToList());
        }


        [HttpGet("user/{login}")]
        public async Task<IActionResult> GetRoleForUser(string login)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == login);
                if (user == null)
                    return NotFound();

                var userRole = await _context.UserRoles
                    .Where(ur => ur.UserId == user.Id && ur.RoleId != null)
                    .Select(ur => ur.RoleId)
                    .FirstOrDefaultAsync();

                if (userRole == null)
                    return Ok(null);

                var role = await _context.Roles
                    .Where(r => r.Id == userRole)
                    .Select(r => r.Name)
                    .FirstOrDefaultAsync();

                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar perfil do usuário: {ex.Message}");
            }
        }

        // GET: api/roles/{id}/users
        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetUsersInRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            try
            {
                var usersInRole = await _context.UserRoles
                    .Where(ur => ur.RoleId == id)
                    .Join(_context.Users, ur => ur.UserId, u => u.Id, (ur, u) => u.UserName)
                    .ToListAsync();

                return Ok(usersInRole);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar usuários do perfil: {ex.Message}");
            }
        }

        // PUT: api/roles/{id}/activate
        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateDeactivateRole(string id, [FromBody] ActivateRoleModel model)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            try
            {
                if (!model.IsActive)
                {
                    var usersInRole = await _context.UserRoles
                        .Where(ur => ur.RoleId == id)
                        .CountAsync();

                    if (usersInRole > 0)
                    {
                        return BadRequest(new
                        {
                            error = $"Não é possível inativar este perfil pois existem {usersInRole} usuário(s) associado(s) a ele."
                        });
                    }
                }

                role.IsActive = model.IsActive;
                var result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Ok(new
                {
                    id = role.Id,
                    name = role.Name,
                    isActive = role.IsActive,
                    message = $"Perfil {(model.IsActive ? "ativado" : "inativado")} com sucesso!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao alterar status do perfil: {ex.Message}");
            }
        }


    }
}