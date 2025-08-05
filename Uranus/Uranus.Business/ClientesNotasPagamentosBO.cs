using Uranus.Data;
using Uranus.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;

namespace Uranus.Business
{
    public class ClientesNotasPagamentosBO : IRequiresSessionState
    {
        public static Int64 Inserir(ClientesNotasPagamentos pagamento)
        {
            using (var context = new UranusEntities())
            {
                context.ClientesNotasPagamentos.Add(pagamento);
                context.SaveChanges();

                return pagamento.Id;
            }
        }

        public static void Salvar(ClientesNotasPagamentos pagamento)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(pagamento).State = EntityState.Modified;
                context.Entry(pagamento).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(pagamento).Property(x => x.NomeUsuarioCadastro).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int64 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var pagamento = context.ClientesNotasPagamentos.Find(id);
                    context.ClientesNotasPagamentos.Attach(pagamento);
                    context.ClientesNotasPagamentos.Remove(pagamento);
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

        public static List<ClientesNotasPagamentos> Listar(Int64 IdNota)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ClientesNotasPagamentos
                            where d.IdClienteNota == IdNota
                            select d;

                return query.ToList();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ClientesNotasPagamentos.Include("ClientesNotas")
                                                                    .Include("Clientes")
                                                                    .Include("Pessoas")
                                                                    .Include("FinanceiroOrigem")
                             where d.Id == Id
                             select new
                             {
                                 Id = d.Id,
                                 IdClientesNota = d.IdClienteNota,
                                 DataPagamento = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataPagamento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataPagamento), 2) + "/" + SqlFunctions.DatePart("year", d.DataPagamento).ToString(),
                                 ValorPago = d.ValorPago,
                                 Juros = d.Juros,
                                 Desconto = d.Desconto,
                                 Observacao = d.Observacao,
                                 IdDestino = d.IdDestino,
                                 IdFormapagamento = d.IdFormaPagamento,
                                 NumeroDocumento = d.ClientesNotas.NumeroDocumento,
                                 DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                 UsuarioCadastro = d.NomeUsuarioCadastro,
                                 DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                 UsuarioAlteracao = d.NomeUsuarioAlteracao,
                             }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                IdClientesNota = x.IdClientesNota,
                                DataPagamento = x.DataPagamento,
                                ValorPago = x.ValorPago.Value.ToString("#,0.00", new CultureInfo("pt-BR")),
                                Juros = x.Juros.Value.ToString("#,0.00", new CultureInfo("pt-BR")),
                                Descontos = x.Desconto.Value.ToString("#,0.00", new CultureInfo("pt-BR")),
                                Observacao = x.Observacao,
                                IdDestino = x.IdDestino,
                                IdFormapagamento = x.IdFormapagamento,
                                NumeroDocumento = x.NumeroDocumento,
                                DataCadastro = x.DataCadastro,
                                UsuarioCadastro = x.UsuarioCadastro,
                                DataAlteracao = x.DataAlteracao,
                                UsuarioAlteracao = x.UsuarioAlteracao,
                            });

                return query.ToArray();
            }
        }

        public static ClientesNotasPagamentos ListarParcela(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ClientesNotasPagamentos
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Clientes Consultar(string CpfCnpj)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Clientes
                            where d.Pessoas.CpfCnpj == CpfCnpj
                            select d;

                return query.FirstOrDefault();
            }
        }

    }
}