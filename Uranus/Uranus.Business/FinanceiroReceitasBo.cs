using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class FinanceiroReceitasBo : IRequiresSessionState
    {
        public static Int64 Inserir(FinanceiroReceitas financeiroreceitas)
        {
            using (var context = new UranusEntities())
            {
                context.FinanceiroReceitas.Add(financeiroreceitas);
                context.SaveChanges();

                return financeiroreceitas.Id;
            }
        }

        public static void Salvar(FinanceiroReceitas financeiroreceitas)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(financeiroreceitas).State = EntityState.Modified;
                context.Entry(financeiroreceitas).Property(x => x.Nota).IsModified = false;
                context.Entry(financeiroreceitas).Property(x => x.IdNota).IsModified = false;
                context.Entry(financeiroreceitas).Property(x => x.IdReceber).IsModified = false;
                context.Entry(financeiroreceitas).Property(x => x.IdCaixa).IsModified = false;
                context.Entry(financeiroreceitas).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(financeiroreceitas).Property(x => x.NomeUsuarioCadastro).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var financeiroreceitas = context.FinanceiroReceitas.Find(id);
                    context.FinanceiroReceitas.Attach(financeiroreceitas);
                    context.FinanceiroReceitas.Remove(financeiroreceitas);
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

        public static List<FinanceiroReceitas> Listar(string DataInicio = "", string DataFim = "", string FiltrarNomeCliente = "", Int32? FiltrarSede = null)
        {
            using (var context = new UranusEntities())
            {
                var query = context.FinanceiroReceitas.Include("Clientes")
                                                      .Include("Clientes.Pessoas")
                                                     .Include("Sedes")
                                   .AsQueryable();

                query = query.Where(x => x.Clientes.Pessoas.Nome.Contains(FiltrarNomeCliente));

                if (FiltrarSede != null)
                {
                    query = query.Where(x => x.Clientes.IdSede == FiltrarSede);
                }

                if (DataInicio != "")
                {
                    var dataInicio = DateTime.Parse(DataInicio);
                    query = query.Where(x => x.DataDocumento >= dataInicio);
                }
                if (DataFim != "")
                {
                    var dataFim = DateTime.Parse(DataFim);
                    query = query.Where(x => x.DataDocumento <= dataFim);
                }

                query = query.AsQueryable().OrderBy(x => x.DataDocumento);
//                query = query.AsQueryable().OrderBy(x => x.Clientes.Pessoas.Nome);

                return query.Take(500).ToList();
            }
        }

        public static FinanceiroReceitas Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroReceitas
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.FinanceiroReceitas.Include("Clientes")
                                                                 .Include("Clientes.Pessoas")
                                                                 .Include("FinanceirOrigem")
                                                                 .Include("FinanceiroTipo")
                                                                 .Include("Sedes")
                                                                 .Include("ProcessosAreas")
                                                                 .Include("Bancos")
                                                                 .Include("NotasServicos")
                                                                 .Include("ProcessosAcoes")
                             where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                IdOrigem = d.IdOrigem,
                                OrigemNome = d.FinanceiroOrigem.Nome,
                                NumeroDocumento = d.NumeroDocumento,
                                DataDocumento = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataDocumento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataDocumento), 2) + "/" + SqlFunctions.DatePart("year", d.DataDocumento).ToString(),
                                IdFinanceiroTipo = d.IdFinanceiroTipo,
                                FinanceiroTipoNome = d.FinanceiroTipo.Nome,
                                IdCentroCusto = d.IdCentroCusto,
                                CentroCustoNome = d.Sedes.Nome,
                                IdArea = d.IdArea,
                                AreaNome = d.ProcessosAreas.AreaAtuacao,
                                IdCliente = d.IdCliente,
                                ClienteNome = d.Clientes.Pessoas.Nome,
                                Valor = d.Valor,
                                DataPagamento = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataPagamento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataPagamento), 2) + "/" + SqlFunctions.DatePart("year", d.DataPagamento).ToString(),
                                Observacao = d.Observacao,
                                IdBanco = d.IdBanco,
                                BancoNome = d.Bancos.Nome,
                                IdBancosLancamento = d.IdBancosLancamento,
                                ValorBruto = d.ValorBruto,
                                IRRetido = d.IRRetido,
                                IdProcessoAcao = d.IdProcessoAcao,
                                NumeroProcesso = d.ProcessosAcoes.NumeroProcesso,
                                DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                UsuarioCadastro = d.NomeUsuarioCadastro,
                                DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                UsuarioAlteracao = d.NomeUsuarioAlteracao,
                                NumeroNota = d.NotasServicos.NumeroNota,
                            }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                IdOrigem = x.IdOrigem,
                                OrigemNome = x.OrigemNome,
                                NumeroDocumento = x.NumeroDocumento,
                                DataDocumento = x.DataDocumento,
                                IdFinanceiroTipo = x.IdFinanceiroTipo,
                                FinanceiroTipoNome = x.FinanceiroTipoNome,
                                IdCentroCusto = x.IdCentroCusto,
                                CentroCustoNome = x.CentroCustoNome,
                                IdArea = x.IdArea,
                                AreaNome = x.AreaNome,
                                IdCliente = x.IdCliente,
                                ClienteNome = x.ClienteNome,
                                Valor = (x.Valor != null ? x.Valor.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                DataPagamento = (x.DataPagamento != "0/0/" ? x.DataPagamento : string.Empty),
                                Observacao = x.Observacao,
                                IdBanco = x.IdBanco,
                                BancoNome = x.BancoNome,
                                IdBancosLancamento = x.IdBancosLancamento,
                                ValorBruto = (x.ValorBruto != null ? x.ValorBruto.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                IRRetido = (x.IRRetido != null ? x.IRRetido.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                IdProcessoAcao = x.IdProcessoAcao,
                                NumeroProcesso = x.NumeroProcesso,
                                DataCadastro = x.DataCadastro,
                                UsuarioCadastro = x.UsuarioCadastro,
                                DataAlteracao = x.DataAlteracao,
                                UsuarioAlteracao = x.UsuarioAlteracao,
                                NumeroNota = x.NumeroNota,
                            });

                return query.ToArray();

            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroReceitas.Include("Clientes")
                                                                .Include("Clientes.Pessoas")
                            orderby d.Clientes.Pessoas.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Clientes.Pessoas.Nome,
                                Valor = d.Valor
                            };

                return query.ToArray();
            }
        }

        public static long Importar(Int64 IdReceita, String Usuario)
        {
            using (var context = new UranusEntities())
            {
                SqlParameter param1 = new SqlParameter("@IdReceita", IdReceita);
                SqlParameter param2 = new SqlParameter("@Usuario", Usuario);

                context.Database.ExecuteSqlCommand("stpFinanceiroReceitasImportar @IdReceita, @Usuario", param1, param2);

                var id = (from d in context.NotasServicos
                          where d.IdReceita == IdReceita
                          select d.NumeroNota).FirstOrDefault();

                return (id ?? 0);
            }
        }

        public static FinanceiroReceitas ConsultarReceita(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroReceitas
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

    }
}