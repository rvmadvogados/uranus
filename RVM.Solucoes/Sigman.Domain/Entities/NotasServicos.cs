using System;

namespace Sigman.Domain.Entities
{
    public class NotasServicos
    {
        public long Id { get; set; }
        public int IdEmpresa { get; set; }
        public int? Ano { get; set; }
        public long? NumeroNota { get; set; }
        public long? IdOrdemServico { get; set; }
        public long? NumeroOS { get; set; }
        public DateTime? Data { get; set; }
        public int? IdStatus { get; set; }
        public int? IdCliente { get; set; }
        public long? IdClienteEndereco { get; set; }
        public int? IdClienteEmail { get; set; }
        public string DescricaoServico { get; set; }
        public decimal? ValorServico { get; set; }
        public int? PercentualIssqn { get; set; }
        public decimal? ValorIssqn { get; set; }
        public int? Plano { get; set; }
        public int? LocalPagamento { get; set; }
        public string Observacao { get; set; }
        public int? IdFormaPagamento { get; set; }
        public string ReterIssqn { get; set; }
        public long? NumeroReferencia { get; set; }

        public virtual Clientes Clientes { get; set; }
        public virtual string ClientesEmails { get; set; }
        public virtual ClientesEnderecos ClientesEnderecos { get; set; }
        public virtual Empresas Empresas { get; set; }

    }
}
