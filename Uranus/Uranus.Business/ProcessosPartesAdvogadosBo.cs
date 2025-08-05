using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class ProcessosPartesAdvogadosBo : IRequiresSessionState
    {
        public static Int32 Inserir(ProcessosPartesAdvogados advogado)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.ProcessosPartesAdvogados.Add(advogado);
                    context.SaveChanges();

                    return advogado.ID;
                }
            }
            catch (Exception ex)
            {
                return -80;
            }
        }

        public static void Salvar(ProcessosPartesAdvogados advogado)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.Entry(advogado).State = EntityState.Modified;
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
                    var advogado = context.ProcessosPartesAdvogados.Find(id);

                    if (advogado != null)
                    {
                        context.ProcessosPartesAdvogados.Attach(advogado);
                        context.ProcessosPartesAdvogados.Remove(advogado);
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

        public static List<ProcessosPartes> Listar(Int64 IdProcesso)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosPartes.Include("Clientes")
                                                             .Include("Clientes.Pessoas")
                            where d.IdProcesso == IdProcesso
                            select d;

                return query.ToList();
            }
        }

        public static ProcessosPartesAdvogados Consultar(Int32 Id, Int32 IdProcesso)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosPartesAdvogados.Include("ProcessosPartes")
                                                                      .Include("ProcessosPartes.Processos")
                                                                      .Include("ProcessosPartes.Processos.Profissionais")
                                                                      .Include("ProcessosPartes.Processos.Profissionais.Pessoas")
                                                                      .Include("ProcessosPartes.Clientes")
                                                                      .Include("ProcessosPartes.Clientes.Sedes")
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array Consultar(Int64 IdProcesso)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosPartesAdvogados.Include("ProcessosPartes")
                            where d.ProcessosPartes.IdProcesso == IdProcesso
                            orderby d.NomeAdvogado ascending
                            select new
                            {
                                Nome = d.NomeAdvogado
                            };

                return query.Distinct().ToArray();
            }
        }

        public static Array Buscar(Int32 IdProcesso, String NomeAdvogado)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosPartesAdvogados.Include("ProcessosPartes")
                            where d.ProcessosPartes.IdProcesso == IdProcesso
                            where d.NomeAdvogado == NomeAdvogado
                            select new
                            {
                                NumeroOab = d.NumeroOab,
                                TelefoneComercial = d.TelefoneComercial,
                                Celular = d.Celular,
                                Email = d.Email
                            };

                return query.Take(1).ToArray();
            }
        }
    }
}