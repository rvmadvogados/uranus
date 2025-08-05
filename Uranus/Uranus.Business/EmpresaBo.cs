using System;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class EmpresaBo : IRequiresSessionState
    {
        public static void Salvar(Empresa empresa)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(empresa).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static Array ConsultarArray()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Empresa
                            select new
                            {
                                Id = d.ID,
                                CNPJ = d.Cnpj,
                                Nome = d.Nome,
                                CEP = d.Cep,
                                Logradouro = d.Endereco,
                                Numero = d.Numero,
                                Complemento = d.Complemento,
                                Bairro = d.Bairro,
                                UF = d.Estado,
                                Cidade = d.Cidade,
                                Telefone = d.Fone,
                                Email = d.Email,
                                Senha = d.Senha
                            };

                return query.Take(1).ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Empresa
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome,
                            };

                return query.ToArray();
            }
        }

        public static Empresa Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Empresa
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Empresa Consultar(Int32? Id)
        {
            using (var context = new UranusEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                var query = from d in context.Empresa
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

    }
}