using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Data.R9
{
    public class ItemRelatorioEventosPorCliente : BaseModel
    {
        [Column] public DateTime Data { get; set; }
        [Column] public int IdEvento { get; set; }
        [Column] public string NomeEvento { get; set; }
        [Column] public int IdUsuario { get; set; }
        [Column] public string NomeProfissional { get; set; }
        [Column] public int IdPessoa { get; set; }
        [Column] public string NomeCliente { get; set; }
        [Column] public int IdProcessoAcao { get; set; }
        [Column] public string NumeroProcesso { get; set; }
        [Column] public int IdProfissional { get; set; }
    }

    public class ItemRelatorioEventosPorClienteAdapter
    {
        public List<ItemRelatorioEventosPorCliente> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            return RelatorioEventosPorCliente.Gerar(dataInício, dataFim, conexão);
        }
    }

    public static class RelatorioEventosPorCliente
    {
        public static List<ItemRelatorioEventosPorCliente> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            DataContext context = new DataContext(conexão);

            return global::R9.DataBase.Data.BuildAllObjects<ItemRelatorioEventosPorCliente>(context, $"SELECT * FROM dbo.RelatorioEventosPorClientes('{dataInício.ToString("yyyyMMdd")}','{dataFim.ToString("yyyyMMdd")}') ORDER BY NomeProfissional, Data");
        }

        public static void FiltrarPorEvento(List<ItemRelatorioEventosPorCliente> listaBase, List<int> tiposEventos)
        {
            listaBase.RemoveAll(x => !tiposEventos.Contains(x.IdEvento));
        }
        public static void FiltrarPorProfissional(List<ItemRelatorioEventosPorCliente> listaBase, List<int> idsProfissionais)
        {
            listaBase.RemoveAll(x => !idsProfissionais.Contains(x.IdProfissional));
        }
    }


}
