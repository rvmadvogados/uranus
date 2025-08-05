using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;
using Uranus.Domain.Entities;

namespace Uranus.Business
{
    public class RelatorioClientesIndicacaoBO : IRequiresSessionState
    {
        public static List<RelatorioClientesIndicacao> Gerar(Int32? FiltrarIndicacao, Int32? Profissionais, Int32? Parceiros, Int32? Clientes)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                SqlParameter param1 = new SqlParameter("@TipoIndicacao", FiltrarIndicacao);
                SqlParameter param2 = new SqlParameter("@Profissional", Profissionais);
                SqlParameter param3 = new SqlParameter("@Parceiro", Parceiros);
                SqlParameter param4 = new SqlParameter("@Cliente", Clientes);

                var query = context.Database.SqlQuery<RelatorioClientesIndicacao>($"stpRelatorioClientesIndicacao  @TipoIndicacao, @Profissional, @Parceiro, @Cliente", param1, param2, param3, param4).ToList();

                return query;
            }
        }

        public static void FiltrarPorIndicacao(List<RelatorioClientesIndicacao> listaBase, List<int> Indicacao)
        {
            listaBase.RemoveAll(x => !Indicacao.Contains(x.Id));
        }

        public static void FiltrarPorCliente(List<RelatorioClientesIndicacao> listaBase, string FiltrarCliente)
        {
            listaBase.RemoveAll(x => !FiltrarCliente.Contains(x.NomeIndicacao));
            listaBase.RemoveAll(x => (string.IsNullOrEmpty(x.NomeIndicacao)));
        }
    }
}