using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigman.Domain.Entities
{
    public class Produtos
    {
        public long ID { get; set; }
        public string IdTipo { get; set; }
        public bool Ativo { get; set; }
        public string Nome { get; set; }
        public int? IdFabricante { get; set; }
        public string IdApresentacao { get; set; }
        public int? IdCategoria { get; set; }
        public int? IdClasse { get; set; }
        public int? IdFamilia { get; set; }
        public int? IdGrupo { get; set; }
        public string Unidade { get; set; }
        public decimal? PrecoCusto { get; set; }
        public decimal? PrecoVenda { get; set; }
        public decimal? PrecoVenda1 { get; set; }
        public decimal? PrecoVenda2 { get; set; }
        public decimal? PrecoVenda3 { get; set; }
        public decimal? PrecoMedio { get; set; }
        public decimal? MargemLucro { get; set; }
        public decimal? MargemLucro1 { get; set; }
        public decimal? MargemLucro2 { get; set; }
        public decimal? MargemLucro3 { get; set; }
        public string SituacaoTributaria { get; set; }
        public decimal? ICMS { get; set; }
        public decimal? IPI { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataReajuste { get; set; }
        public string NCM { get; set; }
        public decimal? QuantidadeAtual { get; set; }
        public decimal? QuantidadeMinima { get; set; }
        public decimal? QuantidadeMaxima { get; set; }
        public bool? VendaSemEstoque { get; set; }
        public bool? Promocao { get; set; }
        public DateTime? DataInicioPromocao { get; set; }
        public DateTime? DataFimPromocao { get; set; }
        public decimal? ValorPromocao { get; set; }
        public int? IdArea { get; set; }
        public string Almoxarifado { get; set; }
        public string Prateleira { get; set; }
        public decimal? PercentualComissao { get; set; }
        public string TipoFinanceiro { get; set; }
        public string Descricao { get; set; }
        public string Aplicacao { get; set; }
        public string Observacao { get; set; }
        public string Codigo { get; set; }
        public int? Reduzido { get; set; }
        public string Referencia { get; set; }
        public string Origem { get; set; }
        public string UrlAmigavel { get; set; }
        public string QRCode { get; set; }
        public bool? Encomenda { get; set; }
    }
}
