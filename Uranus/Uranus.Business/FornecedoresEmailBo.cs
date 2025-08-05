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
    public class FornecedoresEmailBo : IRequiresSessionState
    {
        public static Int32 Inserir(FornecedoresEmail fornecedoresemail)
        {
            using (var context = new UranusEntities())
            {
                context.FornecedoresEmail.Add(fornecedoresemail);
                context.SaveChanges();

                return fornecedoresemail.ID;
            }
        }

        public static void Salvar(FornecedoresEmail fornecedoresemail)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(fornecedoresemail).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var fornecedoresemail = context.FornecedoresEmail.Find(id);
                    context.FornecedoresEmail.Attach(fornecedoresemail);
                    context.FornecedoresEmail.Remove(fornecedoresemail);
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

        public static List<FornecedoresEmail> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.FornecedoresEmail.Include("Fornecedores")
                                orderby d.Fornecedores.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.FornecedoresEmail.Include("Fornecedores")
                                where d.Fornecedores.Nome.Contains(search)
                                orderby d.Fornecedores.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static FornecedoresEmail Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                var query = from d in context.FornecedoresEmail
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresEmail.Include("Fornecedores")
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Fornecedores.Nome
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresEmail.Include("Fornecedores")
                            orderby d.Fornecedores.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Fornecedores.Nome
                            };

                return query.ToArray();
            }
        }

        public static Boolean Validar(Int64 IdFornecedor)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresEmail
                            where d.IDFornecedor == IdFornecedor
                            select d;

                return query.Any();
            }
        }

        public static List<FornecedoresEmail> Listar(Int64 IdFornecedor)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresEmail
                            where d.IDFornecedor == IdFornecedor
                            orderby d.Ativo descending
                            select d;

                return query.ToList();
            }
        }
    }
}