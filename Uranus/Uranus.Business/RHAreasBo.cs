using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class RHAreasBo : IRequiresSessionState
    {
        public static Int32 Inserir(RHAreas area)
        {
            using (var context = new UranusEntities())
            {
                context.RHAreas.Add(area);
                context.SaveChanges();

                return area.Id;
            }
        }

        public static void Salvar(RHAreas area)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(area).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var area = context.RHAreas.Find(id);
                    context.RHAreas.Attach(area);
                    context.RHAreas.Remove(area);
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

        public static List<RHAreas> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.RHAreas
                                orderby d.Nome ascending
                                select d;

                    return query.Distinct().ToList();
                }
                else
                {
                    var query = from d in context.RHAreas
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.Distinct().ToList();
                }
            }
        }

        public static RHAreas Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.RHAreas
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.RHAreas
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
                var query = from d in context.RHAreas
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome
                            };

                return query.ToArray();
            }
        }

        public static RHAreas ConsultarNome(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.RHAreas
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

    }
}