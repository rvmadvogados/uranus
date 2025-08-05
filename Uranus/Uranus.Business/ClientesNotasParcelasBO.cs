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
    public class ClientesNotasParcelasBO : IRequiresSessionState
    {
        public static Int64 Inserir(ClientesNotasParcelas parcela)
        {
            using (var context = new UranusEntities())
            {
                context.ClientesNotasParcelas.Add(parcela);
                context.SaveChanges();

                return parcela.Id;
            }
        }

        public static void Salvar(ClientesNotasParcelas parcela)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(parcela).State = EntityState.Modified;
                context.Entry(parcela).Property(x => x.LocalPagamento).IsModified = false;
                context.Entry(parcela).Property(x => x.IdBanco).IsModified = false;
                context.Entry(parcela).Property(x => x.Boleto).IsModified = false;
                context.Entry(parcela).Property(x => x.Status).IsModified = false;
                context.Entry(parcela).Property(x => x.NossoNumero).IsModified = false;
                context.Entry(parcela).Property(x => x.Arquivo).IsModified = false;
                context.Entry(parcela).Property(x => x.JurosDia).IsModified = false;
                context.Entry(parcela).Property(x => x.LinhaDigitavel).IsModified = false;
                context.Entry(parcela).Property(x => x.CODBARRAS).IsModified = false;
                context.Entry(parcela).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(parcela).Property(x => x.NomeUsuarioCadastro).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int64 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var parcela = context.ClientesNotasParcelas.Find(id);
                    context.ClientesNotasParcelas.Attach(parcela);
                    context.ClientesNotasParcelas.Remove(parcela);
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

        public static List<ClientesNotasParcelas> Listar(Int64 IdNota)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ClientesNotasParcelas
                            where d.IdClientesNota == IdNota
                            select d;

                return query.ToList();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ClientesNotasParcelas.Include("ClientesNotas")
                                                                    .Include("Clientes")
                                                                    .Include("Pessoas")
                                                                    .Include("FinanceiroOrigem")
                             where d.Id == Id
                             select new
                             {
                                 Id = d.Id,
                                 IdClientesNota = d.IdClientesNota,
                                 Parcela = d.Parcela,
                                 Vencimento = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Vencimento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Vencimento), 2) + "/" + SqlFunctions.DatePart("year", d.Vencimento).ToString(),
                                 ValorParcela = d.ValorParcela,
                                 DataPagamento = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataPagamento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataPagamento), 2) + "/" + SqlFunctions.DatePart("year", d.DataPagamento).ToString(),
                                 ValorPago = d.ValorPago,
                                 Juros = d.Juros,
                                 Descontos = d.Descontos,
                                 Saldo = d.Saldo,
                                 Observacao = d.Observacao,
                                 LocalPagamento = d.LocalPagamento,
                                 IdBanco = d.IdBanco,
                                 IdDestino = d.IdDestino,
                                 NomeDestino = d.FinanceiroOrigem.Nome,
                                 IdFormapagamento = d.IdFormapagamento,
                                 Boleto = d.Boleto,
                                 Status = d.Status,
                                 NossoNumero = d.NossoNumero,
                                 Arquivo = d.Arquivo,
                                 JurosDia = d.JurosDia,
                                 LinhaDigitavel = d.LinhaDigitavel,
                                 CODBARRAS = d.CODBARRAS,
                                 NumeroDocumento = d.ClientesNotas.NumeroDocumento,
                                 DataEmissao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.ClientesNotas.DataEmissao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.ClientesNotas.DataEmissao), 2) + "/" + SqlFunctions.DatePart("year", d.ClientesNotas.DataEmissao).ToString(),
                                 ClienteId = d.ClientesNotas.Clientes.ID,
                                 ClientesNome = d.ClientesNotas.Clientes.Pessoas.Nome,
                                 DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                 UsuarioCadastro = d.NomeUsuarioCadastro,
                                 DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                 UsuarioAlteracao = d.NomeUsuarioAlteracao,
                             }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                IdClientesNota = x.IdClientesNota,
                                Parcela = x.Parcela,
                                Vencimento = x.Vencimento,
                                ValorParcela = x.ValorParcela.Value.ToString("#,0.00", new CultureInfo("pt-BR")),
                                DataPagamento = x.DataPagamento,
                                ValorPago = x.ValorPago.Value.ToString("#,0.00", new CultureInfo("pt-BR")),
                                Juros = x.Juros.Value.ToString("#,0.00", new CultureInfo("pt-BR")),
                                Descontos = x.Descontos.Value.ToString("#,0.00", new CultureInfo("pt-BR")),
                                Saldo = x.Saldo.Value.ToString("#,0.00", new CultureInfo("pt-BR")),
                                Observacao = x.Observacao,
                                LocalPagamento = x.LocalPagamento,
                                IdBanco = x.IdBanco,
                                IdDestino = x.IdDestino,
                                NomeDestino = x.NomeDestino,
                                IdFormapagamento = x.IdFormapagamento,
                                Boleto = x.Boleto,
                                Status = x.Status,
                                NossoNumero = x.NossoNumero,
                                Arquivo = x.Arquivo,
                                JurosDia = x.JurosDia,
                                LinhaDigitavel = x.LinhaDigitavel,
                                CODBARRAS = x.CODBARRAS,
                                NumeroDocumento = x.NumeroDocumento,
                                DataEmissao = x.DataEmissao,
                                ClienteId = x.ClienteId,
                                ClientesNome = x.ClientesNome,
                                DataCadastro = x.DataCadastro,
                                UsuarioCadastro = x.UsuarioCadastro,
                                DataAlteracao = x.DataAlteracao,
                                UsuarioAlteracao = x.UsuarioAlteracao,
                            });

                return query.ToArray();
            }
        }

        public static List<ClientesNotasParcelas> ListarPeriodo(Int64 IdCliente, String Nome, String FiltrarDataInicial, String FiltrarDataFinal)
        {

            using (var context = new UranusEntities())
            {

                DateTime DataInicial;
                DateTime DataFinal;

                if (FiltrarDataInicial == null || FiltrarDataInicial.Length == 0)
                {
                    DataInicial = DateTime.Now;
                }
                else
                {
                    DataInicial = DateTime.Parse(FiltrarDataInicial);
                }

                if (FiltrarDataFinal == null || FiltrarDataFinal.Length == 0)
                {
                    DataFinal = DateTime.Now;
                }
                else
                {
                    DataFinal = DateTime.Parse(FiltrarDataFinal).AddHours(23).AddMinutes(59).AddSeconds(59);
                }

                if (IdCliente > 0)
                {
                    var query = from d in context.ClientesNotasParcelas.Include("ClientesNotas")
                                                                       .Include("ClientesNotas.Clientes")
                                                                       .Include("ClientesNotas.Clientes.Pessoas")
                                where d.DataPagamento == null
                                where d.ClientesNotas.IdCliente == IdCliente
                                orderby d.Vencimento ascending, d.ClientesNotas.NumeroDocumento ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    if (Nome != "")
                    {
                        var query = from d in context.ClientesNotasParcelas.Include("ClientesNotas")
                                                                           .Include("ClientesNotas.Clientes")
                                                                           .Include("ClientesNotas.Clientes.Pessoas")
                                    where d.DataPagamento == null
                                    where d.ClientesNotas.Clientes.Pessoas.Nome.Contains(Nome)
                                    orderby d.Vencimento ascending, d.ClientesNotas.NumeroDocumento ascending
                                    select d;

                        return query.ToList();
                    }
                    else
                    {

                        {
                            var query = from d in context.ClientesNotasParcelas.Include("ClientesNotas")
                                                                               .Include("ClientesNotas.Clientes")
                                                                               .Include("ClientesNotas.Clientes.Pessoas")
                                        where d.Vencimento >= DataInicial && d.Vencimento <= DataFinal
                                        where d.DataPagamento == null
                                        orderby d.Vencimento ascending, d.ClientesNotas.NumeroDocumento ascending
                                        select d;
                            return query.ToList();

                        }
                    }

                }

            }
        }

        public static ClientesNotasParcelas ListarParcela(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ClientesNotasParcelas
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

        public static ClientesNotasParcelas ConsultarParcela(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ClientesNotasParcelas
                            where d.IdBanco == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static ClientesNotasParcelas ConsultarParcelaBaixa(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ClientesNotasParcelas
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static ClientesNotasParcelas ConsultarProximaParcelaBaixa(Int64 IdNota, Int32 Parcela)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ClientesNotasParcelas
                            where d.IdClientesNota == IdNota && d.Parcela == Parcela
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<ClientesNotasParcelas> ListarBoletos(string Status)
        {

            using (var context = new UranusEntities())
            {

                var query = from d in context.ClientesNotasParcelas.Include("ClientesNotas")
                                //                                                   .Include("ClientesNotas.Clientes")
                                //                                                   .Include("ClientesNotas.Clientes.Pessoas")
                where d.DataPagamento == null
                where d.IdBanco == 2 || d.IdBanco == 3
                where d.Boleto == true
                where d.Status == Status
                orderby d.ClientesNotas.IdCliente, d.ClientesNotas.NumeroDocumento, d.Parcela ascending
                select d;

                return query.ToList();
            }
        }

    }
}