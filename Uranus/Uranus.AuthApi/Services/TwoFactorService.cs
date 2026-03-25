using System;
using Microsoft.AspNetCore.Identity;
using Uranus.AuthApi.Models;

namespace Uranus.AuthApi.Services
{
    public class TwoFactorService : ITwoFactorService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TwoFactorService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task SaveLastTwoFactorConfirmedAsync(ApplicationUser user)
        {
            try
            {
                user.LastTwoFactorConfirmed = DateTime.UtcNow;
                var result = await _userManager.UpdateAsync(user);
                
                if (result.Succeeded)
                {
                    System.Diagnostics.Debug.WriteLine($"[2FA SERVICE - SALVO] Usuário: {user.UserName}, Timestamp: {user.LastTwoFactorConfirmed}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[2FA SERVICE - ERRO] Não foi possível salvar 2FA para {user.UserName}");
                    foreach (var error in result.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - {error.Description}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[2FA SERVICE - EXCEÇÃO] {ex.Message}");
            }
        }

        public bool IsTwoFactorStillValid(ApplicationUser user, int validityMinutes = 15)
        {
            try
            {
                if (user == null || user.LastTwoFactorConfirmed == null)
                {
                    return false;
                }

                var timeElapsed = DateTime.UtcNow - user.LastTwoFactorConfirmed.Value;
                bool isValid = timeElapsed.TotalMinutes < validityMinutes;

                return isValid;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[2FA SERVICE - ERRO VERIFICAR] {ex.Message}");
                return false;
            }
        }
        
        public async Task ClearLastTwoFactorConfirmedAsync(ApplicationUser user)
        {
            try
            {
                user.LastTwoFactorConfirmed = null;
                var result = await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[2FA SERVICE - ERRO AO LIMPAR] {ex.Message}");
            }
        }
    }
}
