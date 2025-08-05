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
    public class FornecedoresFonesBo : IRequiresSessionState
    {
        public static Int32 Inserir(FornecedoresFones fornecedoresfones)
        {
            using (var context = new UranusEntities())
            {
                context.FornecedoresFones.Add(fornecedoresfones);
                context.SaveChanges();

                return fornecedoresfones.ID;
            }
        }

        public static void Salvar(FornecedoresFones fornecedoresfones)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(fornecedoresfones).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var fornecedoresfones = context.FornecedoresFones.Find(id);
                    context.FornecedoresFones.Attach(fornecedoresfones);
                    context.FornecedoresFones.Remove(fornecedoresfones);
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

        public static List<FornecedoresFones> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.FornecedoresFones.Include("Fornecedores")
                                orderby d.Fornecedores.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.FornecedoresFones.Include("Fornecedores")
                                where d.Fornecedores.Nome.Contains(search)
                                orderby d.Fornecedores.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static FornecedoresFones Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresFones
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresFones.Include("Fornecedores")
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Fornecedores.Nome
                            };

                return query.ToArray();
            }
        }

        public static List<FornecedoresFones> Listar(Int64 IdFornecedor)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresFones
                            where d.IdFornecedor == IdFornecedor
                            orderby d.Status descending
                            select d;

                return query.ToList();
            }
        }

        public static Boolean Validar(Int64 IdFornecedor)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresFones
                            where d.IdFornecedor == IdFornecedor
                            select d;

                return query.Any();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresFones.Include("Fornecedores")
                            orderby d.Fornecedores.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Fornecedores.Nome
                            };

                return query.ToArray();
            }
        }

        public static String Buscar(Int32 IdFornecedor)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresFones
                            where d.IdFornecedor == IdFornecedor
                            where d.Status == "Ativo"
                            select d;

                return query.FirstOrDefault()?.Numero ?? String.Empty;
            }
        }
    }
}