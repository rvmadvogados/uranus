using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class FinanceiroDespesasBo : IRequiresSessionState
    {
        public static Int64 Inserir(FinanceiroDespesas financeirodespesas)
        {
            using (var context = new UranusEntities())
            {
                context.FinanceiroDespesas.Add(financeirodespesas);
                context.SaveChanges();

                return financeirodespesas.Id;
            }
        }

        public static void Salvar(FinanceiroDespesas financeirodespesas)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(financeirodespesas).State = EntityState.Modified;
                context.Entry(financeirodespesas).Property(x => x.IdPagar).IsModified = false;
                context.Entry(financeirodespesas).Property(x => x.IdCaixa).IsModified = false;
                context.Entry(financeirodespesas).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(financeirodespesas).Property(x => x.NomeUsuarioCadastro).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var financeirodespesas = context.FinanceiroDespesas.Find(id);
                    context.FinanceiroDespesas.Attach(financeirodespesas);
                    context.FinanceiroDespesas.Remove(financeirodespesas);
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

        public static List<FinanceiroDespesas> Listar(string DataInicio = "", string DataFim = "", string FiltrarNomeCliente = "", Int32? FiltrarSede = null)
        {
            using (var context = new UranusEntities())
            {
                var query = context.FinanceiroDespesas.Include("Fornecedores")
                                                      .Include("Sedes")
                                   .AsQueryable();

                query = query.Where(x => x.Fornecedores.Nome.Contains(FiltrarNomeCliente));

                if (FiltrarSede != null)
                {
                    query = query.Where(x => x.IdCentroCusto == FiltrarSede);
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

                query = query.AsQueryable().OrderBy(x => x.Fornecedores.Nome);

                return query.Take(500).ToList();
            }
        }

        public static FinanceiroDespesas Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroDespesas
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.FinanceiroDespesas.Include("Fornecedores")
                                                                 .Include("FinanceirOrigem")
                                                                 .Include("FinanceiroTipo")
                                                                 .Include("Sedes")
                                                                 .Include("Bancos")
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
                                IdFornecedor = d.IdFornecedor,
                                FornecedorNome = d.Fornecedores.Nome,
                                Valor = d.Valor,
                                DataPagamento = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataPagamento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataPagamento), 2) + "/" + SqlFunctions.DatePart("year", d.DataPagamento).ToString(),
                                Observacao = d.Observacao,
                                IdBanco = d.IdBanco,
                                BancoNome = d.Bancos.Nome,
                                IdBancosLancamento = d.IdBancosLancamento,
                                DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                UsuarioCadastro = d.NomeUsuarioCadastro,
                                DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                UsuarioAlteracao = d.NomeUsuarioAlteracao,
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
                                IdFornecedor = x.IdFornecedor,
                                FornecedorNome = x.FornecedorNome,
                                Valor = (x.Valor != null ? x.Valor.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                DataPagamento = (x.DataPagamento != "0/0/" ? x.DataPagamento : string.Empty),
                                Observacao = x.Observacao,
                                IdBanco = x.IdBanco,
                                BancoNome = x.BancoNome,
                                IdBancosLancamento = x.IdBancosLancamento,
                                DataCadastro = x.DataCadastro,
                                UsuarioCadastro = x.UsuarioCadastro,
                                DataAlteracao = x.DataAlteracao,
                                UsuarioAlteracao = x.UsuarioAlteracao,
                            });

                return query.ToArray();

            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroDespesas.Include("Fornecedores")
                            orderby d.Fornecedores.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Fornecedores.Nome,
                                Valor = d.Valor
                            };

                return query.ToArray();
            }
        }
    }
}