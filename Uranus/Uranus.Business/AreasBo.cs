using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class AreasBo : IRequiresSessionState
    {
        public static Int32 Inserir(ProcessosAreas area)
        {
            using (var context = new UranusEntities())
            {
                context.ProcessosAreas.Add(area);
                context.SaveChanges();

                return area.ID;
            }
        }

        public static void Salvar(ProcessosAreas area)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(area).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var area = context.ProcessosAreas.Find(id);
                    context.ProcessosAreas.Attach(area);
                    context.ProcessosAreas.Remove(area);
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

        public static List<ProcessosAreas> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                var nome = PessoasBo.ConverteNome(search);

                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.ProcessosAreas
                                orderby d.AreaAtuacao ascending
                                select d;

                    return query.Distinct().ToList();
                }
                else
                {
                    var query = from d in context.ProcessosAreas
                                where d.NomeBusca.Contains(nome)
                                orderby d.AreaAtuacao ascending
                                select d;

                    return query.Distinct().ToList();
                }
            }
        }

        public static ProcessosAreas Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAreas
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAreas
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                Nome = d.AreaAtuacao
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAreas
                            orderby d.AreaAtuacao ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.AreaAtuacao
                            };

                return query.ToArray();
            }
        }

        public static ProcessosAreas ConsultarNome(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAreas
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

    }
}