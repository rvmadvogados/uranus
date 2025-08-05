using System;

namespace Uranus.Domain.Entities
{
    public class AgendasCanceladosNaocompareceu
    {
        public DateTime? Data { get; set; }
        public string Hora { get; set; }
        public Int32 IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public Int32 IdProfissional { get; set; }
        public string NomeProfissional { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public string Descricao { get; set; }
        public string Sede { get; set; }
        public string Assunto { get; set; }
        public Boolean Compareceu { get; set; }
        public Boolean Cancelou { get; set; }
        public string Tipo { get; set; }
    }

    public class RelatorioClientesIndicacao
    {
        public Int32 Id { get; set; }
        public string TipoIndicacao { get; set; }
        public string NomeCliente { get; set; }
        public string Indicacao { get; set; }
        public string NomeIndicacao { get; set; }
        public string Vinculo { get; set; }
        public DateTime? DataCadastroIndicacao { get; set; }
        public string NomeVinculo { get; set; }
        

    }

}