using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain.Entities;

namespace Uranus.Business
{
    public class FornecedoresRelatoriosBo : IRequiresSessionState
    {
        public static List<PagarEmAbertoEmissao> GerarEmissao(string DataInicio = "", string DataFim = "")
        {
            var datainicio = DateTime.Parse(DataInicio);
            var datafim = DateTime.Parse(DataFim);


            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;
                var query = (from d in context.PagarEmAbertoEmissao(datainicio, datafim)
                                 //orderby d.Nome ascending
                             select new
                             {
                                 IdFornecedor = d.IdFornecedor,
                                 NomeFornecedor = d.NomeFornecedor,
                                 NumeroDocumento = d.NumeroDocumento,
                                 DataEmissao = d.DataEmissao,
                                 Parcela = d.Parcela,
                                 DataVencimento = d.DataVencimento,
                                 ValorParcela = d.ValorParcela,
                             }).AsEnumerable().Select(B => new PagarEmAbertoEmissao()
                             {
                                 IdFornecedor = B.IdFornecedor,
                                 NomeFornecedor = B.NomeFornecedor,
                                 NumeroDocumento = B.NumeroDocumento,
                                 DataEmissao = B.DataEmissao,
                                 Parcela = B.Parcela,
                                 DataVencimento = B.DataVencimento,
                                 ValorParcela = B.ValorParcela,
                             }).ToList();

                return query;
            }
        }

        public static void FiltrarPorFornecedor(List<PagarEmAbertoEmissao> listaBase, List<int> Fornecedor)
        {
            listaBase.RemoveAll(x => !Fornecedor.Contains(x.IdFornecedor));
        }

        public static List<PagarEmAbertoVencimento> GerarVencimento(string DataInicio = "", string DataFim = "")
        {
            var datainicio = DateTime.Parse(DataInicio);
            var datafim = DateTime.Parse(DataFim);


            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;
                var query = (from d in context.PagarEmAbertoVencimento(datainicio, datafim)
                                 //orderby d.Nome ascending
                             select new
                             {
                                 IdFornecedor = d.IdFornecedor,
                                 NomeFornecedor = d.NomeFornecedor,
                                 NumeroDocumento = d.NumeroDocumento,
                                 DataEmissao = d.DataEmissao,
                                 Parcela = d.Parcela,
                                 DataVencimento = d.DataVencimento,
                                 ValorParcela = d.ValorParcela,
                             }).AsEnumerable().Select(B => new PagarEmAbertoVencimento()
                             {
                                 IdFornecedor = B.IdFornecedor,
                                 NomeFornecedor = B.NomeFornecedor,
                                 NumeroDocumento = B.NumeroDocumento,
                                 DataEmissao = B.DataEmissao,
                                 Parcela = B.Parcela,
                                 DataVencimento = B.DataVencimento,
                                 ValorParcela = B.ValorParcela,
                             }).ToList();

                return query;
            }
        }

        public static void FiltrarPorFornecedorVencimento(List<PagarEmAbertoVencimento> listaBase, List<int> Fornecedor)
        {
            listaBase.RemoveAll(x => !Fornecedor.Contains(x.IdFornecedor));
        }

        public static List<PagarPagas> GerarPagas(string DataInicio = "", string DataFim = "")
        {
            var datainicio = DateTime.Parse(DataInicio);
            var datafim = DateTime.Parse(DataFim);


            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;
                var query = (from d in context.PagarPagas(1, datainicio, datafim)
                                 //orderby d.Nome ascending
                             select new
                             {
                                 IdFornecedor = d.IdFornecedor,
                                 NomeFornecedor = d.NomeFornecedor,
                                 NumeroDocumento = d.NumeroDocumento,
                                 DataEmissao = d.DataEmissao,
                                 Parcela = d.Parcela,
                                 DataVencimento = d.DataVencimento,
                                 ValorParcela = d.ValorParcela,
                                 DataPagamento = d.DataPagamento,
                                 Juros = d.Juros,
                                 Descontos = d.Descontos,
                                 ValorPago = d.ValorPago,
                                 Observacao = d.Observacao,
                             }).AsEnumerable().Select(B => new PagarPagas()
                             {
                                 IdFornecedor = B.IdFornecedor,
                                 NomeFornecedor = B.NomeFornecedor,
                                 NumeroDocumento = B.NumeroDocumento,
                                 DataEmissao = B.DataEmissao,
                                 Parcela = B.Parcela,
                                 Vencimento = B.DataVencimento,
                                 ValorParcela = B.ValorParcela,
                                 DataPagamento = B.DataPagamento,
                                 Juros = B.Juros,
                                 Descontos = B.Descontos,
                                 ValorPago = B.ValorPago,
                                 Observacao = B.Observacao,
                             }).ToList();

                return query;
            }
        }

        public static void FiltrarPorFornecedorPagas(List<PagarPagas> listaBase, List<int> Fornecedor)
        {
            listaBase.RemoveAll(x => !Fornecedor.Contains(x.IdFornecedor));
        }

        public static List<PagarRolPagamentos> GerarRolPagamentos(string DataInicio = "", string DataFim = "")
        {
            var datainicio = DateTime.Parse(DataInicio);
            var datafim = DateTime.Parse(DataFim);


            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;
                var query = (from d in context.fnPagarRolPagamentos(0, datainicio, datafim)
                                 //orderby d.Nome ascending
                             select new
                             {
                                 IdFornecedor = d.IdFornecedor,
                                 NomeFornecedor = d.NomeFornecedor,
                                 NumeroDocumento = d.NumeroDocumento,
                                 Parcela = d.Parcela,
                                 Vencimento = d.Vencimento,
                                 ValorParcela = d.ValorParcela,
                             }).AsEnumerable().Select(B => new PagarRolPagamentos()
                             {
                                 IdFornecedor = int.Parse(B.IdFornecedor.ToString()),
                                 NomeFornecedor = B.NomeFornecedor,
                                 NumeroDocumento = B.NumeroDocumento,
                                 Parcela = B.Parcela,
                                 Vencimento = B.Vencimento,
                                 ValorParcela = B.ValorParcela,
                             }).ToList();

                return query;
            }
        }

        public static void FiltrarPorFornecedorRolPagamentos(List<PagarRolPagamentos> listaBase, List<int> Fornecedor)
        {
            listaBase.RemoveAll(x => !Fornecedor.Contains(x.IdFornecedor));
        }
    }
}