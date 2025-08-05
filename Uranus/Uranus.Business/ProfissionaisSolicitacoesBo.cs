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
using Newtonsoft.Json;

namespace Uranus.Business
{
    public class ProfissionaisSolicitacoesBo : IRequiresSessionState
    {

        public static Int32 Inserir(ProfissionaisSolicitacoes profissionaissolicitacoes)
        {
            using (var context = new UranusEntities())
            {
                context.ProfissionaisSolicitacoes.Add(profissionaissolicitacoes);
                context.SaveChanges();

                return profissionaissolicitacoes.Id;
            }
        }


        public static void Salvar(ProfissionaisSolicitacoes profissionaissolicitacoes)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(profissionaissolicitacoes).State = EntityState.Modified;
                context.Entry(profissionaissolicitacoes).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(profissionaissolicitacoes).Property(x => x.UsuarioCadastro).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var profissionaissolicitacoes = context.ProfissionaisSolicitacoes.Find(id);
                    context.ProfissionaisSolicitacoes.Attach(profissionaissolicitacoes);
                    context.ProfissionaisSolicitacoes.Remove(profissionaissolicitacoes);
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

        public static List<ProfissionaisSolicitacoes> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.ProfissionaisSolicitacoes.Include("Profissionais")
                                                                        .Include("Profissionais.Pessoas")
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.ProfissionaisSolicitacoes.Include("Profissionais")
                                                                         .Include("Profissionais.Pessoas")
                                where d.Profissionais.Pessoas.Nome.Contains(search)
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static List<ProfissionaisSolicitacoes> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisSolicitacoes
                            orderby d.Profissionais.Pessoas.Nome ascending
                            select d;

                return query.ToList();
            }
        }

        public static List<DashProfissionaisSolicitacoes> ListarDashboard()
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ProfissionaisSolicitacoes.Include("Profissionais")
                                                                .Include("Profissionais.Pessoas")
                             where d.Status == "Em Análise"
                             orderby d.Profissionais.Pessoas.Nome ascending
                             select new
                             {
                                 Nome = d.Profissionais.Pessoas.Nome,
                                 Saldo = d.Saldo,
                                 Dias = d.Dias,
                                 DataInicio = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataInicio), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataInicio), 2) + "/" + SqlFunctions.DatePart("year", d.DataInicio).ToString(),
                             }).ToList();

                // Mapear para DashProfissionaisSolicitacoes
                var solicitacoes = query.Select(q => new DashProfissionaisSolicitacoes
                {
                    Nome = q.Nome,
                    Saldo = q.Saldo,
                    Dias = q.Dias,
                    DataInicio = q.DataInicio
                }).ToList();

                return solicitacoes;
            }
        }


        public static List<ProfissionaisSolicitacoes> Listar(Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisSolicitacoes
                            where d.IdProfissional == IdProfissional
                            select d;

                return query.ToList();
            }
        }

        public static ProfissionaisSolicitacoes Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisSolicitacoes.Include("Profissionais")
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
                var query = from d in context.ProfissionaisSolicitacoes.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                IdProfissional = d.IdProfissional,
                                Nome = d.Profissionais.Pessoas.Nome,
                                IdContrato = d.IdContrato,
                                DataSolicitacao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataSolicitacao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataSolicitacao), 2) + "/" + SqlFunctions.DatePart("year", d.DataSolicitacao).ToString(),
                                DataAprovacao = (d.DataAprovacao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAprovacao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAprovacao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAprovacao).ToString() : string.Empty),
                                Status = d.Status,
                                DataInicio = (d.DataInicio != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataInicio), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataInicio), 2) + "/" + SqlFunctions.DatePart("year", d.DataInicio).ToString() : string.Empty),
                                Saldo = d.Saldo,
                                Dias = d.Dias,
                                Justificativa = d.Justificativa,
                                IdPeriodoAquisitivo = d.IdPeriodoAquisitivo,
                                SolicitacaoEspecial = d.SolicitacaoEspecial,
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
                    var query = from d in context.ProfissionaisSolicitacoes.Include("Profissionais")
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
                    var query = from d in context.ProfissionaisSolicitacoes.Include("Profissionais")
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

        public static ProfissionaisSolicitacoes Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisSolicitacoes.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<ProfissionaisSolicitacoes> ListarSolicitacoes(Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                    var query = from d in context.ProfissionaisSolicitacoes
                                where d.IdProfissional == IdProfissional
                                orderby d.DataSolicitacao descending
                                select d;

                     return query.Take(12).ToList();
            }
        }

    }
}