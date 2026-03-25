namespace Uranus.Suite.ViewModels
{
    using System.Collections.Generic;

    public class PerfilDeAcessoViewModel
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public List<string> Claims { get; set; }
        public bool IsActive { get; set; }
        public int? UsersCount { get; set; }
        public List<string> UsersNames { get; set; }  // Lista de nomes para tooltip
        //public string Nome { get; set; }
        //public List<string> Roles { get; set; }
    }
}