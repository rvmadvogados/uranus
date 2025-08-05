using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taylor.Data.R9
{
    public class ItemReceberEmAbertoEmissaoOld : BaseModel
    {
        [Column] public Int32 IdCliente { get; set; }
        [Column] public string NomeCliente { get; set; }
        [Column] public long NumeroDocumento { get; set; }
        [Column] public DateTime DataEmissao { get; set; }
        [Column] public Int32 Parcela { get; set; }
        [Column] public DateTime DataVencimento { get; set; }
        [Column] public Decimal ValorParcela { get; set; }
    }
    public class ItemReceberEmAbertoEmissaoEtiquetasAdapter
    {
        public List<ItemReceberEmAbertoEmissaoOld> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            return ReceberEmAbertoEmissao.Gerar(dataInício, dataFim, conexão);
        }
    }

    public static class ReceberEmAbertoEmissao
    {
        public static List<ItemReceberEmAbertoEmissaoOld> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            DataContext context = new DataContext(conexão);

            return global::R9.DataBase.Data.BuildAllObjects<ItemReceberEmAbertoEmissaoOld>(context, $"SELECT * FROM dbo.ReceberEmAbertoEmissao('{dataInício.ToString("yyyyMMdd")}','{dataFim.ToString("yyyyMMdd")}' )");
        }

        public static void FiltrarPorCliente(List<ItemReceberEmAbertoEmissaoOld> listaBase, List<int> Cliente)
        {
            listaBase.RemoveAll(x => !Cliente.Contains(x.IdCliente));
        }
    }


}
