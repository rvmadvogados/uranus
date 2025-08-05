using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain.Entities;

namespace Uranus.Business
{
    public class ClientesRelatoriosBO : IRequiresSessionState
    {

        public static List<ReceberEmAbertoEmissao> GerarEmissao(string DataInicio = "", string DataFim = "")
        {
            var datainicio = DateTime.Parse(DataInicio);
            var datafim = DateTime.Parse(DataFim);


            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;
                var query = (from d in context.ReceberEmAbertoEmissao(datainicio, datafim)
                            //orderby d.Nome ascending
                            select new
                            {
                                IdCliente = d.IdCliente,
                                NomeCliente = d.NomeCliente,
                                NumeroDocumento = d.NumeroDocumento,
                                DataEmissao = d.DataEmissao,
                                Parcela = d.Parcela,
                                DataVencimento = d.DataVencimento,
                                ValorParcela = d.ValorParcela,
                            }).AsEnumerable().Select(B => new ReceberEmAbertoEmissao()
                            {
                                IdCliente = B.IdCliente,
                                NomeCliente = B.NomeCliente,
                                NumeroDocumento = B.NumeroDocumento,
                                DataEmissao = B.DataEmissao,
                                Parcela = B.Parcela,
                                DataVencimento = B.DataVencimento,
                                ValorParcela = B.ValorParcela,
                            }).ToList();

                return query;
            }
        }

        public static void FiltrarPorCliente(List<ReceberEmAbertoEmissao> listaBase, List<int> Cliente)
        {
            listaBase.RemoveAll(x => !Cliente.Contains(x.IdCliente));
        }

        public static List<ReceberEmAbertoVencimento> GerarVencimento(string DataInicio = "", string DataFim = "")
        {
            var datainicio = DateTime.Parse(DataInicio);
            var datafim = DateTime.Parse(DataFim);


            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;
                var query = (from d in context.ReceberEmAbertoVencimento(datainicio, datafim)
                                 //orderby d.Nome ascending
                             select new
                             {
                                 IdCliente = d.IdCliente,
                                 NomeCliente = d.NomeCliente,
                                 NumeroDocumento = d.NumeroDocumento,
                                 DataEmissao = d.DataEmissao,
                                 Parcela = d.Parcela,
                                 DataVencimento = d.DataVencimento,
                                 ValorParcela = d.ValorParcela,
                             }).AsEnumerable().Select(B => new ReceberEmAbertoVencimento()
                             {
                                 IdCliente = B.IdCliente,
                                 NomeCliente = B.NomeCliente,
                                 NumeroDocumento = B.NumeroDocumento,
                                 DataEmissao = B.DataEmissao,
                                 Parcela = B.Parcela,
                                 DataVencimento = B.DataVencimento,
                                 ValorParcela = B.ValorParcela,
                             }).ToList();

                return query;
            }
        }

        public static void FiltrarPorClienteVencimento(List<ReceberEmAbertoVencimento> listaBase, List<int> Cliente)
        {
            listaBase.RemoveAll(x => !Cliente.Contains(x.IdCliente));
        }

        public static List<ReceberPagas> GerarPagas(Int32 IdEmpresa, string DataInicio = "", string DataFim = "")
        {
            var datainicio = DateTime.Parse(DataInicio);
            var datafim = DateTime.Parse(DataFim);


            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;
                var query = (from d in context.ReceberPagas(datainicio, datafim)
                                 //orderby d.Nome ascending
                             select new
                             {
                                 IdCliente = d.IdCliente,
                                 NomeCliente = d.NomeCliente,
                                 NumeroDocumento = d.NumeroDocumento,
                                 DataEmissao = d.DataEmissao,
                                 Parcela = d.Parcela,
                                 DataVencimento = d.DataVencimento,
                                 ValorParcela = d.ValorParcela,
                                 DataPagamento = d.DataPagamento,
                                 Juros = d.Juros,
                                 Descontos = d.Descontos,
                                 ValorPago = d.ValorPago,
                             }).AsEnumerable().Select(B => new ReceberPagas()
                             {
                                 IdCliente = B.IdCliente,
                                 NomeCliente = B.NomeCliente,
                                 NumeroDocumento = B.NumeroDocumento,
                                 DataEmissao = B.DataEmissao,
                                 Parcela = B.Parcela,
                                 DataVencimento = B.DataVencimento,
                                 ValorParcela = B.ValorParcela,
                                 DataPagamento = B.DataPagamento,
                                 Juros = B.Juros,
                                 Descontos = B.Descontos,
                                 ValorPago = B.ValorPago,
                             }).ToList();

                return query;
            }
        }

        public static void FiltrarPorClientePagas(List<ReceberPagas> listaBase, List<int> Cliente)
        {
            listaBase.RemoveAll(x => !Cliente.Contains(x.IdCliente));
        }

    }

}

