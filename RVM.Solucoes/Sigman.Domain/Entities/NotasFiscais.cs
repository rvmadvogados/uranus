using System;

namespace Sigman.Domain.Entities
{
    public class NotasFiscais
    {
        public long ID { get; set; }
        public int IdEmpresa { get; set; }
        public long? NumeroNota { get; set; }
        public string Tipo { get; set; }
        public int? IdCliente { get; set; }
        public int? IdClienteEmail { get; set; }
        public long? IdClienteEndereco { get; set; }
        public int? IdFornecedor { get; set; }
        public DateTime? DataEmissao { get; set; }
        public DateTime? DataSaida { get; set; }
        public string ConsumidorFinal { get; set; }
        public int? ContribuinteIcms { get; set; }
        public int? IdNaturezaOperacao { get; set; }
        public decimal? TotalProduto { get; set; }
        public decimal? TotalNota { get; set; }
        public decimal? ValorServico { get; set; }
        public decimal? Desconto { get; set; }
        public decimal? DescontoPecas { get; set; }
        public decimal? ValorIssqn { get; set; }
        public decimal? PercentualIssqn { get; set; }
        public string DescricaoServico { get; set; }
        public decimal? BaseCalculoIcms { get; set; }
        public decimal? PercentualIcms { get; set; }
        public decimal? TotalIcms { get; set; }
        public decimal? TotalIpi { get; set; }
        public decimal? BaseIcmsSubstituicao { get; set; }
        public decimal? ValorIcmsSubstituicao { get; set; }
        public int? IdTransportadora { get; set; }
        public int? TipoFrete { get; set; }
        public decimal? ValorFrete { get; set; }
        public decimal? ValorSeguro { get; set; }
        public decimal? OutrasDespesas { get; set; }
        public int? QuantidadeVolumes { get; set; }
        public string EspecieVolumes { get; set; }
        public string Marca { get; set; }
        public string Numero { get; set; }
        public decimal? PesoBruto { get; set; }
        public decimal? PesoLiquido { get; set; }
        public string Observacao { get; set; }
        public int? QuantidadeParcelas { get; set; }
        public int? LocalPagamento { get; set; }
        public string Cancelada { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public string Motivo { get; set; }
        public string Recebedor { get; set; }
        public decimal? TotalCusto { get; set; }
        public string IdentificadorPagamento { get; set; }
        public string TipoPagamento { get; set; }
        public string IdentificadorPresenca { get; set; }
        public string CnpjIntermediador { get; set; }
        public string NomeIntermediador { get; set; }
        public decimal? TotalImpostoAproximado { get; set; }
        public long? IdPedido { get; set; }
        public long? IdOrdemServico { get; set; }
        public int? IdStatus { get; set; }
        public int? IdCondicaoPagamento { get; set; }
        public int? IdFornecedorNotaEntrada { get; set; }
        public int? IdFornecedorEndereco { get; set; }
        public int? IdFornecedorEmail { get; set; }
        public long? NumeroNotaFornecedor { get; set; }
        public string NumeroChaveNfe { get; set; }
        public long? NumeroReferencia { get; set; }


        public virtual Clientes Clientes { get; set; }
        public virtual string ClientesEmails { get; set; }
        public virtual ClientesEnderecos ClientesEnderecos { get; set; }
        //public virtual ClientesPedidos ClientesPedidos { get; set; }
        //public virtual CondicoesPagamentos CondicoesPagamentos { get; set; }
        public virtual Empresas Empresas { get; set; }
        public virtual Fornecedores Fornecedores { get; set; }
        public virtual NaturezaOperacoes NaturezaOperacoes { get; set; }
        //public virtual Status Status { get; set; }
    }
}
