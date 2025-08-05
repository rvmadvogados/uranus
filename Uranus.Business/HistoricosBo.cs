using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class HistoricosBo : IRequiresSessionState
    {
        public static Int32 Inserir(Historicos historicos)
        {
            using (var context = new UranusEntities())
            {
                context.Historicos.Add(historicos);
                context.SaveChanges();

                return historicos.Id;
            }
        }

        public static void Salvar(Historicos historicos)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(historicos).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var historicos = context.Historicos.Find(id);
                    context.Historicos.Attach(historicos);
                    context.Historicos.Remove(historicos);
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

        public static List<Historicos> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.Historicos
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Historicos
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static Historicos Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Historicos
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Historicos
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Historicos
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome
                            };

                return query.ToArray();
            }
        }
    }
}