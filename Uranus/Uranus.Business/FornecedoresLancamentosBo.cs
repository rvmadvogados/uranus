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
    public class FornecedoresLancamentosBo : IRequiresSessionState
    {
        public static Int64 Inserir(FornecedoresLancamentos fornecedoreslancamentos)
        {
            using (var context = new UranusEntities())
            {
                context.FornecedoresLancamentos.Add(fornecedoreslancamentos);
                context.SaveChanges();

                return fornecedoreslancamentos.Id;
            }
        }

        public static void Salvar(FornecedoresLancamentos fornecedoreslancamentos)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(fornecedoreslancamentos).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int64 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var fornecedoreslancamentos = context.FornecedoresLancamentos.Find(id);
                    context.FornecedoresLancamentos.Attach(fornecedoreslancamentos);
                    context.FornecedoresLancamentos.Remove(fornecedoreslancamentos);
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

        public static List<Fornecedores> Listar(string CPFCNPJ, string Nome)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(CPFCNPJ.Trim()) && String.IsNullOrEmpty(Nome.Trim()))
                {
                    var query = from d in context.Fornecedores.Include("FornecedoresLancamentos")
                                                              .Include("FornecedoresLancamentos.FornecedoresLancamentosParcelas")
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else if ((!String.IsNullOrEmpty(CPFCNPJ.Trim()) || !String.IsNullOrEmpty(Nome.Trim())))
                {
                    var query = from d in context.Fornecedores.Include("FornecedoresLancamentos")
                                                              .Include("FornecedoresLancamentos.FornecedoresLancamentosParcelas")
                                where d.CpfCnpj.Contains(CPFCNPJ)
                                where d.Nome.Contains(Nome)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Fornecedores.Include("FornecedoresLancamentos")
                                                              .Include("FornecedoresLancamentos.FornecedoresLancamentosParcelas")
                                where d.CpfCnpj.Contains(CPFCNPJ)
                                where d.Nome.Contains(Nome)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresLancamentos.Include("Fornecedores")
                            orderby d.Fornecedores.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Fornecedores.Nome
                            };

                return query.ToArray();
            }
        }

        public static FornecedoresLancamentos Consultar(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresLancamentos.Include("FornecedoresLancamentosParcelas")
                                                               .Include("Fornecedores")
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.FornecedoresLancamentos.Include("FornecedoresLancamentosParcelas")
                                                                .Include("Fornecedores")
                             where d.Id == Id
                             select new
                             {
                                 Id = d.Id,
                                 IdFornecedor = d.IdFornecedor,
                                 NumeroDocumento = d.NumeroDocumento,
                                 DataEmissao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataEmissao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataEmissao), 2) + "/" + SqlFunctions.DatePart("year", d.DataEmissao).ToString(),
                                 Valor = d.Valor,
                                 Plano = d.Plano,
                                 Observacao = d.Observacao,
                                 PrimeiroVencimento = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.PrimeiroVencimento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.PrimeiroVencimento), 2) + "/" + SqlFunctions.DatePart("year", d.PrimeiroVencimento).ToString(),
                             }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                IdFornecedor = x.IdFornecedor,
                                NumeroDocumento = x.NumeroDocumento,
                                DataEmissao = x.DataEmissao,
                                Valor = (x.Valor != null ? x.Valor.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                Plano = x.Plano,
                                Observacao = x.Observacao,
                                PrimeiroVencimento = x.PrimeiroVencimento                            });

                return query.ToArray();
            }
        }

        public static void GerarNotaParcelas(Int64 IdNota)
        {
            using (var context = new UranusEntities())
            {
                SqlParameter param1 = new SqlParameter("@IdNota", IdNota);

                context.Database.ExecuteSqlCommand("stpGeraFornecedoresLancamentosParcelas @IdNota", param1);
            }
        }
    }
}