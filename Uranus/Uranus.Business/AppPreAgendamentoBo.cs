using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class AppPreAgendamentoBo : IRequiresSessionState
    {
        public static List<PreAgendamento> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.PreAgendamento.Include("PreCadastro")
                                                            .Include("PreCadastro.Clientes")
                                                            .Include("PreCadastro.Clientes.Pessoas")
                                                            .Include("ProcessosAreas")
                                                            .Include("Profissionais")
                                                            .Include("Profissionais.Pessoas")
                            select d;

                return query.ToList();
            }
        }
    }
}