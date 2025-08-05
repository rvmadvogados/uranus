using System;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;
using System.Collections.Generic;
using System.Data;

namespace Uranus.Business
{
    public class PessoasBo : IRequiresSessionState
    {

        public class Select2Model
        {
            public string id { get; set; }
            public string text { get; set; }
        }

        public static List<Select2Model> GetClientsList(string search, bool filter)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                search = PessoasBo.ConverteNome(search);

                var query = (from d in context.Pessoas
                             where d.NomeBusca.Contains(search)
                             orderby d.Nome ascending
                             select new
                             {
                                 id = (filter ? d.Nome.Trim() : d.ID.ToString()),
                                 text = d.Nome
                             }).AsEnumerable().Select(B => new Select2Model()
                             {
                                 id = B.id.ToString(),
                                 text = B.text
                             });

                var xxx = query.ToList();

                return query.ToList();
            }
        }

        public static Int32 Inserir(Pessoas pessoa)
        {
            using (var context = new UranusEntities())
            {
                context.Pessoas.Add(pessoa);
                context.SaveChanges();

                return pessoa.ID;
            }
        }

        public static void Salvar(Pessoas pessoa)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(pessoa).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static Pessoas Consultar(String Nome)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Pessoas.Include("Clientes")
                            where d.Nome == Nome
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static String ConverteNome(String Nome)
        {
            using (var context = new UranusEntities())
            {
                Nome = Nome.Replace("'", String.Empty);

                return context.Database.SqlQuery<String>("SELECT [dbo].[fnRetornarSomenteCaracteres]('" + Nome + "')").FirstOrDefault();
            }
        }

        public static Clientes ConsultarPessoa(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Clientes.Include("Pessoas")
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Fones BuscarForne(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Fones
                            where d.IDPessoa == Id
                            where d.Principal == true
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Email ConsultarEmail(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                var query = from d in context.Email
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }
    }
}