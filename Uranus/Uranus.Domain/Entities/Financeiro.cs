using System;

namespace Uranus.Domain.Entities
{
    public class ReceberEmAbertoEmissao
    {
        public Int32 IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public long? NumeroDocumento { get; set; }
        public DateTime? DataEmissao { get; set; }
        public Int32? Parcela { get; set; }
        public DateTime? DataVencimento { get; set; }
        public decimal? ValorParcela { get; set; }
    }

    public class ReceberEmAbertoVencimento
    {
        public Int32 IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public long? NumeroDocumento { get; set; }
        public DateTime? DataEmissao { get; set; }
        public Int32? Parcela { get; set; }
        public DateTime? DataVencimento { get; set; }
        public decimal? ValorParcela { get; set; }
        public string FormaPagamento { get; set; }
        public string Sede { get; set; }
        public string NomeVinculo { get; set; }
        
    }

    public class ReceberPagas
    {
        public Int32 IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public long? NumeroDocumento { get; set; }
        public DateTime? DataEmissao { get; set; }
        public Int32? Parcela { get; set; }
        public DateTime? DataVencimento { get; set; }
        public decimal? ValorParcela { get; set; }
        public DateTime? DataPagamento { get; set; }
        public decimal? Juros { get; set; }
        public decimal? Descontos { get; set; }
        public decimal? ValorPago { get; set; }
    }

    public class PagarEmAbertoEmissao
    {
        public Int32 IdFornecedor { get; set; }
        public string NomeFornecedor { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime? DataEmissao { get; set; }
        public Int32? Parcela { get; set; }
        public DateTime? DataVencimento { get; set; }
        public decimal? ValorParcela { get; set; }
    }

    public class PagarEmAbertoVencimento
    {
        public Int32 IdFornecedor { get; set; }
        public string NomeFornecedor { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime? DataEmissao { get; set; }
        public Int32? Parcela { get; set; }
        public DateTime? DataVencimento { get; set; }
        public decimal? ValorParcela { get; set; }
    }

    public class PagarPagas
    {
        public Int32 IdFornecedor { get; set; }
        public string NomeFornecedor { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime? DataEmissao { get; set; }
        public Int32? Parcela { get; set; }
        public DateTime? Vencimento { get; set; }
        public decimal? ValorParcela { get; set; }
        public DateTime? DataPagamento { get; set; }
        public decimal? Juros { get; set; }
        public decimal? Descontos { get; set; }
        public decimal? ValorPago { get; set; }
        public string Observacao { get; set; }
    }

    public class PagarRolPagamentos
    {
        public Int32 IdFornecedor { get; set; }
        public string NomeFornecedor { get; set; }
        public string NumeroDocumento { get; set; }
        public Int32? Parcela { get; set; }
        public DateTime? Vencimento { get; set; }
        public decimal? ValorParcela { get; set; }
    }

    public class ConsultaFinanceiro
    {
        public Int32 IdCliente { get; set; }
        public Int32 IdPessoa { get; set; }
        public string NomePessoa { get; set; }
        public Int64? NumeroDocumento { get; set; }
        public DateTime? DataEmissao { get; set; }
        public Int32? Parcela { get; set; }
        public DateTime? Vencimento { get; set; }
        public decimal? ValorParcela { get; set; }
        public DateTime? DataPagamento { get; set; }
    }
}