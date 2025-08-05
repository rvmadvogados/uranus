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
    public class ProfissionaisSolicitacoesEspeciaisBo : IRequiresSessionState
    {

        public static Int64 Inserir(ProfissionaisSolicitacoesEspeciais profissionaissolicitacoesespeciais)
        {
            using (var context = new UranusEntities())
            {
                context.ProfissionaisSolicitacoesEspeciais.Add(profissionaissolicitacoesespeciais);
                context.SaveChanges();

                return profissionaissolicitacoesespeciais.Id;
            }
        }


        public static void Salvar(ProfissionaisSolicitacoesEspeciais profissionaissolicitacoesespeciais)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(profissionaissolicitacoesespeciais).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int64 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var profissionaissolicitacoesespeciais = context.ProfissionaisSolicitacoesEspeciais.Find(id);
                    context.ProfissionaisSolicitacoesEspeciais.Attach(profissionaissolicitacoesespeciais);
                    context.ProfissionaisSolicitacoesEspeciais.Remove(profissionaissolicitacoesespeciais);
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

        public static List<ProfissionaisSolicitacoesEspeciais> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.ProfissionaisSolicitacoesEspeciais.Include("Profissionais")
                                                                        .Include("Profissionais.Pessoas")
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.ProfissionaisSolicitacoesEspeciais.Include("Profissionais")
                                                                         .Include("Profissionais.Pessoas")
                                where d.Profissionais.Pessoas.Nome.Contains(search)
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static List<ProfissionaisSolicitacoesEspeciais> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisSolicitacoesEspeciais
                            orderby d.Profissionais.Pessoas.Nome ascending
                            select d;

                return query.ToList();
            }
        }


        
        public static List<ProfissionaisSolicitacoesEspeciais> ListarSolicitacoesEspeciais(Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisSolicitacoesEspeciais
                            where d.IdProfissional == IdProfissional
                            select d;

                return query.ToList();
            }
        }

        public static ProfissionaisSolicitacoesEspeciais Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisSolicitacoesEspeciais.Include("Profissionais")
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
                var query = from d in context.ProfissionaisSolicitacoesEspeciais.Include("Profissionais")
                                                                                .Include("Profissionais.Pessoas")
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                IdProfissional = d.IdProfissional,
                                Nome = d.Profissionais.Pessoas.Nome,
                                Data = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Data), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Data), 2) + "/" + SqlFunctions.DatePart("year", d.Data).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.Data), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.Data), 2),
                                DataAprovacao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAprovacao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAprovacao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAprovacao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAprovacao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAprovacao), 2),
                                Solicitacao = d.Solicitacao,
                                Status = d.Status,
                                Justificativa = d.Justificativa
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
                    var query = from d in context.ProfissionaisSolicitacoesEspeciais.Include("Profissionais")
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
                    var query = from d in context.ProfissionaisSolicitacoesEspeciais.Include("Profissionais")
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

        public static ProfissionaisSolicitacoesEspeciais Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisSolicitacoesEspeciais.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<DashProfissionaisSolicitacoesEspeciais> ListarDashboard()
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ProfissionaisSolicitacoesEspeciais.Include("Profissionais")
                                                                                 .Include("Profissionais.Pessoas")
                             where d.Status == "Em Análise"
                             orderby d.Profissionais.Pessoas.Nome ascending
                             select new
                             {
                                 Nome = d.Profissionais.Pessoas.Nome,
                                 Data = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Data), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Data), 2) + "/" + SqlFunctions.DatePart("year", d.Data).ToString(),
                                 Solicitacao = d.Solicitacao,
                             }).ToList();

                // Mapear para DashProfissionaisSolicitacoes
                var solicitacoesespeciais = query.Select(q => new DashProfissionaisSolicitacoesEspeciais
                {
                    Nome = q.Nome,
                    Data = q.Data,
                    Solicitacao = q.Solicitacao
                }).ToList();

                return solicitacoesespeciais;
            }
        }

    }
}