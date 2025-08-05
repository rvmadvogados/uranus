using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class ProcessosAutoresBo : IRequiresSessionState
    {
        public static Int32 Inserir(ProcessosAutores autor)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.ProcessosAutores.Add(autor);
                    context.SaveChanges();

                    return autor.ID;
                }
            }
            catch (Exception ex)
            {
                return -80;
            }
        }

        public static void Salvar(ProcessosAutores autor)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.Entry(autor).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var autor = context.ProcessosAutores.Find(id);

                    if (autor != null)
                    {
                        context.ProcessosAutores.Attach(autor);
                        context.ProcessosAutores.Remove(autor);
                        context.SaveChanges();
                    }
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

        public static List<ProcessosAutores> Listar(Int64 IdProcesso)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAutores.Include("Clientes")
                                                              .Include("Clientes.Pessoas")
                            where d.IdProcesso == IdProcesso
                            select d;

                return query.Distinct().ToList();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAutores.Include("Clientes")
                                                              .Include("Clientes.Pessoas")
                            where d.ID == Id
                            select new
                            {
                                IdCliente = d.IdCliente,
                                Cliente = d.Clientes.Pessoas.Nome
                            };

                return query.ToArray();
            }
        }

        public static ProcessosAutores Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAutores.Include("Processos")
                                                              .Include("Processos.Profissionais")
                                                              .Include("Processos.Profissionais.Pessoas")
                                                              .Include("Clientes")
                                                              .Include("Clientes.Pessoas")
                                                              .Include("Clientes.Sedes")
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }
    }
}