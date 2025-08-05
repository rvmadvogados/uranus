using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class SalasBo : IRequiresSessionState
    {
        public static Int32 Inserir(Salas sala)
        {
            using (var context = new UranusEntities())
            {
                context.Salas.Add(sala);
                context.SaveChanges();

                return sala.ID;
            }
        }

        public static void Salvar(Salas sala)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(sala).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var sala = context.Salas.Find(id);
                    context.Salas.Attach(sala);
                    context.Salas.Remove(sala);
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

        public static List<Salas> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.Salas.Include("Sedes")
                                orderby d.Sedes.Nome, d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Salas.Include("Sedes")
                                where d.Nome.Contains(search)
                                orderby d.Sedes.Nome, d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static Salas Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Salas
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Salas.Include("Sedes")
                            where d.ID == Id
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome,
                                IDSede = d.Sedes.ID
                            };

                return query.ToArray();
            }
        }

        public static Array ListarArray(Int64 IdSede)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Salas
                            where d.IDSede == IdSede
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