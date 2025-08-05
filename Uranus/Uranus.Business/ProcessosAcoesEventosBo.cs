using System;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class ProcessosAcoesEventosBo : IRequiresSessionState
    {
        public static Int32 Inserir(ProcessosAcoesEventos acaoEvento)
        {
            using (var context = new UranusEntities())
            {
                context.ProcessosAcoesEventos.Add(acaoEvento);
                context.SaveChanges();

                return acaoEvento.ID;
            }
        }

        public static void Salvar(ProcessosAcoesEventos acaoEvento)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(acaoEvento).State = EntityState.Modified;
                context.Entry(acaoEvento).Property(x => x.Data).IsModified = false;
                context.Entry(acaoEvento).Property(x => x.IdProcessosEventoPendente).IsModified = false;
                context.Entry(acaoEvento).Property(x => x.IdUsuario).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var acaoEvento = context.ProcessosAcoesEventos.Find(id);

                    if (acaoEvento != null)
                    {
                        context.ProcessosAcoesEventos.Attach(acaoEvento);
                        context.ProcessosAcoesEventos.Remove(acaoEvento);
                        context.SaveChanges();
                    }
                }

                return "00";
            }
            catch (Exception ex)
            {
                String error = "99";
                String message = ex.InnerException.ToString();

                if (message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                    error = "98";

                return error;
            }
        }

        public static ProcessosAcoesEventos Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoesEventos.Include("ProcessosAcoes")
                                                                   .Include("ProcessosEventos")
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }
    }
}