using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class TiposIndicacoesBo : IRequiresSessionState
    {
        public static Int32 Inserir(ProcessosIndicacoesTipos indicacao)
        {
            using (var context = new UranusEntities())
            {
                context.ProcessosIndicacoesTipos.Add(indicacao);
                context.SaveChanges();

                return indicacao.ID;
            }
        }

        public static void Salvar(ProcessosIndicacoesTipos indicacao)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(indicacao).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var indicacao = context.ProcessosIndicacoesTipos.Find(id);
                    context.ProcessosIndicacoesTipos.Attach(indicacao);
                    context.ProcessosIndicacoesTipos.Remove(indicacao);
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

        public static List<ProcessosIndicacoesTipos> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.ProcessosIndicacoesTipos
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.ProcessosIndicacoesTipos
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static ProcessosIndicacoesTipos Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosIndicacoesTipos
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosIndicacoesTipos
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome,
                                Tipo = d.Tipo
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosIndicacoesTipos
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome,
                                Tipo = d.Tipo
                            };

                return query.ToArray();
            }
        }

    }
}