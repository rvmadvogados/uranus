using R9.DataBase;
using System;
using System.Collections.Generic;

namespace Uranus.Data.R9
{
    public class ItemRelatorioAgendaNaoCompareceu : BaseModel
    {
        [Column] public DateTime Data { get; set; }
        [Column] public string Hora { get; set; }
        [Column] public string MotivoConsulta { get; set; }
        [Column] public string NomeCliente { get; set; }
        [Column] public string NomeProfissional { get; set; }
        [Column] public string NomeSede { get; set; }
        [Column] public int IdSede { get; set; }
    }
    public class ItemRelatorioAgendaNaoCompareceuAdapter
    {
        public List<ItemRelatorioAgendaNaoCompareceu> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            return RelatorioAgendaNaoCompareceu.Gerar(dataInício, dataFim, conexão);
        }
    }

    public static class RelatorioAgendaNaoCompareceu
    {
        public static List<ItemRelatorioAgendaNaoCompareceu> Gerar(DateTime dataInício, DateTime dataFim, string conexão)
        {
            DataContext context = new DataContext(conexão);

            return global::R9.DataBase.Data.BuildAllObjects<ItemRelatorioAgendaNaoCompareceu>(context, $"SELECT * FROM dbo.RelatorioAgendasNaoCompareceu('{dataInício.ToString("yyyyMMdd")}','{dataFim.ToString("yyyyMMdd")}' )");
        }

        public static void FiltrarPorSede(List<ItemRelatorioAgendaNaoCompareceu> listaBase, List<int> Sedes)
        {
            listaBase.RemoveAll(x => !Sedes.Contains(x.IdSede));
        }
    }
}
