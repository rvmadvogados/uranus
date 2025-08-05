using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class FeriadosBo : IRequiresSessionState
    {
        public static Int32 Inserir(Feriados feriado)
        {
            using (var context = new UranusEntities())
            {
                context.Feriados.Add(feriado);
                context.SaveChanges();

                return feriado.ID;
            }
        }

        public static void Salvar(Feriados feriado)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(feriado).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var feriado = context.Feriados.Find(id);
                    context.Feriados.Attach(feriado);
                    context.Feriados.Remove(feriado);
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

        public static List<Feriados> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.Feriados
                                orderby d.Data ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Feriados
                                where d.Descricao.Contains(search)
                                orderby d.Data ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static List<Feriados> Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Feriados
                            orderby d.Data ascending
                            select d;

                return query.ToList();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Feriados
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                Data = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Data), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Data), 2) + "/" + SqlFunctions.DatePart("year", d.Data).ToString() +
                                       " - " +
                                       DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Data), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Data), 2) + "/" + SqlFunctions.DatePart("year", d.Data).ToString(),
                                Nome = d.Descricao
                            };

                return query.ToArray();
            }
        }

        public static string Buscar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Feriados
                            select new
                            {
                                Data = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Data), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Data), 2) + "/" + SqlFunctions.DatePart("year", d.Data).ToString() +
                                       " - " + d.Descricao
                            };

                var feriados = string.Empty;

                foreach (var item in query.ToList())
                {
                    feriados += item.Data + ";";
                }

                return feriados;
            }
        }

        public static string Buscar(DateTime Data)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Feriados
                            where d.Data == Data
                            select new
                            {
                                Data = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Data), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Data), 2) + "/" + SqlFunctions.DatePart("year", d.Data).ToString() +
                                       " - " + d.Descricao
                            };

                return query.FirstOrDefault()?.Data.ToString() ?? string.Empty;
            }
        }
    }
}