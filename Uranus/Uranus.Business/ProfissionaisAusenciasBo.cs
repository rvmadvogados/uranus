using System;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using Uranus.Domain.Entities;

namespace Uranus.Business
{
    public class ProfissionaisAusenciasBo : IRequiresSessionState
    {

        public static Int32 Inserir(ProfissionaisAusencias profissionaisausencias)
        {
            using (var context = new UranusEntities())
            {
                context.ProfissionaisAusencias.Add(profissionaisausencias);
                context.SaveChanges();

                return profissionaisausencias.Id;
            }
        }


        public static void Salvar(ProfissionaisAusencias profissionaisausencias)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(profissionaisausencias).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var profissionaisausencias = context.ProfissionaisAusencias.Find(id);
                    context.ProfissionaisAusencias.Attach(profissionaisausencias);
                    context.ProfissionaisAusencias.Remove(profissionaisausencias);
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

        public static List<ProfissionaisAusencias> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.ProfissionaisAusencias.Include("Profissionais")
                                                                        .Include("Profissionais.Pessoas")
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.ProfissionaisAusencias.Include("Profissionais")
                                                                         .Include("Profissionais.Pessoas")
                                where d.Profissionais.Pessoas.Nome.Contains(search)
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static List<ProfissionaisAusencias> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisAusencias
                            orderby d.Profissionais.Pessoas.Nome ascending
                            select d;

                return query.ToList();
            }
        }


        
        public static List<ProfissionaisAusencias> ListarAusencias(Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisAusencias
                            where d.IdProfissional == IdProfissional
                            orderby d.DataInicio
                            select d;

                return query.ToList();
            }
        }

        public static ProfissionaisAusencias Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisAusencias.Include("Profissionais")
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
                var query = from d in context.ProfissionaisAusencias.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                IdProfissional = d.IdProfissional,
                                Nome = d.Profissionais.Pessoas.Nome,
                                DataInicio = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataInicio), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataInicio), 2) + "/" + SqlFunctions.DatePart("year", d.DataInicio).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataInicio), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataInicio), 2),
                                DataFim = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataFim), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataFim), 2) + "/" + SqlFunctions.DatePart("year", d.DataFim).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataFim), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataFim), 2),
                                Abonado = d.Abonado,
                                MotivoAusencia = d.MotivoAusencia
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
                    var query = from d in context.ProfissionaisAusencias.Include("Profissionais")
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
                    var query = from d in context.ProfissionaisAusencias.Include("Profissionais")
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

        public static ProfissionaisAusencias Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisAusencias.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<DashProfissionaisAusencias> ListarDashboard()
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ProfissionaisAusencias.Include("Profissionais")
                                                                     .Include("Profissionais.Pessoas")
                             orderby d.Profissionais.Pessoas.Nome ascending
                             select new
                             {
                                 Nome = d.Profissionais.Pessoas.Nome,
                                 DataInicio = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataInicio), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataInicio), 2) + "/" + SqlFunctions.DatePart("year", d.DataInicio).ToString(),
                                 DataFim = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataFim), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataFim), 2) + "/" + SqlFunctions.DatePart("year", d.DataFim).ToString(),
                             }).ToList();

                // Mapear para DashProfissionaisSolicitacoes
                var ausencias = query.Select(q => new DashProfissionaisAusencias
                {
                    Nome = q.Nome,
                    DataInicio = q.DataInicio,
                    DataFim = q.DataFim
                }).ToList();

                return ausencias;
            }
        }
    }
}