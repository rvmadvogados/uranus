namespace Sigman.Domain.Entities
{
    public class NotasFiscaisProdutos
    {
        public long ID { get; set; }
        public long IdNotaFiscal { get; set; }
        public int Sequencia { get; set; }
        public long? IdProduto { get; set; }
        public long? IdProdutoItem { get; set; }
        public string Descricao { get; set; }
        public string Unidade { get; set; }
        public string SituacaoTributaria { get; set; }
        public decimal? Quantidade { get; set; }
        public decimal? PrecoUnitario { get; set; }
        public decimal? PrecoTotal { get; set; }
        public decimal? PercentualDesconto { get; set; }
        public decimal? ValorDesconto { get; set; }
        public decimal? BaseCalculoIcms { get; set; }
        public decimal? PercentualIcms { get; set; }
        public decimal? ValorIcms { get; set; }
        public decimal? PercentualIpi { get; set; }
        public decimal? ValorIpi { get; set; }
        public decimal? BaseCalculoSt { get; set; }
        public decimal? PercentualSt { get; set; }
        public decimal? ValorIcmsSt { get; set; }
        public decimal? PercentualReducao { get; set; }
        public decimal? ValorReducaoSt { get; set; }
        public decimal? CustoProduto { get; set; }
        public string Ncm { get; set; }
        public string Cfop { get; set; }
        public decimal? Mva { get; set; }
        public string ReferenciaFornecedor { get; set; }
        public decimal? ImpostoAproximado { get; set; }
        public string Csosn { get; set; }
        public int? IdNotaFiscalEntrada { get; set; }
        public decimal? OutrasDespesas { get; set; }
        public decimal? ValorFrete { get; set; }
        public decimal? IcmsOperacao { get; set; }
        public decimal? PercDiferido { get; set; }
        public decimal? IcmsDiferido { get; set; }
        public string CodigoProdutoFornecedor { get; set; }

        public virtual Produtos Produtos { get; set; }
    }
}
