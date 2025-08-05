using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taylor.Data.R9
{
    public class ItemReceberEmAbertoVencimento : BaseModel
    {
        [Column] public Int32 IdCliente { get; set; }
        [Column] public string NomeCliente { get; set; }
        [Column] public long NumeroDocumento { get; set; }
        [Column] public DateTime DataEmissao { get; set; }
        [Column] public Int32 Parcela { get; set; }
        [Column] public DateTime DataVencimento { get; set; }
        [Column] public Decimal ValorParcela { get; set; }
    }
    public class ItemReceberEmAbertoVencimentoAdapter
    {
        public List<ItemReceberEmAbertoVencimento> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            return ReceberEmAbertoVencimento.Gerar(dataInício, dataFim, conexão);
        }
    }

    public static class ReceberEmAbertoVencimento
    {
        public static List<ItemReceberEmAbertoVencimento> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            DataContext context = new DataContext(conexão);

            return global::R9.DataBase.Data.BuildAllObjects<ItemReceberEmAbertoVencimento>(context, $"SELECT * FROM dbo.ReceberEmAbertoVencimento('{dataInício.ToString("yyyyMMdd")}','{dataFim.ToString("yyyyMMdd")}' )");
        }

        public static void FiltrarPorCliente(List<ItemReceberEmAbertoVencimento> listaBase, List<int> Cliente)
        {
            listaBase.RemoveAll(x => !Cliente.Contains(x.IdCliente));
        }
    }
}
