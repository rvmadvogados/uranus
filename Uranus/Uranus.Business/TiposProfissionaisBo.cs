using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class TiposProfissionaisBo : IRequiresSessionState
    {
        public static Int32 Inserir(ProfissionaisTipos tipo)
        {
            using (var context = new UranusEntities())
            {
                context.ProfissionaisTipos.Add(tipo);
                context.SaveChanges();

                return tipo.ID;
            }
        }

        public static void Salvar(ProfissionaisTipos tipo)
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
                    var tipo = context.ProfissionaisTipos.Find(id);
                    context.ProfissionaisTipos.Attach(tipo);
                    context.ProfissionaisTipos.Remove(tipo);
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

        public static List<ProfissionaisTipos> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.ProfissionaisTipos
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else {
                    var query = from d in context.ProfissionaisTipos
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static ProfissionaisTipos Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisTipos
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisTipos
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
                var query = from d in context.ProfissionaisTipos
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