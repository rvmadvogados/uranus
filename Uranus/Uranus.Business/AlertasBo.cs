using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class AlertasBo : IRequiresSessionState
    {
        public static Int64 Inserir(Alertas alerta)
        {
            using (var context = new UranusEntities())
            {
                context.Alertas.Add(alerta);
                context.SaveChanges();

                return alerta.Id;
            }
        }

        public static void Salvar(Alertas alerta)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(alerta).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int64 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var alerta = context.Alertas.Find(id);
                    context.Alertas.Attach(alerta);
                    context.Alertas.Remove(alerta);
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

        public static List<Alertas> Listar(Int32? IdAcao, Int32? IdPessoa)
        {
            using (var context = new UranusEntities())
            {
                var query = context.Alertas
                                   .Include("Usuarios")
                                   .AsQueryable();


                if (IdAcao.HasValue)
                {
                    query = query.Where(x => x.IdAcao == IdAcao.Value);
                }

                if (IdPessoa.HasValue)
                {
                    query = query.Where(x => x.IdPessoa == IdPessoa.Value);
                }

                query = query.Where(x => x.Lido == false);
                query = query.AsQueryable().OrderBy(x => x.DataHora);

                return query.ToList();
            }
        }

        public static Alertas Consultar(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Alertas
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Alertas.Include("Usuarios")
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                DataHora = d.DataHora,
                                Usuario = d.Usuarios.Nome,
                                Mensagem = d.Mensagem,
                                Lido = d.Lido
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Alertas
                            orderby d.DataHora ascending
                            select new
                            {
                                Id = d.Id,
                                DataHora = d.DataHora,
                                Usuario = d.Usuarios.Nome,
                                Mensagem = d.Mensagem,
                                Lido = d.Lido
                            };

                return query.ToArray();
            }
        }
    }
}