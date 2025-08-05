using System;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;

namespace Uranus.Business
{
    public class ProfissionaisFeriasBo : IRequiresSessionState
    {

        public static Int32 Inserir(ProfissionaisFerias profissionaisferias)
        {
            using (var context = new UranusEntities())
            {
                context.ProfissionaisFerias.Add(profissionaisferias);
                context.SaveChanges();

                return profissionaisferias.Id;
            }
        }


        public static void Salvar(ProfissionaisFerias profissionaisferias)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(profissionaisferias).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var profissionaisferias = context.ProfissionaisFerias.Find(id);
                    context.ProfissionaisFerias.Attach(profissionaisferias);
                    context.ProfissionaisFerias.Remove(profissionaisferias);
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

        public static List<ProfissionaisFerias> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.ProfissionaisFerias.Include("Profissionais")
                                                                        .Include("Profissionais.Pessoas")
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.ProfissionaisFerias.Include("Profissionais")
                                                                         .Include("Profissionais.Pessoas")
                                where d.Profissionais.Pessoas.Nome.Contains(search)
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static List<ProfissionaisFerias> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisFerias
                            orderby d.Profissionais.Pessoas.Nome ascending
                            select d;

                return query.ToList();
            }
        }


        
        public static List<ProfissionaisFerias> ListarFerias(Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisFerias
                            where d.IdProfissional == IdProfissional
                            select d;

                return query.ToList();
            }
        }

        public static ProfissionaisFerias Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisFerias.Include("Profissionais")
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
                var query = from d in context.ProfissionaisFerias.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                IdProfissional = d.IdProfissional,
                                IdContrato = d.IdContrato,
                                Nome = d.Profissionais.Pessoas.Nome,
                                Tipo = d.Tipo,
                                DataInicio = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataInicio), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataInicio), 2) + "/" + SqlFunctions.DatePart("year", d.DataInicio).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataInicio), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataInicio), 2),
                                DataFim = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataFim), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataFim), 2) + "/" + SqlFunctions.DatePart("year", d.DataFim).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataFim), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataFim), 2),
                                DiasGozados = d.DiasGozados,
                                Saldo = d.Saldo,
                                PeriodoAquisitivoInicio = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.PeriodoAquisitivoInicio), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.PeriodoAquisitivoInicio), 2) + "/" + SqlFunctions.DatePart("year", d.PeriodoAquisitivoInicio).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.PeriodoAquisitivoInicio), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.PeriodoAquisitivoInicio), 2),
                                PeriodoAquisitivoFim = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.PeriodoAquisitivoFim), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.PeriodoAquisitivoFim), 2) + "/" + SqlFunctions.DatePart("year", d.PeriodoAquisitivoFim).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.PeriodoAquisitivoFim), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.PeriodoAquisitivoFim), 2),
                                DataCadastro = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString(),
                                UsuarioCadastro = d.UsuarioCadastro,
                                DataAlteracao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString(),
                                UsuarioAlteracao = d.UsuarioAlteracao
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
                    var query = from d in context.ProfissionaisFerias.Include("Profissionais")
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
                    var query = from d in context.ProfissionaisFerias.Include("Profissionais")
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

        public static ProfissionaisFerias Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisFerias.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            select d;

                return query.FirstOrDefault();
            }
        }

    }
}