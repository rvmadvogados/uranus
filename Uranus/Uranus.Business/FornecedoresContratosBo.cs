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
    public class FornecedoresContratosBo : IRequiresSessionState
    {
        public static Int32 Inserir(FornecedoresContratos fornecedorescontratos)
        {
            using (var context = new UranusEntities())
            {
                context.FornecedoresContratos.Add(fornecedorescontratos);
                context.SaveChanges();

                return fornecedorescontratos.ID;
            }
        }

        public static void Salvar(FornecedoresContratos fornecedorescontratos)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(fornecedorescontratos).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var fornecedorescontratos = context.FornecedoresContratos.Find(id);
                    context.FornecedoresContratos.Attach(fornecedorescontratos);
                    context.FornecedoresContratos.Remove(fornecedorescontratos);
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

        public static List<FornecedoresContratos> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.FornecedoresContratos.Include("Fornecedores")
                                orderby d.Fornecedores.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.FornecedoresContratos.Include("Fornecedores")
                                where d.Fornecedores.Nome.Contains(search)
                                orderby d.Fornecedores.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static FornecedoresContratos Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresContratos
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.FornecedoresContratos.Include("Fornecedores")
                             where d.ID == Id
                             select new
                             {
                                 Id = d.ID,
                                 IdEmpresa = d.IdEmpresa,
                                 IdFornecedor = d.IdFornecedor,
                                 Nome = d.Fornecedores.Nome,
                                 Objeto = d.Objeto,
                                 DataInicio = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataInicio), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataInicio), 2) + "/" + SqlFunctions.DatePart("year", d.DataInicio).ToString(),
                                 DataFim = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataFim), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataFim), 2) + "/" + SqlFunctions.DatePart("year", d.DataFim).ToString(),
                                 Meses = d.Meses,
                                 ValorMensal = d.ValorMensal
                             }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                IdEmpresa = x.IdEmpresa,
                                IdFornecedor = x.IdFornecedor,
                                Nome = x.Nome,
                                Objeto = x.Objeto,
                                DataInicio = x.DataInicio,
                                DataFim = x.DataFim,
                                Meses = x.Meses,
                                ValorMensal = (x.ValorMensal != null ? x.ValorMensal.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                            });

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresContratos.Include("Fornecedores")
                            orderby d.Fornecedores.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Fornecedores.Nome
                            };

                return query.ToArray();
            }
        }
    }
}