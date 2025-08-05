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
    public class ProfissionaisDocumentosBo : IRequiresSessionState
    {

        public static Int32 Inserir(ProfissionaisDocumentos profissionaisdocumentos)
        {
            using (var context = new UranusEntities())
            {
                context.ProfissionaisDocumentos.Add(profissionaisdocumentos);
                context.SaveChanges();

                return profissionaisdocumentos.Id;
            }
        }


        public static void Salvar(ProfissionaisDocumentos profissionaisdocumentos)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(profissionaisdocumentos).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var profissionaisdocumentos = context.ProfissionaisDocumentos.Find(id);
                    context.ProfissionaisDocumentos.Attach(profissionaisdocumentos);
                    context.ProfissionaisDocumentos.Remove(profissionaisdocumentos);
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

        public static List<ProfissionaisDocumentos> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.ProfissionaisDocumentos.Include("Profissionais")
                                                                        .Include("Profissionais.Pessoas")
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.ProfissionaisDocumentos.Include("Profissionais")
                                                                         .Include("Profissionais.Pessoas")
                                where d.Profissionais.Pessoas.Nome.Contains(search)
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static List<ProfissionaisDocumentos> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisDocumentos
                            orderby d.Profissionais.Pessoas.Nome ascending
                            select d;

                return query.ToList();
            }
        }


        public static ProfissionaisDocumentos Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisDocumentos.Include("Profissionais")
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
                var query = from d in context.ProfissionaisDocumentos.Include("Profissionais")
                                                                     .Include("Profissionais.Pessoas")
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                IdProfissional = d.IdProfissional,
                                Nome = d.Profissionais.Pessoas.Nome,
                                TipoDocumento = d.TipoDocumento,
                                NumeroDocumento = d.NumeroDocumento,
                                DataEmissao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataEmissao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataEmissao), 2) + "/" + SqlFunctions.DatePart("year", d.DataEmissao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataEmissao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataEmissao), 2),
                                DataValidade = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataValidade), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataValidade), 2) + "/" + SqlFunctions.DatePart("year", d.DataValidade).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataValidade), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataValidade), 2),
                                Observacao = d.Observacao,
                                DataCadastro = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2),
                                UsuarioCadastro = d.UsuarioCadastro,
                                DataAlteracao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2),
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
                    var query = from d in context.ProfissionaisDocumentos.Include("Profissionais")
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
                    var query = from d in context.ProfissionaisDocumentos.Include("Profissionais")
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

        public static ProfissionaisDocumentos Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisDocumentos.Include("Profissionais")
                                                                        .Include("Profissionais.Pessoas")
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<ProfissionaisDocumentos> ListarDocumentos(Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisDocumentos
                            where d.IdProfissional == IdProfissional
                            orderby d.DataValidade descending
                            select d;

                return query.Take(12).ToList();
            }
        }

        public static List<ProfissionaisDocumentos> ListarDocumentosVencidos(Int32 Ano, Int32 Mes)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisDocumentos.Include("Profissionais")
                                                                     .Include("Profissionais.Pessoas")
                            where d.DataValidade.HasValue && d.DataValidade.Value.Year == Ano && d.DataValidade.Value.Month == Mes
                            orderby d.DataValidade
                            select d;
                return query.ToList();
            }
        }

        public static List<DashProfissionaisDocumentos> ListarDashboard()
        {
            using (var context = new UranusEntities())
            {
                var data = DateTime.Now;
                var datafim = new DateTime(data.Year, data.Month, data.Day);
                var datainicio = new DateTime(data.Year, data.Month, data.Day);
                datainicio = datafim.AddDays(-5);
                var query = (from d in context.ProfissionaisDocumentos.Include("Profissionais")
                                                                     .Include("Profissionais.Pessoas")
                             where d.DataCadastro >= datainicio && d.DataCadastro <= datafim
                             orderby d.Profissionais.Pessoas.Nome ascending
                             select new
                             {
                                 Nome = d.Profissionais.Pessoas.Nome,
                                 TipoDocumento = d.TipoDocumento,
                                 NumeroDocumento = d.NumeroDocumento,
                                 DataCadastro = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString(),
                             }).ToList();

                // Mapear para DashProfissionaisSolicitacoes
                var ausencias = query.Select(q => new DashProfissionaisDocumentos
                {
                    Nome = q.Nome,
                    TipoDocumento = q.TipoDocumento,
                    NumeroDocumento = q.NumeroDocumento,
                    DataCadastro = q.DataCadastro
                }).ToList();

                return ausencias;
            }
        }

    }
}