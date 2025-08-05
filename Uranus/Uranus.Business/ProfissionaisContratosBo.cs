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
    public class ProfissionaisContratosBo : IRequiresSessionState
    {

        public static Int32 Inserir(ProfissionaisContratos profissionaiscontratos)
        {
            using (var context = new UranusEntities())
            {
                context.ProfissionaisContratos.Add(profissionaiscontratos);
                context.SaveChanges();

                return profissionaiscontratos.Id;
            }
        }


        public static void Salvar(ProfissionaisContratos profissionaiscontratos)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(profissionaiscontratos).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var profissionaiscontratos = context.ProfissionaisContratos.Find(id);
                    context.ProfissionaisContratos.Attach(profissionaiscontratos);
                    context.ProfissionaisContratos.Remove(profissionaiscontratos);
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

        public static List<ProfissionaisContratos> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.ProfissionaisContratos.Include("Profissionais")
                                                                        .Include("Profissionais.Pessoas")
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.ProfissionaisContratos.Include("Profissionais")
                                                                         .Include("Profissionais.Pessoas")
                                where d.Profissionais.Pessoas.Nome.Contains(search)
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static List<ProfissionaisContratos> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisContratos
                            orderby d.Profissionais.Pessoas.Nome ascending
                            select d;

                return query.ToList();
            }
        }


        
        public static List<ProfissionaisContratos> ListarContratos(Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisContratos.Include("Profissionais")
                                                                    .Include("Profissionais.Sedes")
                                                                    .Include("Sedes")
                            where d.IdProfissional == IdProfissional
                            select d;

                return query.ToList();
            }
        }

        public static ProfissionaisContratos Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisContratos.Include("Profissionais")
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
                var query = from d in context.ProfissionaisContratos.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                IdProfissional = d.IdProfissional,
                                Nome = d.Profissionais.Pessoas.Nome,
                                DataInicio = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataInicio), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataInicio), 2) + "/" + SqlFunctions.DatePart("year", d.DataInicio).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataInicio), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataInicio), 2),
                                DataFim = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataFim), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataFim), 2) + "/" + SqlFunctions.DatePart("year", d.DataFim).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataFim), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataFim), 2),
                                TipoContrato = d.TipoContrato,
                                IdSede = d.IdSede
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
                    var query = from d in context.ProfissionaisContratos.Include("Profissionais")
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
                    var query = from d in context.ProfissionaisContratos.Include("Profissionais")
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

        public static ProfissionaisContratos Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisContratos.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static ProfissionaisContratos ConsultarContrato()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisContratos
                            where d.DataFim == null
                            select d;

                return query.FirstOrDefault();
            }
        }
    }
}