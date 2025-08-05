using R9.DataBase;
using System;
using System.Collections.Generic;

namespace Uranus.Data.R9
{
    public class ItemRelatorioAgendasProfissionais : BaseModel
    {
        [Column] public DateTime Data { get; set; }
        [Column] public string NomeProfissional { get; set; }
        [Column] public string NomeCliente { get; set; }
        [Column] public string Evento { get; set; }
        [Column] public DateTime? Prazo1 { get; set; }
        [Column] public DateTime? Prazo2 { get; set; }
        [Column] public int IdProfissional { get; set; }
        [Column] public int IdEvento { get; set; }
        [Column] public DateTime? CumprimentoPrazo { get; set; }
        [Column] public string NumeroProcesso { get; set; }
        [Column] public int IdUsuario { get; set; }
    }

    public class ItemRelatorioAgendasProfissionaisAdapter
    {
        public List<ItemRelatorioAgendasProfissionais> Gerar(DateTime? dataInício, DateTime? dataFim, DateTime? cumprimetoInicio, DateTime? cumprimetoFim, string conexão)
        {
            return RelatorioAgendasProfissionais.Gerar(dataInício, dataFim, cumprimetoInicio, cumprimetoFim, conexão);
        }
    }

    public static class RelatorioAgendasProfissionais
    {
        public static List<ItemRelatorioAgendasProfissionais> Gerar(DateTime? dataInício, DateTime? dataFim, DateTime? CumprimentoInicio, DateTime? CumprimentoFim, string conexão)
        {
            DataContext context = new DataContext(conexão);

            return global::R9.DataBase.Data.BuildAllObjects<ItemRelatorioAgendasProfissionais>(context, $"SELECT * FROM dbo.RelatorioAgendaProfissionais('{dataInício?.ToString("yyyyMMdd")}','{dataFim?.ToString("yyyyMMdd")}','{CumprimentoInicio?.ToString("yyyyMMdd")}','{CumprimentoFim?.ToString("yyyyMMdd")}' ) order by Prazo1");
            //            return global::R9.DataBase.Data.BuildAllObjects<ItemRelatorioAgendasProfissionais>(context, $"SELECT * FROM dbo.RelatorioAgendaProfissionais('{dataInício?.ToString("yyyyMMdd")}','{dataFim?.ToString("yyyyMMdd")}','{CumprimentoInicio?.ToString("yyyyMMdd")}','{CumprimentoFim?.ToString("yyyyMMdd")}' )");
        }

        public static void FiltrarPorProfissional(List<ItemRelatorioAgendasProfissionais> listaBaseprofissional, List<int> Profissionais)
        {
            listaBaseprofissional.RemoveAll(x => !Profissionais.Contains(x.IdProfissional));
        }

        public static void FiltrarPorUsuario(List<ItemRelatorioAgendasProfissionais> listaBaseusuarrio, List<int> Usuarios)
        {
            listaBaseusuarrio.RemoveAll(x => !Usuarios.Contains(x.IdUsuario));
        }

        public static void FiltrarPorEvento(List<ItemRelatorioAgendasProfissionais> listaBaseEvento, List<int> Eventos)
        {
            listaBaseEvento.RemoveAll(x => !Eventos.Contains(x.IdEvento));
        }
    }

}
