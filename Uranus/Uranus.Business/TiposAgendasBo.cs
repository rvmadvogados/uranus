using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class TiposAgendasBo : IRequiresSessionState
    {
        public static Int32 Inserir(AgendasTipos tipo)
        {
            using (var context = new UranusEntities())
            {
                context.AgendasTipos.Add(tipo);
                context.SaveChanges();

                return tipo.ID;
            }
        }

        public static void Salvar(AgendasTipos tipo)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(tipo).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var tipo = context.AgendasTipos.Find(id);
                    context.AgendasTipos.Attach(tipo);
                    context.AgendasTipos.Remove(tipo);
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

        public static List<AgendasTipos> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.AgendasTipos
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.AgendasTipos
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static AgendasTipos Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.AgendasTipos
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.AgendasTipos
                            where d.ID == Id
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.AgendasTipos
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