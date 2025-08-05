using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using System.Data.SqlClient;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Data;
using Uranus.Domain;
using Uranus.Data;

namespace Uranus.Business
{
    public class ClientesNotasBO : IRequiresSessionState
    {
        public static Int64 Inserir(ClientesNotas nota)
        {
            using (var context = new UranusEntities())
            {
                context.ClientesNotas.Add(nota);
                context.SaveChanges();

                return nota.Id;
            }
        }

        public static void Salvar(ClientesNotas nota)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(nota).State = EntityState.Modified;
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
                    var nota = context.ClientesNotas.Find(id);
                    context.ClientesNotas.Attach(nota);
                    context.ClientesNotas.Remove(nota);
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

        public static List<Clientes> Listar(string CPFCNPJ, string Nome)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(CPFCNPJ.Trim()) && String.IsNullOrEmpty(Nome.Trim()))
                {
                    var query = from d in context.Clientes.Include("Pessoas")
                                                          .Include("ClientesNotas")
                                                          .Include("ClientesNotas.ClientesNotasParcelas")
                                                          .Include("ClientesNotas.FinanceiroTipo")
                                                          .Include("ClientesNotas.ProcessosAreas")
                                                          .Include("ClientesNotas.Sedes")
                                                          .Include("ClientesNotas.FormaPagamentos")

                                orderby d.Status descending, d.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
                else if ((!String.IsNullOrEmpty(CPFCNPJ.Trim()) || !String.IsNullOrEmpty(Nome.Trim())))
                {
                    var query = from d in context.Clientes.Include("Pessoas")
                                                          .Include("ClientesNotas")
                                                          .Include("ClientesNotas.ClientesNotasParcelas")
                                                          .Include("ClientesNotas.FinanceiroTipo")
                                                          .Include("ClientesNotas.ProcessosAreas")
                                                          .Include("ClientesNotas.Sedes")
                                                          .Include("ClientesNotas.FormaPagamentos")

                                where d.Pessoas.CpfCnpj.Contains(CPFCNPJ)
                                where d.Pessoas.Nome.Contains(Nome)
                                orderby d.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Clientes.Include("Pessoas")
                                                          .Include("ClientesNotas")
                                                          .Include("ClientesNotas.ClientesNotasParcelas")
                                                          .Include("ClientesNotas.FinanceiroTipo")
                                                          .Include("ClientesNotas.ProcessosAreas")
                                                          .Include("ClientesNotas.Sedes")
                                                          .Include("ClientesNotas.FormaPagamentos")

                                where d.Pessoas.CpfCnpj.Contains(CPFCNPJ)
                                where d.Pessoas.Nome.Contains(Nome)
                                orderby d.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ClientesNotas.Include("Clientes")
                                                           .Include("Pessoas")
                            orderby d.Clientes.Pessoas.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Clientes.Pessoas.Nome
                            };

                return query.ToArray();
            }
        }

        public static ClientesNotas Consultar(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ClientesNotas.Include("ClientesNotasParcelas")
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ClientesNotas.Include("ClientesNotasParcelas")
                                                            .Include("Clientes")
                                                            .Include("Clientes.Pessoas")
                                                            .Include("FinanceiroTipo")
                                                            .Include("ProcessosAreas")
                                                            .Include("Sedes")
                                                            .Include("FormaPagamentos")
                                                            .Include("ProcessosAcoes")
                             where d.Id == Id
                             select new
                             {
                                 Id = d.Id,
                                 IdCliente = d.IdCliente,
                                 ClienteNome = d.Clientes.Pessoas.Nome,
                                 ClienteCpjCnpj = d.Clientes.Pessoas.CpfCnpj,
                                 NumeroDocumento = d.NumeroDocumento,
                                 DataEmissao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataEmissao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataEmissao), 2) + "/" + SqlFunctions.DatePart("year", d.DataEmissao).ToString(),
                                 Total = d.Total,
                                 Plano = d.Plano,
                                 ValorParcela = d.ValorParcela,
                                 Observacao = d.Observacao,
                                 IdTipo = d.IdTipo,
                                 IdFinanceiroTipo = d.IdFinanceiroTipo,
                                 NomeFinanceiroTipo = d.FinanceiroTipo.Nome,
                                 IdSede = d.IdSede,
                                 NomeSede = d.Sedes.Nome,
                                 IdArea = d.IdArea,
                                 NomeArea = d.ProcessosAreas.AreaAtuacao,
                                 IdFormaPagamento = d.IdFormaPagamento,
                                 IdBanco = d.IdBanco,
                                 NomeFormaPagamento = d.FormaPagamentos.Nome,
                                 NomeIndicacao = d.Clientes.NomeIndicacao,
                                 PrimeiroVencimento = (d.PrimeiroVencimento != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.PrimeiroVencimento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.PrimeiroVencimento), 2) + "/" + SqlFunctions.DatePart("year", d.PrimeiroVencimento).ToString() : string.Empty),
                                 IdProcessoAcao = d.IdProcessoAcao,
                                 NumeroProcesso = d.ProcessosAcoes.NumeroProcesso,
                                 DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                 UsuarioCadastro = d.NomeUsuarioCadastro,
                                 DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                 UsuarioAlteracao = d.NomeUsuarioAlteracao,
                             }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                IdCliente = x.IdCliente,
                                ClienteNome = x.ClienteNome,
                                ClienteCpjCnpj = x.ClienteCpjCnpj,
                                NumeroDocumento = x.NumeroDocumento,
                                DataEmissao = x.DataEmissao,
                                Total = (x.Total != null ? x.Total.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                Plano = x.Plano,
                                ValorParcela = (x.ValorParcela != null ? x.ValorParcela.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                Observacao = x.Observacao,
                                IdTipo = x.IdTipo,
                                IdFinanceiroTipo = x.IdFinanceiroTipo,
                                NomeFinanceiroTipo = x.NomeFinanceiroTipo,
                                IdSede = x.IdSede,
                                NomeSede = x.NomeSede,
                                IdArea = x.IdArea,
                                NomeArea = x.NomeArea,
                                IdBanco = x.IdBanco,
                                IdFormaPagamento = x.IdFormaPagamento,
                                NomeFormaPagamento = x.NomeFormaPagamento,
                                NomeIndicacao = x.NomeIndicacao,
                                PrimeiroVencimento = x.PrimeiroVencimento,
                                IdProcessoAcao = x.IdProcessoAcao,
                                NumeroProcesso = x.NumeroProcesso,
                                DataCadastro = x.DataCadastro,
                                UsuarioCadastro = x.UsuarioCadastro,
                                DataAlteracao = x.DataAlteracao,
                                UsuarioAlteracao = x.UsuarioAlteracao,
                            });

                return query.ToArray();
            }
        }

        public static void GerarNotaReceita(Int64 IdReceita)
        {
            using (var context = new UranusEntities())
            {
                SqlParameter param1 = new SqlParameter("@IdReceita", IdReceita);

                context.Database.ExecuteSqlCommand("stpGeraNotasReceitas @IdReceita", param1);
            }
        }

        public static void GerarNotaParcelas(Int64 IdNota)
        {
            using (var context = new UranusEntities())
            {
                SqlParameter param1 = new SqlParameter("@IdNota", IdNota);

                context.Database.ExecuteSqlCommand("stpGeraClientesNotasParcelas @IdNota", param1);
            }
        }
    }
}