using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.SessionState;
using System.Xml;
using System.Xml.Serialization;
using Uranus.Data;
using Uranus.Domain;
//using Uranus.NFE;

namespace Uranus.Business
{
    public class NotasServicosBO : IRequiresSessionState
    {
        public static void Salvar(NotasServicos notas)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(notas).State = EntityState.Modified;
                context.Entry(notas).Property(x => x.Ano).IsModified = false;
                context.Entry(notas).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(notas).Property(x => x.NomeUsuarioCadastro).IsModified = false;
                //context.Entry(notas).Property(x => x.IdReceita).IsModified = false;
                context.SaveChanges();
            }
        }

        public static void SalvarParcelas(NotasServicosParcelas notasparcelas)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(notasparcelas).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static List<FinanceiroReceitas> Listar(string action, string search)
        {
            using (var context = new UranusEntities())
            {

                var query = context.FinanceiroReceitas
                                                    //.Include("Status")
                                                    .Include("Clientes")
                                                    .Include("Clientes.Pessoas")
                                                    .Include("Clientes.Pessoas.Fones")
                                                    .Include("Clientes.Pessoas.Email")
                                                    .Include("NotasServicos1")
                                                    .Include("NotasServicos1.Status")
                                                    .AsQueryable();

                //query = query.Where(x => x.NotasServicos.Status.TipoAcao == action);

                //if (!String.IsNullOrEmpty(search.Trim()))
                //{
                //    long numero = long.Parse(search);
                //    query = query.Where(x => x.Id == numero);
                //}

                query = query.AsQueryable().OrderByDescending(x => x.DataDocumento);


                var xxx = query.Take(500).ToList();

                return query.Take(500).ToList();
            }
        }

        public static NotasServicos Consultar(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.NotasServicos.Include("NotasServicosParcelas")
                                                          .Include("Status")
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static NotasServicos ConsultarUltimaNota(Int32 IdEmpresa)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.NotasServicos
                            where d.IdEmpresa == IdEmpresa
                            orderby d.NumeroNota descending
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static NotasServicos ConsultarUltimaReferencia(Int32 IdEmpresa)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.NotasServicos
                            where d.IdEmpresa == IdEmpresa
                            orderby d.NumeroReferencia descending
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static NotasServicos ConsultarNFS(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                var query = from d in context.NotasServicos
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.NotasServicos.Include("Clientes")
                             where d.Id == Id
                             select new
                             {
                                 IdEmpresa = d.IdEmpresa,
                                 Ano = d.Ano,
                                 NumeroNota = d.NumeroNota,
                                 IdReceita = d.IdReceita,
                                 NumeroDocumento = d.NumeroDocumento,
                                 Data = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Data), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Data), 2) + "/" + SqlFunctions.DatePart("year", d.Data).ToString(),
                                 IdStatus = d.IdStatus,
                                 IdCliente = d.IdCliente,
                                 Cliente = d.Clientes.Pessoas.Nome,
                                 IdClienteEmail = d.IdClienteEmail,
                                 DescricaoServico = d.DescricaoServico,
                                 ValorServico = d.ValorServico,
                                 ValorLiquido = d.ValorLiquido,
                                 PercentualIssqn = d.PercentualIssqn,
                                 ValorIssqn = d.ValorIssqn,
                                 PercentualIR = d.PercentualIR,
                                 ValorIR = d.ValorIR,
                                 PercentualCofins = d.PercentualCofins,
                                 ValorCofins = d.ValorCofins,
                                 PercentualPis = d.PercentualPis,
                                 ValorPis = d.ValorPis,
                                 PercentualCSLL = d.PercentualCSLL,
                                 ValorCSLL = d.ValorCSLL,
                                 Plano = d.Plano,
                                 IdFormaPagamento = d.IdFormaPagamento,
                                 Observacao = d.Observacao,
                                 ReterIssqn = d.ReterIssqn,
                                 NumeroReferencia = d.NumeroReferencia,
                             }).ToList()
                            .Select(x => new
                            {
                                IdEmpresa = x.IdEmpresa,
                                Ano = x.Ano,
                                NumeroNota = x.NumeroNota,
                                IdReceita = x.IdReceita,
                                NumeroDocumento = x.NumeroDocumento,
                                Data = x.Data,
                                IdStatus = x.IdStatus,
                                IdCliente = x.IdCliente,
                                Cliente = x.Cliente,
                                IdClienteEmail = x.IdClienteEmail,
                                DescricaoServico = x.DescricaoServico.Replace("\\r\\n", "&#10;"),
                                ValorServico = (x.ValorServico?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                ValorLiquido = (x.ValorLiquido?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                PercentualIssqn = (x.PercentualIssqn?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                ValorIssqn = (x.ValorIssqn?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                PercentualIR = (x.PercentualIR?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                ValorIR = (x.ValorIR?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                PercentualCofins = (x.PercentualCofins?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                ValorCofins = (x.ValorCofins?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                PercentualPis = (x.PercentualPis?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                ValorPis = (x.ValorPis?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                PercentualCSLL = (x.PercentualCSLL?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                ValorCSLL = (x.ValorCSLL?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                Plano = x.Plano,
                                IdFormaPagamento = x.IdFormaPagamento,
                                Observacao = x.Observacao,
                                ReterIssqn = x.ReterIssqn,
                                NumeroReferencia = x.NumeroReferencia,
                            });

                return query.ToArray();
            }
        }

        public static void AlterarClientesNotasServicos(Int64 IdNota)
        {
            using (var context = new UranusEntities())
            {
                SqlParameter param1 = new SqlParameter("@IdNota", IdNota);

                context.Database.ExecuteSqlCommand("stpAlteraClientesNotasServicos @IdNota", param1);

            }
        }

        public static void AlteraClientesNotasServicos(Int64 IdNota)
        {
            using (var context = new UranusEntities())
            {
                SqlParameter param1 = new SqlParameter("@IdNota", IdNota);

                context.Database.ExecuteSqlCommand("stpAlteraClientesNotasServicos @IdNota", param1);

            }
        }

        public static List<FinanceiroReceitas> ListarReceitas(string action, string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.FinanceiroReceitas
                                                                  .Include("Clientes")
                                                                  .Include("Clientes.Pessoas")
                                where d.Nota == false
                                orderby d.DataDocumento ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.FinanceiroReceitas
                                                                  .Include("Clientes")
                                                                  .Include("Clientes.Pessoas")
                                where d.Nota == false
                                where d.NumeroDocumento.ToString().Contains(search)
                                orderby d.DataDocumento ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static List<NotasServicosParcelas> ListarParcelas(Int64 IdNotaFiscal)
        {
            using (var context = new UranusEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                var query = from d in context.NotasServicosParcelas.Include("NotasServicos")
                            where d.IdNotaServico == IdNotaFiscal
                            select d;

                return query.ToList();
            }
        }

        public static List<Clientes> ListarNotasServicos(string NumeroNota, string CPFCNPJ, string Nome)
        {
            using (var context = new UranusEntities())
            {
                var query = context.Clientes
                                   .Include("Pessoas")
                                   .Include("Pessoas.Fones")
                                   .Include("NotasServicos")
                                   .Include("NotasServicos.Status")
                                   .AsQueryable();

                if (NumeroNota == "")
                {
                    query = query.Where(x => x.Pessoas.CpfCnpj.Contains(CPFCNPJ));
                    query = query.Where(x => x.Pessoas.Nome.Contains(Nome));
                    query = query.Where(x => x.NotasServicos.Count > 0);
                }
                else
                {
                    query = query.Where(x => x.NotasServicos.Where(w => w.IdCliente == x.ID).FirstOrDefault().NumeroNota.ToString() == NumeroNota);
                }

                query = query.AsQueryable().OrderBy(x => x.Pessoas.Nome);

                return query.Take(500).ToList();
            }
        }

        public static void SalvarParcelasNotasServicos(Int64 IdNota)
        {
            using (var context = new UranusEntities())
            {
                SqlParameter param1 = new SqlParameter("@IdNota", IdNota);

                context.Database.ExecuteSqlCommand("stpSalvarParcelasNotasServicos @IdNota", param1);

            }
        }

    }
}