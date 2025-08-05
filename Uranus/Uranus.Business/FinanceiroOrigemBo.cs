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
    public class FinanceiroOrigemBo : IRequiresSessionState
    {
        public static Int32 Inserir(FinanceiroOrigem financeiroorigem)
        {
            using (var context = new UranusEntities())
            {
                context.FinanceiroOrigem.Add(financeiroorigem);
                context.SaveChanges();

                return financeiroorigem.Id;
            }
        }

        public static void Salvar(FinanceiroOrigem financeiroorigem)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(financeiroorigem).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var financeiroorigem = context.FinanceiroOrigem.Find(id);
                    context.FinanceiroOrigem.Attach(financeiroorigem);
                    context.FinanceiroOrigem.Remove(financeiroorigem);
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

        public static List<FinanceiroOrigem> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.FinanceiroOrigem
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.FinanceiroOrigem
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static FinanceiroOrigem Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroOrigem
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static FinanceiroOrigem ConsultarOregiemBanco(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroOrigem
                            where d.IdBanco == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static FinanceiroOrigem ConsultarOregiemSede(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroOrigem
                            where d.IdCaixa == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroOrigem.Include("Sedes")
                                                              .Include("Bancos")
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome,
                                Tipo = d.Tipo,
                                IdCaixa = d.IdCaixa,
                                CaixaNome = d.Sedes.Nome,
                                IdBanco = d.IdBanco,
                                BancoNome = d.Bancos.Nome,
                            };

                return query.ToArray();

            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroOrigem.Include("Sedes")
                                                              .Include("Bancos")
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome,
                                Tipo = d.Tipo,
                                IdCaixa = d.IdCaixa,
                                CaixaNome = d.Sedes.Nome,
                                IdBanco = d.IdBanco,
                                BancoNome = d.Bancos.Nome,
                            };

                return query.ToArray();
            }
        }
    }
}