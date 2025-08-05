using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class EmailsBo : IRequiresSessionState
    {
        public static Int32 Inserir(Email email)
        {
            using (var context = new UranusEntities())
            {
                context.Email.Add(email);
                context.SaveChanges();

                return email.ID;
            }
        }

        public static void Salvar(Email email)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(email).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var email = context.Email.Find(id);

                    if (email != null)
                    {
                        context.Email.Attach(email);
                        context.Email.Remove(email);
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

        public static List<Email> Listar(Int64 IdPessoa)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Email
                            where d.IDPessoa == IdPessoa
                            orderby d.Principal descending, d.Ativo descending
                            select d;

                return query.ToList();
            }
        }

        public static Email ListarEmail(Int64 IdPessoa)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Email
                            where d.IDPessoa == IdPessoa
                            where d.Principal == true
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Email Buscar(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Email.Include("Pessoas")
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Boolean Validar(Int64 IdPessoa)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Email
                            where d.IDPessoa == IdPessoa
                            where d.Principal
                            select d;

                return query.Any();
            }
        }
    }
}