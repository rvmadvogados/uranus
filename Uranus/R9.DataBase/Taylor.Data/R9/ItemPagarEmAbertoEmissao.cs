using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taylor.Data.R9
{
    public class ItemPagarEmAbertoEmissao : BaseModel
    {
        [Column] public Int32 IdFornecedor { get; set; }
        [Column] public string NomeFornecedor { get; set; }
        [Column] public string NumeroDocumento { get; set; }
        [Column] public DateTime DataEmissao { get; set; }
        [Column] public Int32 Parcela { get; set; }
        [Column] public DateTime DataVencimento { get; set; }
        [Column] public Decimal ValorParcela { get; set; }
    }
    public class ItemPagarEmAbertoEmissaoEtiquetasAdapter
    {
        public List<ItemPagarEmAbertoEmissao> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            return PagarEmAbertoEmissao.Gerar(dataInício, dataFim, conexão);
        }
    }

    public static class PagarEmAbertoEmissao
    {
        public static List<ItemPagarEmAbertoEmissao> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            DataContext context = new DataContext(conexão);

            return global::R9.DataBase.Data.BuildAllObjects<ItemPagarEmAbertoEmissao>(context, $"SELECT * FROM dbo.PagarEmAbertoEmissao('{dataInício.ToString("yyyyMMdd")}','{dataFim.ToString("yyyyMMdd")}' )");
        }
        
        public static void FiltrarPorFornecedor(List<ItemPagarEmAbertoEmissao> listaBase, List<int> Fornecedores)
        {
            listaBase.RemoveAll(x => !Fornecedores.Contains(x.IdFornecedor));
        }
    }


}
