using System;

namespace Sigman.Domain.Entities
{
    public class Clientes
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string TipoPessoa { get; set; }
        public string CpfCnpj { get; set; }
        public string InscricaoEstadual { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string RG { get; set; }
        public string Cnae { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataNascimento { get; set; }
        public int? IdVendedor { get; set; }
        public string Status { get; set; }
        public string HomePage { get; set; }
        public int? CodigoLegado { get; set; }
        public int? EmpresaLegado { get; set; }
        public bool? CreditoIcms { get; set; }
        public bool? ConsumidorFinal { get; set; }
        public int? ContribuinteIcms { get; set; }
    }
}
