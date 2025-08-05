using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Uranus.Data.R9
{
    public class ItemRelatorioAgendaCompromisso : BaseModel
    {
        [Column] public DateTime Data { get; set; }
        [Column] public string Hora { get; set; }
        [Column] public string Assunto { get; set; }
        [Column] public string Cliente { get; set; }
        [Column] public string NumeroProcesso { get; set; }
        [Column] public string NomeProfissional { get; set; }
        [Column] public string NomeSede { get; set; }
        [Column] public int IdProfissional { get; set; }
        [Column] public int IdAssunto { get; set; }
    }

    public class ItemRelatorioAgendaCompromissoAdapter
    {
        public List<ItemRelatorioAgendaCompromisso> Gerar(DateTime? dataInício, DateTime? dataFim, string conexão)
        {
            return RelatorioAgendaCompromisso.Gerar(dataInício, dataFim, conexão);
        }
    }

    public static class RelatorioAgendaCompromisso
    {
        public static List<ItemRelatorioAgendaCompromisso> Gerar(DateTime? dataInício, DateTime? dataFim, string conexão)
        {
            DataContext context = new DataContext(conexão);

            var parameters = new List<SqlParameter>
            {
                { "@dataInicio", dataInício },
                { "@dataFim", dataFim }
            };

            return global::R9.DataBase.Data.BuildAllObjects<ItemRelatorioAgendaCompromisso>(context, $"SELECT * FROM dbo.RelatorioAgendaCompromisso(@dataInicio, @dataFim) order by Data, Hora", parameters);
        }

        public static void FiltrarPorProfissional(List<ItemRelatorioAgendaCompromisso> listaBase, List<int> Profissional)
        {
                listaBase.RemoveAll(x => !Profissional.Contains(x.IdProfissional));
        }
        public static void FiltrarPorAssunto(List<ItemRelatorioAgendaCompromisso> listaBase, List<int> Assunto)
        {
            listaBase.RemoveAll(x => !Assunto.Contains(x.IdAssunto));
        }
        public static void LimparRetorno(List<ItemRelatorioPorSede> listaBase)
        {
                listaBase.Clear();
        }
    }

}
