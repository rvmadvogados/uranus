using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class ParceirosBo : IRequiresSessionState
    {
        public static Int32 Inserir(Parceiros parceiro)
        {
            using (var context = new UranusEntities())
            {
                context.Parceiros.Add(parceiro);
                context.SaveChanges();

                return parceiro.ID;
            }
        }

        public static void Salvar(Parceiros parceiro)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(parceiro).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var parceiro = context.Parceiros.Find(id);
                    context.Parceiros.Attach(parceiro);
                    context.Parceiros.Remove(parceiro);
                    context.SaveChanges();
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

        public static List<Parceiros> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.Parceiros
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Parceiros
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static Parceiros Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Parceiros
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Parceiros.Include("Profissional")
                                                       .Include("Profissional.Pessoas")
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome,
                                IdProfissional = d.IdProfissional,
                                NomeProfissional = d.Profissionais.Pessoas.Nome
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Parceiros
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome
                            };

                return query.ToArray();
            }
        }
    }
}