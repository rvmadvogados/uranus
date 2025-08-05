using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Business.DTOs
{
    public class EventoDTO
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Hora { get; set; }
        public string Encaixe { get; set; }

        public int? IdSede { get; set; }
        public int? IdProfissional { get; set; }
        public int? IdCliente { get; set; }
        public int? IdTipo { get; set; }
        public int? IdSala { get; set; }
        public int? IdUsuario { get; set; }

        public string NomeProfissional { get; set; }
        public string NomeCliente { get; set; }
        public string NomeTipo { get; set; }
        public string NomeSede { get; set; }
        public string NomeSala { get; set; }
    }
}
