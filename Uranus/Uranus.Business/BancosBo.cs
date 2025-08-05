using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class BancosBo : IRequiresSessionState
    {
        public static Int32 Inserir(Bancos bancos)
        {
            using (var context = new UranusEntities())
            {
                context.Bancos.Add(bancos);
                context.SaveChanges();

                return bancos.ID;
            }
        }

        public static void Salvar(Bancos bancos)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(bancos).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var bancos = context.Bancos.Find(id);
                    context.Bancos.Attach(bancos);
                    context.Bancos.Remove(bancos);
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

        public static List<Bancos> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.Bancos
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Bancos
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static Bancos Consultar(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Bancos
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Bancos
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome,
                                NumeroBanco = d.NumeroBanco,
                                Agencia = d.Agencia,
                                Conta = d.Conta,
                                Convenio = d.Convenio,
                                Carteira = d.Carteira
                            };

                return query.ToArray();
            }
        }

        public static Array ConsultarBanco()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Bancos
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome,
                            };

                return query.ToArray();
            }
        }

        public static Bancos Buscar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Bancos
                            select d;

                return query.FirstOrDefault();
            }
        }
    }
}