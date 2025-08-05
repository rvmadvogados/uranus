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
    public class FornecedoresNotasParcelasBo : IRequiresSessionState
    {
        public static Int64 Inserir(FornecedoresNotasParcelas parcela)
        {
            using (var context = new UranusEntities())
            {
                context.FornecedoresNotasParcelas.Add(parcela);
                context.SaveChanges();

                return parcela.Id;
            }
        }

        public static void Salvar(FornecedoresNotasParcelas parcela)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(parcela).State = EntityState.Modified;
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
                    var parcela = context.FornecedoresNotasParcelas.Find(id);
                    context.FornecedoresNotasParcelas.Attach(parcela);
                    context.FornecedoresNotasParcelas.Remove(parcela);
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

        public static List<FornecedoresNotasParcelas> Listar(Int64 IdNota)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresNotasParcelas
                            where d.IdFornecedoresNota == IdNota
                            select d;

                return query.ToList();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.FornecedoresNotasParcelas.Include("FornecedoresNotas")
                                                                   .Include("Fornecedores")
                             where d.Id == Id
                             select new
                             {
                                 Id = d.Id,
                                 IdFornecedorNota = d.IdFornecedoresNota,
                                 Parcela = d.Parcela,
                                 Vencimento = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Vencimento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Vencimento), 2) + "/" + SqlFunctions.DatePart("year", d.Vencimento).ToString(),
                                 ValorParcela = d.ValorParcela,
                                 Saldo = d.Saldo,
                                 Observacao = d.Observacao,
                                 NumeroDocumento = d.FornecedoresNotas.NumeroDocumento,
                                 DataEmissao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.FornecedoresNotas.DataEmissao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.FornecedoresNotas.DataEmissao), 2) + "/" + SqlFunctions.DatePart("year", d.FornecedoresNotas.DataEmissao).ToString(),
                                 FornecedorId = d.FornecedoresNotas.Fornecedores.ID,
                                 FornecedorNome = d.FornecedoresNotas.Fornecedores.Nome,
                                 DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                 UsuarioCadastro = d.NomeUsuarioCadastro,
                                 DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                 UsuarioAlteracao = d.NomeUsuarioAlteracao,
                             }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                IdFornecedorNota = x.IdFornecedorNota,
                                Parcela = x.Parcela,
                                Vencimento = x.Vencimento,
                                ValorParcela = x.ValorParcela.Value.ToString("#,0.00", new CultureInfo("pt-BR")),
                                Saldo = x.Saldo.Value.ToString("#,0.00", new CultureInfo("pt-BR")),
                                Observacao = x.Observacao,
                                NumeroDocumento = x.NumeroDocumento,
                                DataEmissao = x.DataEmissao,
                                FornecedorId = x.FornecedorId,
                                FornecedorNome = x.FornecedorNome,
                                DataCadastro = x.DataCadastro,
                                UsuarioCadastro = x.UsuarioCadastro,
                                DataAlteracao = x.DataAlteracao,
                                UsuarioAlteracao = x.UsuarioAlteracao,
                            });

                return query.ToArray();
            }
        }

        public static List<FornecedoresNotasParcelas> ListarPeriodo(Int64 IdFornecedor, String Nome, String FiltrarDataInicial, String FiltrarDataFinal)
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

                if (IdFornecedor > 0)
                {
                    var query = from d in context.FornecedoresNotasParcelas.Include("FornecedoresNotas")
                                                                       .Include("Fornecedores")
                                //where d.Vencimento >= DataInicial && d.Vencimento <= DataFinal
                                where d.DataPagamento == null
                                where d.FornecedoresNotas.IdFornecedor == IdFornecedor
                                orderby d.Vencimento ascending, d.FornecedoresNotas.NumeroDocumento ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    if (Nome != "")
                    {
                        var query = from d in context.FornecedoresNotasParcelas.Include("FornecedoresNotas")
                                                                           .Include("FornecedoresNotas.Fornecedores")
                                    //where d.Vencimento >= DataInicial && d.Vencimento <= DataFinal
                                    where d.DataPagamento == null
                                    where d.FornecedoresNotas.Fornecedores.Nome.Contains(Nome)
                                    orderby d.Vencimento ascending, d.FornecedoresNotas.NumeroDocumento ascending
                                    select d;

                        return query.ToList();
                    }
                    else
                    {

                        {
                            var query = from d in context.FornecedoresNotasParcelas.Include("FornecedoresNotas")
                                                                                   .Include("FornecedoresNotas.Fornecedores")
                                        where d.Vencimento >= DataInicial && d.Vencimento <= DataFinal
                                        where d.DataPagamento == null
                                        orderby d.Vencimento ascending, d.FornecedoresNotas.NumeroDocumento ascending
                                        select d;
                            return query.ToList();
                        }
                    }
                }
            }
        }
    }
}