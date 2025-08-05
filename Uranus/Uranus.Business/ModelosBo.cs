using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class ModelosBo : IRequiresSessionState
    {
        public static Int32 Inserir(Modelos modelo)
        {
            using (var context = new UranusEntities())
            {
                context.Modelos.Add(modelo);
                context.SaveChanges();

                return modelo.Id;
            }
        }

        public static void Salvar(Modelos modelo)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(modelo).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var modelo = context.Modelos.Find(id);
                    context.Modelos.Attach(modelo);
                    context.Modelos.Remove(modelo);
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

        public static List<Modelos> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.Modelos
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Modelos
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static Modelos Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Modelos
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Modelos
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome,
                                Modelo = d.Modelo
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Modelos
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome
                            };

                return query.ToArray();
            }
        }
    }
}