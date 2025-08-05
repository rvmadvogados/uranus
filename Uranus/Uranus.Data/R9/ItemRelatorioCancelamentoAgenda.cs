using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Uranus.Domain.Entities;

namespace Uranus.Data.R9
{
    public class ItemRelatorioCancelamentoAgenda : BaseModel
    {
        [Column] public DateTime Data { get; set; }
        [Column] public string Hora { get; set; }
        [Column] public int IdCliente { get; set; }
        [Column] public string NomeCliente { get; set; }
        [Column] public int IdProfissional { get; set; }
        [Column] public string NomeProfissional { get; set; }
        [Column] public DateTime DataCancelamento { get; set; }
        [Column] public string Descricao { get; set; }
        [Column] public string Sede { get; set; }
        [Column] public string Assunto { get; set; }
    }
    public class ItemRelatorioCancelamentoAgendaAdapter
    {
        public List<ItemRelatorioCancelamentoAgenda> Gerar(DateTime? dataInício, DateTime? dataFim, DateTime? DataCancelamentoInicial, DateTime? DataCancelamentoFinal, string Tipo)
        {
            return RelatorioCancelamentoAgenda.Gerar(dataInício, dataFim, DataCancelamentoInicial, DataCancelamentoFinal, Tipo);
        }
    }

    public static class RelatorioCancelamentoAgenda
    {
        public static List<ItemRelatorioCancelamentoAgenda> Gerar(DateTime? dataInício, DateTime? dataFim, DateTime? @DataCancelamentoInicial, DateTime? @DataCancelamentoFinal, string conexão)
        {
            DataContext context = new DataContext(conexão);

            return global::R9.DataBase.Data.BuildAllObjects<ItemRelatorioCancelamentoAgenda>(context, $"SELECT * FROM dbo.RelatorioCancelamentoAgenda('{dataInício?.ToString("yyyyMMdd")}','{dataFim?.ToString("yyyyMMdd")}','{@DataCancelamentoInicial?.ToString("yyyyMMdd")}','{@DataCancelamentoFinal?.ToString("yyyyMMdd")}')");   //.ToString("yyyyMMdd")
        }

        public static void FiltrarPorCliente(List<ItemRelatorioCancelamentoAgenda> listaBase, List<int> idsClientes)
        {
            listaBase.RemoveAll(x => !idsClientes.Contains(x.IdCliente));
        }
        public static void FiltrarPorProfissional(List<ItemRelatorioCancelamentoAgenda> listaBase, List<int> idsProfissionais)
        {
            listaBase.RemoveAll(x => !idsProfissionais.Contains(x.IdProfissional));
        }
    }


}
