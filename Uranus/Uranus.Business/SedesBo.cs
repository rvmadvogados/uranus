using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class SedesBo : IRequiresSessionState
    {
        public static Int32 Inserir(Sedes sede)
        {
            using (var context = new UranusEntities())
            {
                context.Sedes.Add(sede);
                context.SaveChanges();

                return sede.ID;
            }
        }

        public static void Salvar(Sedes sede)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(sede).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var sede = context.Sedes.Find(id);
                    context.Sedes.Attach(sede);
                    context.Sedes.Remove(sede);
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

        public static List<Sedes> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.Sedes
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Sedes
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static Sedes Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Sedes
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }
        
        public static Sedes ConsultarSede(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Sedes
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Sedes
                            where d.ID == Id
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome,
                                Agenda = d.Agenda
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Sedes
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome,
                                Agenda = d.Agenda
                            };

                return query.ToArray();
            }
        }
        public static Array ConsultarClientes()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Sedes
                            where d.Agenda == true
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome,
                                Agenda = d.Agenda
                            };

                return query.ToArray();
            }
        }
    }
}