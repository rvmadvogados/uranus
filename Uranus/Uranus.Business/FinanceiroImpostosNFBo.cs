using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class FinanceiroImpostosNFBo : IRequiresSessionState
    {
        public static Int32 Inserir(FinanceiroImpostosNF financeiroimpostosnf)
        {
            using (var context = new UranusEntities())
            {
                context.FinanceiroImpostosNF.Add(financeiroimpostosnf);
                context.SaveChanges();

                return financeiroimpostosnf.Id;
            }
        }

        public static void Salvar(FinanceiroImpostosNF financeiroimpostosnf)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(financeiroimpostosnf).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var financeiroimpostosnf = context.FinanceiroImpostosNF.Find(id);
                    context.FinanceiroImpostosNF.Attach(financeiroimpostosnf);
                    context.FinanceiroImpostosNF.Remove(financeiroimpostosnf);
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

        public static List<FinanceiroImpostosNF> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.FinanceiroImpostosNF
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.FinanceiroImpostosNF
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static FinanceiroImpostosNF Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroImpostosNF
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.FinanceiroImpostosNF
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome,
                                Valor = d.Valor
                            }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                Nome = x.Nome,
                                Valor = (x.Valor != null ? x.Valor.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00")
                            });

                return query.ToArray();

            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroImpostosNF
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome,
                                Valor = d.Valor
                            };

                return query.ToArray();
            }
        }
    }
}