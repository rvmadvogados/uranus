using System;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;

namespace Uranus.Business
{
    public class ProfissionaisPeriodosAquisitivoBo : IRequiresSessionState
    {

        public static Int32 Inserir(ProfissionaisPeriodosAquisitivo profissionaisperiodosaquisitivo)
        {
            using (var context = new UranusEntities())
            {
                context.ProfissionaisPeriodosAquisitivo.Add(profissionaisperiodosaquisitivo);
                context.SaveChanges();

                return profissionaisperiodosaquisitivo.Id;
            }
        }


        public static void Salvar(ProfissionaisPeriodosAquisitivo profissionaisperiodosaquisitivo)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(profissionaisperiodosaquisitivo).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var profissionaisperiodosaquisitivo = context.ProfissionaisPeriodosAquisitivo.Find(id);
                    context.ProfissionaisPeriodosAquisitivo.Attach(profissionaisperiodosaquisitivo);
                    context.ProfissionaisPeriodosAquisitivo.Remove(profissionaisperiodosaquisitivo);
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

        public static List<ProfissionaisPeriodosAquisitivo> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.ProfissionaisPeriodosAquisitivo.Include("Profissionais")
                                                                        .Include("Profissionais.Pessoas")
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.ProfissionaisPeriodosAquisitivo.Include("Profissionais")
                                                                         .Include("Profissionais.Pessoas")
                                where d.Profissionais.Pessoas.Nome.Contains(search)
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static List<ProfissionaisPeriodosAquisitivo> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisPeriodosAquisitivo.Include("Profissionais")
                                                                             .Include("Profissionais.Pessoas")
                            orderby d.Profissionais.Pessoas.Nome ascending
                            select d;

                return query.ToList();
            }
        }

        public static List<ProfissionaisPeriodosAquisitivo> ListarFerias(Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisPeriodosAquisitivo
                            where d.IdProfissional == IdProfissional
                            select d;

                return query.ToList();
            }
        }

        public static ProfissionaisPeriodosAquisitivo Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisPeriodosAquisitivo.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisPeriodosAquisitivo.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                IdProfissional = d.IdProfissional,
                                IdContrato = d.IdContrato,
                                Nome = d.Profissionais.Pessoas.Nome,
                                DataInicio = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataInicio), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataInicio), 2) + "/" + SqlFunctions.DatePart("year", d.DataInicio).ToString(),
                                DataFim = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataFim), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataFim), 2) + "/" + SqlFunctions.DatePart("year", d.DataFim).ToString(),
                                DiasGozados = d.DiasGozados,
                            };

                return query.ToArray();
            }
        }

        public static Array ListarArray(String Data)
        {
            if (!String.IsNullOrEmpty(Data) && Data.Length == 10)
            {
                var date = DateTime.Parse(Data);

                using (var context = new UranusEntities())
                {
                    var query = from d in context.ProfissionaisPeriodosAquisitivo.Include("Profissionais")
                                                                        .Include("Profissionais.Pessoas")
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select new
                                {
                                    Id = d.Id,
                                    Nome = d.Profissionais.Pessoas.Nome,
                                };

                    return query.ToArray();
                }
            }
            else
            {
                using (var context = new UranusEntities())
                {
                    var query = from d in context.ProfissionaisPeriodosAquisitivo.Include("Profissionais")
                                                                        .Include("Profissionais.Pessoas")
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select new
                                {
                                    Id = d.Id,
                                    Nome = d.Profissionais.Pessoas.Nome,
                                };

                    return query.ToArray();
                }
            }
        }

        public static ProfissionaisPeriodosAquisitivo Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisPeriodosAquisitivo.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static ProfissionaisPeriodosAquisitivo BuscarPeriodo(Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisPeriodosAquisitivo.Include("Profissionais")
                                                                             .Include("Profissionais.Pessoas")
                            where d.IdProfissional == d.IdProfissional
                            where d.DiasGozados < d.Dias
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<ProfissionaisPeriodosAquisitivo> ListarPeriodoVencido(DateTime Data)
        {
            using (var context = new UranusEntities())
            {

                var query = from d in context.ProfissionaisPeriodosAquisitivo
                            where d.DataFim == Data
                            orderby d.Id ascending
                            select d;

                return query.ToList();
            }
        }

    }
}