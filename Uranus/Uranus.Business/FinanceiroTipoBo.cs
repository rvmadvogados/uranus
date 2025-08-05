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
    public class FinanceiroTipoBo : IRequiresSessionState
    {
        public static Int32 Inserir(FinanceiroTipo financeirotipo)
        {
            using (var context = new UranusEntities())
            {
                context.FinanceiroTipo.Add(financeirotipo);
                context.SaveChanges();

                return financeirotipo.Id;
            }
        }

        public static void Salvar(FinanceiroTipo financeirotipo)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(financeirotipo).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var financeirotipo = context.FinanceiroTipo.Find(id);
                    context.FinanceiroTipo.Attach(financeirotipo);
                    context.FinanceiroTipo.Remove(financeirotipo);
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

        public static List<FinanceiroTipo> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.FinanceiroTipo
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.FinanceiroTipo
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static FinanceiroTipo Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroTipo
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroTipo
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome,
                                Tipo = d.Tipo
                            };

                return query.ToArray();

            }
        }

        public static Array Consultar(string Tipo)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FinanceiroTipo
                            where d.Tipo == Tipo
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome,
                                Tipo = d.Tipo
                            };

                return query.ToArray();
            }
        }
    }
}