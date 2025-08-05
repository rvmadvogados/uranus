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
    public class FornecedoresNotasBo : IRequiresSessionState
    {
        public static Int64 Inserir(FornecedoresNotas nota)
        {
            using (var context = new UranusEntities())
            {
                context.FornecedoresNotas.Add(nota);
                context.SaveChanges();

                return nota.Id;
            }
        }

        public static void Salvar(FornecedoresNotas nota)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(nota).State = EntityState.Modified;
                context.Entry(nota).Property(x => x.Total).IsModified = false;
                context.Entry(nota).Property(x => x.Plano).IsModified = false;
                context.Entry(nota).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(nota).Property(x => x.NomeUsuarioCadastro).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int64 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var nota = context.FornecedoresNotas.Find(id);
                    context.FornecedoresNotas.Attach(nota);
                    context.FornecedoresNotas.Remove(nota);
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
                    var query = from d in context.Fornecedores.Include("FornecedoresNotas")
                                                              .Include("FornecedoresNotas.FornecedoresNotasParcelas")
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else if ((!String.IsNullOrEmpty(CPFCNPJ.Trim()) || !String.IsNullOrEmpty(Nome.Trim())))
                {
                    var query = from d in context.Fornecedores.Include("FornecedoresNotas")
                                                              .Include("FornecedoresNotas.FornecedoresNotasParcelas")
                                where d.CpfCnpj.Contains(CPFCNPJ)
                                where d.Nome.Contains(Nome)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Fornecedores.Include("FornecedoresNotas")
                                                              .Include("FornecedoresNotas.FornecedoresNotasParcelas")
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
                var query = from d in context.FornecedoresNotas.Include("Fornecedores")
                            orderby d.Fornecedores.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Fornecedores.Nome
                            };

                return query.ToArray();
            }
        }

        public static FornecedoresNotas Consultar(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresNotas.Include("FornecedoresNotasParcelas")
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
                var query = (from d in context.FornecedoresNotas.Include("FornecedoresNotasParcelas")
                                                                .Include("Fornecedores")
                             where d.Id == Id
                             select new
                             {
                                 Id = d.Id,
                                 IdEmpresa = d.IdEmpresa,
                                 IdFornecedor = d.IdFornecedor,
                                 NumeroDocumento = d.NumeroDocumento,
                                 DataEmissao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataEmissao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataEmissao), 2) + "/" + SqlFunctions.DatePart("year", d.DataEmissao).ToString(),
                                 Total = d.Total,
                                 Plano = d.Plano,
                                 ValorParcela = d.ValorParcela,
                                 IdFormaPagamento = d.IdFormaPagamento,
                                 Observacao = d.Observacao,
                                 PrimeiroVencimento = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.PrimeiroVencimento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.PrimeiroVencimento), 2) + "/" + SqlFunctions.DatePart("year", d.PrimeiroVencimento).ToString(),
                                 IdContrato = d.IdContrato,
                                 IdAcao = d.IdAcao,
                                 IdSede = d.IdSede,
                                 DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                 UsuarioCadastro = d.NomeUsuarioCadastro,
                                 DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                 UsuarioAlteracao = d.NomeUsuarioAlteracao,
                             }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                IdEmpresa = x.IdEmpresa,
                                IdFornecedor = x.IdFornecedor,
                                NumeroDocumento = x.NumeroDocumento,
                                DataEmissao = x.DataEmissao,
                                Total = (x.Total != null ? x.Total.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                Plano = x.Plano,
                                ValorParcela = (x.ValorParcela != null ? x.ValorParcela.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                IdFormaPagamento = x.IdFormaPagamento,
                                Observacao = x.Observacao,
                                PrimeiroVencimento = x.PrimeiroVencimento,
                                IdContrato = x.IdContrato,
                                IdAcao = x.IdAcao,
                                IdSede = x.IdSede,
                                DataCadastro = x.DataCadastro,
                                UsuarioCadastro = x.UsuarioCadastro,
                                DataAlteracao = x.DataAlteracao,
                                UsuarioAlteracao = x.UsuarioAlteracao,
                            });

                return query.ToArray();
            }
        }

        public static void GerarNotaParcelas(Int64 IdNota)
        {
            using (var context = new UranusEntities())
            {
                SqlParameter param1 = new SqlParameter("@IdNota", IdNota);

                context.Database.ExecuteSqlCommand("stpGeraFornecedoresNotasParcelas @IdNota", param1);
            }
        }
    }
}