using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taylor.Data.R9
{
    public class ItemReceberPagas : BaseModel
    {
        [Column] public Int32 IdCliente { get; set; }
        [Column] public string NomeCliente { get; set; }
        [Column] public long NumeroDocumento { get; set; }
        [Column] public DateTime DataEmissao { get; set; }
        [Column] public Int32 Parcela { get; set; }
        [Column] public DateTime DataVencimento { get; set; }
        [Column] public Decimal ValorParcela { get; set; }
        [Column] public DateTime DataPagamento { get; set; }
        [Column] public Decimal Juros { get; set; }
        [Column] public Decimal Descontos { get; set; }
        [Column] public Decimal ValorPago { get; set; }
    }
    public class ItemReceberPagasAdapter
    {
        public List<ItemReceberPagas> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            return ReceberPagas.Gerar(dataInício, dataFim, conexão);
        }
    }

    public static class ReceberPagas
    {
        public static List<ItemReceberPagas> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            DataContext context = new DataContext(conexão);

            return global::R9.DataBase.Data.BuildAllObjects<ItemReceberPagas>(context, $"SELECT * FROM dbo.ReceberPagas('{dataInício.ToString("yyyyMMdd")}','{dataFim.ToString("yyyyMMdd")}' )");
        }

        public static void FiltrarPorCliente(List<ItemReceberPagas> listaBase, List<int> Cliente)
        {
            listaBase.RemoveAll(x => !Cliente.Contains(x.IdCliente));
        }
    }


}
