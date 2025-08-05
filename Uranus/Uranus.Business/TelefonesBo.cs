using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class TelefonesBo : IRequiresSessionState
    {
        public static Int32 Inserir(Fones telefone)
        {
            using (var context = new UranusEntities())
            {
                context.Fones.Add(telefone);
                context.SaveChanges();

                return telefone.ID;
            }
        }

        public static void Salvar(Fones telefone)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(telefone).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var telefone = context.Fones.Find(id);

                    if (telefone != null)
                    {
                        context.Fones.Attach(telefone);
                        context.Fones.Remove(telefone);
                        context.SaveChanges();
                    }
                }

                return "00";
            }
            catch (Exception ex)
            {
                String error = "99";

                if (ex.InnerException != null)
                {
                    String message = ex.InnerException.ToString();

                    if (message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                        error = "98";
                }

                return error;
            }
        }

        public static List<Fones> Listar(Int64 IdPessoa)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Fones
                            where d.IDPessoa == IdPessoa
                            orderby d.Principal descending, d.Status ascending
                            select d;

                return query.ToList();
            }
        }

        public static Fones Consultar(Int32 IdPessoa)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Fones
                            where d.IDPessoa == IdPessoa
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Fones Consultar(String Telefone)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Fones
                            where d.Numero == Telefone
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Fones Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Fones.Include("Pessoas")
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Boolean Validar(Int64 IdPessoa)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Fones
                            where d.IDPessoa == IdPessoa
                            where d.Principal
                            select d;

                return query.Any();
            }
        }

    }
}