using System;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class ProcessosPartesBo : IRequiresSessionState
    {
        public static Int32 Inserir(ProcessosPartes parte)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.ProcessosPartes.Add(parte);
                    context.SaveChanges();

                    return parte.ID;
                }
            }
            catch (Exception ex)
            {
                return -80;
            }
        }

        public static void Salvar(ProcessosPartes parte)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.Entry(parte).State = EntityState.Modified;
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
                    var parte = context.ProcessosPartes.Find(id);
                    context.ProcessosPartes.Attach(parte);
                    context.ProcessosPartes.Remove(parte);
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

        public static Array Consultar(Int64 IdProcesso)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosPartes.Include("Clientes")
                            where d.IdProcesso == IdProcesso
                            orderby d.NomeParte ascending
                            select new
                            {
                                Id = d.IdCliente,
                                Nome = d.Clientes.Pessoas.Nome
                            };

                return query.ToArray();
            }
        }

        public static ProcessosPartes Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosPartes
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static ProcessosPartes BuscarIdProcessos(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosPartes
                            where d.IdProcesso == Id
                            select d;

                return query.FirstOrDefault();
            }
        }
    }
}