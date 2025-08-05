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
    public class FornecedoresLancamentosParcelasBo : IRequiresSessionState
    {
        public static Int64 Inserir(FornecedoresLancamentosParcelas parcela)
        {
            using (var context = new UranusEntities())
            {
                context.FornecedoresLancamentosParcelas.Add(parcela);
                context.SaveChanges();

                return parcela.Id;
            }
        }

        public static void Salvar(FornecedoresLancamentosParcelas parcela)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(parcela).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int64 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var parcela = context.FornecedoresLancamentosParcelas.Find(id);
                    context.FornecedoresLancamentosParcelas.Attach(parcela);
                    context.FornecedoresLancamentosParcelas.Remove(parcela);
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

        public static List<FornecedoresLancamentosParcelas> Listar(Int64 IdNota)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresLancamentosParcelas
                            where d.IdFornecedoresLancamentos == IdNota
                            select d;

                return query.ToList();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.FornecedoresLancamentosParcelas.Include("FornecedoresLancamentos")
                                                                   .Include("Fornecedores")
                             where d.Id == Id
                             select new
                             {
                                 Id = d.Id,
                                 IdFornecedorNota = d.IdFornecedoresLancamentos,
                                 Parcela = d.Parcela,
                                 Vencimento = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Vencimento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Vencimento), 2) + "/" + SqlFunctions.DatePart("year", d.Vencimento).ToString(),
                                 ValorParcela = d.ValorParcela,
                                 Observacao = d.Observacao,
                                 NumeroDocumento = d.FornecedoresLancamentos.NumeroDocumento,
                                 DataEmissao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.FornecedoresLancamentos.DataEmissao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.FornecedoresLancamentos.DataEmissao), 2) + "/" + SqlFunctions.DatePart("year", d.FornecedoresLancamentos.DataEmissao).ToString(),
                                 FornecedorId = d.FornecedoresLancamentos.Fornecedores.ID,
                                 FornecedorNome = d.FornecedoresLancamentos.Fornecedores.Nome
                             }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                IdFornecedorNota = x.IdFornecedorNota,
                                Parcela = x.Parcela,
                                Vencimento = x.Vencimento,
                                ValorParcela = x.ValorParcela.Value.ToString("#,0.00", new CultureInfo("pt-BR")),
                                Observacao = x.Observacao,
                                NumeroDocumento = x.NumeroDocumento,
                                DataEmissao = x.DataEmissao,
                                FornecedorId = x.FornecedorId,
                                FornecedorNome = x.FornecedorNome
                            });

                return query.ToArray();
            }
        }

        //public static List<FornecedoresLancamentosParcelas> ListarPeriodo(Int64 IdFornecedor, String Nome, String FiltrarDataInicial, String FiltrarDataFinal)
        //{

        //    using (var context = new UranusEntities())
        //    {

        //        DateTime DataInicial;
        //        DateTime DataFinal;

        //        if (FiltrarDataInicial == null || FiltrarDataInicial.Length == 0)
        //        {
        //            DataInicial = DateTime.Now;
        //        }
        //        else
        //        {
        //            DataInicial = DateTime.Parse(FiltrarDataInicial);
        //        }

        //        if (FiltrarDataFinal == null || FiltrarDataFinal.Length == 0)
        //        {
        //            DataFinal = DateTime.Now;
        //        }
        //        else
        //        {
        //            DataFinal = DateTime.Parse(FiltrarDataFinal).AddHours(23).AddMinutes(59).AddSeconds(59);
        //        }

        //        if (IdFornecedor > 0)
        //        {
        //            var query = from d in context.FornecedoresLancamentosParcelas.Include("FornecedoresLancamentos")
        //                                                               .Include("Fornecedores")
        //                        where d.Vencimento >= DataInicial && d.Vencimento <= DataFinal
        //                        where d.DataPagamento == null
        //                        where d.FornecedoresLancamentos.IdFornecedor == IdFornecedor
        //                        orderby d.Vencimento ascending, d.FornecedoresLancamentos.NumeroDocumento ascending
        //                        select d;

        //            return query.ToList();
        //        }
        //        else
        //        {
        //            if (Nome != "")
        //            {
        //                var query = from d in context.FornecedoresLancamentosParcelas.Include("FornecedoresLancamentos")
        //                                                                   .Include("FornecedoresLancamentos.Fornecedores")
        //                            where d.Vencimento >= DataInicial && d.Vencimento <= DataFinal
        //                            where d.DataPagamento == null
        //                            where d.FornecedoresLancamentos.Fornecedores.Nome.Contains(Nome)
        //                            orderby d.Vencimento ascending, d.FornecedoresLancamentos.NumeroDocumento ascending
        //                            select d;

        //                return query.ToList();
        //            }
        //            else
        //            {

        //                {
        //                    var query = from d in context.FornecedoresLancamentosParcelas.Include("FornecedoresLancamentos")
        //                                                                           .Include("FornecedoresLancamentos.Fornecedores")
        //                                where d.Vencimento >= DataInicial && d.Vencimento <= DataFinal
        //                                where d.DataPagamento == null
        //                                orderby d.Vencimento ascending, d.FornecedoresLancamentos.NumeroDocumento ascending
        //                                select d;
        //                    return query.ToList();
        //                }
        //            }
        //        }
        //    }
        //}
    }
}