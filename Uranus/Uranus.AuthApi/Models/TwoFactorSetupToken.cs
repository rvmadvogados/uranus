using System;

namespace Uranus.AuthApi.Models
{
  /// <summary>
    /// ? Modelo para armazenar tokens temporários de reconfiguração de 2FA
    /// </summary>
    public class TwoFactorSetupToken
    {
      public int Id { get; set; }
        public string Username { get; set; }
  public string Token { get; set; } // Token aleatório e seguro
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }
    }
}
