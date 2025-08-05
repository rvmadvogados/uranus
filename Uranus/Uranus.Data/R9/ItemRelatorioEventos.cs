using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Data.R9
{
    public class ItemRelatorioEventos : BaseModel
    {
        [Column] public int IdProfissional { get; set; }
        [Column] public string NomeProfissional { get; set; }
        [Column] public string TipoEvento { get; set; }
        [Column] public int Quantidade { get; set; }
        [Column] public int Cumpridos { get; set; }
        [Column] public int IdTipoEvento { get; set; }
    }

    public class ItemRelatórioEventosAdapter
    {
        public List<ItemRelatorioEventos> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            return RelatórioEventos.Gerar(dataInício, dataFim, conexão);
        }
    }

    public static class RelatórioEventos
    {
        public static List<ItemRelatorioEventos> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            DataContext context = new DataContext(conexão);
            
            return global::R9.DataBase.Data.BuildAllObjects<ItemRelatorioEventos>(context, $"SELECT * FROM dbo.RelatorioEventos('{dataInício.ToString("yyyyMMdd")}','{dataFim.ToString("yyyyMMdd")}') ORDER BY NomeProfissional, TipoEvento ");
        }

        public static void FiltrarPorEvento(List<ItemRelatorioEventos> listaBase, List<int> tiposEventos)
        {
            listaBase.RemoveAll(x => !tiposEventos.Contains(x.IdTipoEvento));
        }
        public static void FiltrarPorProfissional(List<ItemRelatorioEventos> listaBase, List<int> idsProfissionais)
        {
            listaBase.RemoveAll(x => !idsProfissionais.Contains(x.IdProfissional));
        }
    }


}
