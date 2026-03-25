using System;

namespace Sigman.Domain.Entities
{
    public class Fornecedores
    {
        public int ID { get; set; }
        public string TipoPessoa { get; set; }
        public string CpfCnpj { get; set; }
        public string Nome { get; set; }
        public string NomeFantasia { get; set; }
        public bool? Ativo { get; set; }
        public long? RG { get; set; }
        public string InscricaoEstadual { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string Cnae { get; set; }
        public string Vendedor { get; set; }
        public decimal? LimiteCredito { get; set; }
        public bool? Estrangeiro { get; set; }
        public string Cep { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string CodigoMunicipio { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public DateTime? DataCadastro { get; set; }
        public string BancoNumero { get; set; }
        public string AgenciaNumero { get; set; }
        public string ContaNumero { get; set; }
        public string HomePage { get; set; }
        public int? CodigoLegado { get; set; }
    }
}
