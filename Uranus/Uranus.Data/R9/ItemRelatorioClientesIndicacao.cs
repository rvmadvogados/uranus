using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Data.R9
{
    public class ItemRelatorioClientesIndicacao : BaseModel
    {
        [Column] public Int32 Id { get; set; }
        [Column] public string TipoIndicacao { get; set; }
        [Column] public string NomeCliente { get; set; }
        [Column] public string Indicacao { get; set; }
        [Column] public string NomeIndicacao { get; set; }
        [Column] public string Vinculo { get; set; }

    }
    public class ItemRelatorioClientesIndicacaoAdapter
    {
        public List<ItemRelatorioClientesIndicacao> Gerar(string conexão)
        {
            return RelatorioClientesIndicacao.Gerar(conexão);
        }
    }

    public static class RelatorioClientesIndicacao
    {
        public static List<ItemRelatorioClientesIndicacao> Gerar(string conexão)
        {


            DataContext context = new DataContext(conexão);

            return global::R9.DataBase.Data.BuildAllObjects<ItemRelatorioClientesIndicacao>(context, $"SELECT * FROM dbo.RelatorioClientesIndicacao()");
        }

        public static void FiltrarPorIndicacao(List<ItemRelatorioClientesIndicacao> listaBase, List<int> Indicacao)
        {
            listaBase.RemoveAll(x => !Indicacao.Contains(x.Id));
        }

        public static void FiltrarPorCliente(List<ItemRelatorioClientesIndicacao> listaBase, string FiltrarCliente)
        {
            listaBase.RemoveAll(x => !FiltrarCliente.Contains(x.NomeIndicacao));
            listaBase.RemoveAll(x => (string.IsNullOrEmpty(x.NomeIndicacao)));
        }
    }

}
