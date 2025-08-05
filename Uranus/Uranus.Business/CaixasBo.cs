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
    public class CaixasBo : IRequiresSessionState
    {
        public static Int64 Inserir(Caixas caixa)
        {
            using (var context = new UranusEntities())
            {
                context.Caixas.Add(caixa);
                context.SaveChanges();

                return caixa.ID;
            }
        }

        public static void Salvar(Caixas caixa)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(caixa).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var caixa = context.Caixas.Find(id);
                    context.Caixas.Attach(caixa);
                    context.Caixas.Remove(caixa);
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

        //public static List<Caixa> Listar(string search)
        //{
        //    using (var context = new UranusEntities())
        //    {
        //        if (String.IsNullOrEmpty(search.Trim()))
        //        {
        //            var query = from d in context.Caixas
        //                        orderby d.Data ascending
        //                        select d;

        //            return query.ToList();
        //        }
        //        else
        //        {
        //            var query = from d in context.Caixas
        //                        where d.Data.Contains(search)
        //                        orderby d.Data ascending
        //                        select d;

        //            return query.ToList();
        //        }
        //    }
        //}

        public static Caixas Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Caixas
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            {
                using (var context = new UranusEntities())
                {
                    var query = (from d in context.Caixas
                                 where d.ID == Id
                                 select new
                                 {
                                     Id = d.ID,
                                     IdOrigem = d.IdOrigem,
                                     Data = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Data), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Data), 2) + "/" + SqlFunctions.DatePart("year", d.Data).ToString(),
                                     IdHistorico = d.IdHistorico,
                                     Descricao = d.Descricao,
                                     ValorDebito = d.ValorDebito,
                                     ValorCredito = d.ValorCredito
                                 }).ToList()
                                 .Select(x => new
                                 {
                                     Id = x.Id,
                                     IdOrigem = x.IdOrigem,
                                     Data = x.Data,
                                     IdHistorico = x.IdHistorico,
                                     ValorDebito = (x.ValorDebito != null ? x.ValorDebito.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                     ValorCredito = (x.ValorCredito != null ? x.ValorCredito.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                 });

                    return query.ToArray();

                }
            }
        }
        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Caixas
                            orderby d.Data ascending
                            select new
                            {
                                Id = d.ID,
                                DataLancamento = d.Data
                            };

                return query.ToArray();
            }
        }
    }
}