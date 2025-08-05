using R9.DataBase;
using System;
using System.Collections.Generic;

namespace Uranus.Data.R9
{
    public class ItemRelatorioAgendaProfissionaisPrazos : BaseModel
    {
        [Column] public DateTime Data { get; set; }
        [Column] public string NomeProfissional { get; set; }
        [Column] public string NomeCliente { get; set; }
        [Column] public string Evento { get; set; }
        [Column] public DateTime? Prazo1 { get; set; }
        [Column] public DateTime? Prazo2 { get; set; }
        [Column] public int IdProfissional { get; set; }
        [Column] public DateTime? CumprimentoPrazo { get; set; }
        [Column] public string NumeroProcesso { get; set; }
    }

    public class ItemRelatorioAgendaProfissionaisPrazosAdapter
    {
        public List<ItemRelatorioAgendaProfissionaisPrazos> Gerar(DateTime? dataprazo1Início, DateTime? dataprazo1Fim, DateTime? dataprazo2Inicio, DateTime? dataprazo2Fim, string conexão)
        {
            return RelatorioAgendaProfissionaisPrazos.Gerar(dataprazo1Início, dataprazo1Fim, dataprazo2Inicio, dataprazo2Fim, conexão);
        }
    }

    public static class RelatorioAgendaProfissionaisPrazos
    {
        public static List<ItemRelatorioAgendaProfissionaisPrazos> Gerar(DateTime? dataprazo1Início, DateTime? dataprazo1Fim, DateTime? dataprazo2Inicio, DateTime? dataprazo2Fim, string conexão)
        {
            DataContext context = new DataContext(conexão);

            return global::R9.DataBase.Data.BuildAllObjects<ItemRelatorioAgendaProfissionaisPrazos>(context, $"SELECT * FROM dbo.RelatorioAgendaProfissionaisPrazos('{dataprazo1Início?.ToString("yyyyMMdd")}','{dataprazo1Fim?.ToString("yyyyMMdd")}','{dataprazo2Inicio?.ToString("yyyyMMdd")}','{dataprazo2Fim?.ToString("yyyyMMdd")}' ) order by Prazo1 ");
        }

        public static void FiltrarPorProfissional(List<ItemRelatorioAgendaProfissionaisPrazos> listaBaseprofissional, List<int> Profissionais)
        {
            listaBaseprofissional.RemoveAll(x => !Profissionais.Contains(x.IdProfissional));
        }

        //public static void FiltrarPorUsuario(List<ItemRelatorioAgendaProfissionaisPrazos> listaBaseusuarrio, List<int> Usuarios)
        //{
        //    listaBaseusuarrio.RemoveAll(x => !Usuarios.Contains(x.IdUsuario));
        //}

        //public static void FiltrarPorEvento(List<ItemRelatorioAgendaProfissionaisPrazos> listaBaseEvento, List<int> Eventos)
        //{
        //    listaBaseEvento.RemoveAll(x => !Eventos.Contains(x.IdEvento));
        //}
    }

}
