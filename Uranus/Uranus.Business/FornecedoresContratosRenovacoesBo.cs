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
    public class FornecedoresContratosRenovacoesBo : IRequiresSessionState
    {
        //public static Int32 Inserir(FornecedoresContratosRenovacoes fornecedorescontratosrenovacoes)
        //{
        //    using (var context = new UranusEntities())
        //    {
        //        context.FornecedoresContratosRenovacoes.Add(fornecedorescontratosrenovacoes);
        //        context.SaveChanges();

        //        return fornecedorescontratosrenovacoes.Id;
        //    }
        //}

        //public static void Salvar(FornecedoresContratosRenovacoes fornecedorescontratosrenovacoes)
        //{
        //    using (var context = new UranusEntities())
        //    {
        //        context.Entry(fornecedorescontratosrenovacoes).State = EntityState.Modified;
        //        context.SaveChanges();
        //    }
        //}

        //public static string Excluir(Int32 id)
        //{
        //    try
        //    {
        //        using (var context = new UranusEntities())
        //        {
        //            var fornecedorescontratosrenovacoes = context.FornecedoresContratosRenovacoes.Find(id);
        //            context.FornecedoresContratosRenovacoes.Attach(fornecedorescontratosrenovacoes);
        //            context.FornecedoresContratosRenovacoes.Remove(fornecedorescontratosrenovacoes);
        //            context.SaveChanges();
        //        }

        //        return "00";
        //    }
        //    catch (Exception ex)
        //    {
        //        String error = "99";
        //        String message = ex.InnerException.ToString();

        //        if (message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
        //            error = "98";

        //        return error;
        //    }
        //}

        //public static List<FornecedoresContratosRenovacoes> Listar(string search)
        //{
        //    using (var context = new UranusEntities())
        //    {
        //        if (String.IsNullOrEmpty(search.Trim()))
        //        {
        //            var query = from d in context.FornecedoresContratosRenovacoes.Include("FornecedoresContratos")
        //                                                                         .Include("FornecedoresContratos.Fornecedores")
        //                        orderby d.FornecedoresContratos.Fornecedores.Nome ascending
        //                        select d;

        //            return query.ToList();
        //        }
        //        else
        //        {
        //            var query = from d in context.FornecedoresContratosRenovacoes.Include("FornecedoresContratos")
        //                                                                         .Include("FornecedoresContratos.Fornecedores")
        //                        where d.FornecedoresContratos.Fornecedores.Nome.Contains(search)
        //                        orderby d.FornecedoresContratos.Fornecedores.Nome ascending
        //                        select d;

        //            return query.ToList();
        //        }
        //    }
        //}

        //public static FornecedoresContratosRenovacoes Consultar(Int32 Id)
        //{
        //    using (var context = new UranusEntities())
        //    {
        //        var query = from d in context.FornecedoresContratosRenovacoes
        //                    where d.Id == Id
        //                    select d;

        //        return query.FirstOrDefault();
        //    }
        //}

        //public static Array ConsultarArray(Int64 Id)
        //{
        //    using (var context = new UranusEntities())
        //    {
        //        var query = (from d in context.FornecedoresContratosRenovacoes
        //                     where d.Id == Id
        //                     select new
        //                     {
        //                         Id = d.Id,
        //                         IdFornecedorContrato = d.IdFornecedorContrato,
        //                         DataInicio = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataInicio), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataInicio), 2) + "/" + SqlFunctions.DatePart("year", d.DataInicio).ToString(),
        //                         DataFim = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataFim), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataFim), 2) + "/" + SqlFunctions.DatePart("year", d.DataFim).ToString(),
        //                         Meses = d.Meses,
        //                         ValorMensal = d.ValorMensal
        //                     }).ToList()
        //                    .Select(x => new
        //                    {
        //                        Id = x.Id,
        //                        IdFornecedorContrato = x.IdFornecedorContrato,
        //                        DataInicio = x.DataInicio,
        //                        DataFim = x.DataFim,
        //                        Meses = x.Meses,
        //                        ValorMensal = (x.ValorMensal != null ? x.ValorMensal.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
        //                    });

        //        return query.ToArray();
        //    }
        //}

        //public static Array Consultar()
        //{
        //    using (var context = new UranusEntities())
        //    {
        //        var query = from d in context.FornecedoresContratosRenovacoes.Include("FornecedoresContratos")
        //                                                                     .Include("FornecedoresContratos.Fornecedores")
        //                    orderby d.FornecedoresContratos.Fornecedores.Nome ascending
        //                    select new
        //                    {
        //                        Id = d.Id,
        //                        Nome = d.FornecedoresContratos.Fornecedores.Nome
        //                    };

        //        return query.ToArray();
        //    }
        //}
    }
}