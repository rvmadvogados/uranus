using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taylor.Data.R9
{
    public class ItemPagarPagas : BaseModel
    {
        [Column] public Int32 IdFornecedor { get; set; }
        [Column] public string NomeFornecedor { get; set; }
        [Column] public string NumeroDocumento { get; set; }
        [Column] public DateTime DataEmissao { get; set; }
        [Column] public Int32 Parcela { get; set; }
        [Column] public DateTime DataVencimento { get; set; }
        [Column] public Decimal ValorParcela { get; set; }
        [Column] public DateTime DataPagamento { get; set; }
        [Column] public Decimal Juros { get; set; }
        [Column] public Decimal Descontos { get; set; }
        [Column] public Decimal ValorPago { get; set; }
    }
    public class ItemPagarPagasAdapter
    {
        public List<ItemPagarPagas> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            return PagarPagas.Gerar(dataInício, dataFim, conexão);
        }
    }

    public static class PagarPagas
    {
        public static List<ItemPagarPagas> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            DataContext context = new DataContext(conexão);

            return global::R9.DataBase.Data.BuildAllObjects<ItemPagarPagas>(context, $"SELECT * FROM dbo.PagarPagas('{dataInício.ToString("yyyyMMdd")}','{dataFim.ToString("yyyyMMdd")}' )");
        }

        public static void FiltrarPorFornecedor(List<ItemPagarPagas> listaBase, List<int> Fornecedor)
        {
            listaBase.RemoveAll(x => !Fornecedor.Contains(x.IdFornecedor));
        }
    }


}
