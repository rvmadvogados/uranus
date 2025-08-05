using Uranus.Data;
using Uranus.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;

namespace Uranus.Business
{
    public class StatusBO : IRequiresSessionState
    {
        public static Int32 Inserir(Status status)
        {
            using (var context = new UranusEntities())
            {
                context.Status.Add(status);
                context.SaveChanges();

                return status.ID;
            }
        }

        public static void Salvar(Status status)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(status).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var status = context.Status.Find(id);
                    context.Status.Attach(status);
                    context.Status.Remove(status);
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

        public static List<Status> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.Status
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Status
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static Status Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Status
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Status
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome,
                                TipoStatus = d.TipoStatus,
                                TipoAcao = d.TipoAcao,
                                Status = d.Status1
                            };

                return query.ToArray();
            }
        }

        public static Int32 ConsultarId(String Tipo)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Status
                            where d.TipoAcao == Tipo
                            select d;

                return query.FirstOrDefault().ID; 
            }
        }

        public static Array Consultar(String Tipo)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Status
                            where d.TipoStatus == Tipo
                            where d.Status1 == true
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome,
                                TipoStatus = d.TipoStatus,
                                TipoAcao = d.TipoAcao
                            };

                return query.ToArray();
            }
        }

        public static Int32 Buscar(String Tipo)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Status
                            where d.TipoAcao == Tipo
                            select new
                            {
                                Id = d.ID
                            };

                return query.FirstOrDefault().Id;
            }
        }
    }
}