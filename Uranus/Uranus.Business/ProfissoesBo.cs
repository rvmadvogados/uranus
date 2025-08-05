using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class ProfissoesBo : IRequiresSessionState
    {
        public static Int32 Inserir(Profissoes profissao)
        {
            using (var context = new UranusEntities())
            {
                context.Profissoes.Add(profissao);
                context.SaveChanges();

                return profissao.Id;
            }
        }

        public static void Salvar(Profissoes profissao)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(profissao).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var profissao = context.Profissoes.Find(id);
                    context.Profissoes.Attach(profissao);
                    context.Profissoes.Remove(profissao);
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

        public static List<Profissoes> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.Profissoes
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Profissoes
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static Profissoes Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Profissoes
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Profissoes
                            where d.Id == Id
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Profissoes
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome
                            };

                return query.ToArray();
            }
        }
        public static Array ConsultarClientes()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Profissoes
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome,
                            };

                return query.ToArray();
            }
        }
    }
}