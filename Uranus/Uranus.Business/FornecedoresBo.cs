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
    public class FornecedoresBo : IRequiresSessionState
    {
        public class Select2Model
        {
            public string id { get; set; }
            public string text { get; set; }
        }

        public static List<Select2Model> GetProviderList(string search, bool filter)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                var query = (from d in context.Fornecedores
                             where d.Nome.Contains(search)
                             orderby d.Nome ascending
                             select new
                             {
                                 id = (filter ? d.Nome.Trim() : d.ID.ToString()),
                                 text = d.Nome
                             }).AsEnumerable().Select(B => new Select2Model()
                             {
                                 id = B.id.ToString(),
                                 text = B.text
                             }).ToList();

                return query;
            }
        }

        public static Int32 Inserir(Fornecedores fornecedores)
        {
            using (var context = new UranusEntities())
            {
                context.Fornecedores.Add(fornecedores);
                context.SaveChanges();

                return fornecedores.ID;
            }
        }

        public static void Salvar(Fornecedores fornecedores)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(fornecedores).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var fornecedores = context.Fornecedores.Find(id);
                    context.Fornecedores.Attach(fornecedores);
                    context.Fornecedores.Remove(fornecedores);
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

        public static List<Fornecedores> Listar(string Nome)
        {
            using (var context = new UranusEntities())
            {
                var query = context.Fornecedores
                                   .AsQueryable();

                query = query.Where(x => x.Nome.Contains(Nome));
                query = query.AsQueryable().OrderBy(x => x.Nome);

                return query.ToList();
            }
        }

        public static Fornecedores Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                var query = from d in context.Fornecedores
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Fornecedores Buscar(String CNPJ)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Fornecedores
                            where d.CpfCnpj == CNPJ
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.Fornecedores
                             where d.ID == Id
                             select d).ToList().Select(x => new
                             {
                                 Id = x.ID,
                                 TipoPessoa = x.TipoPessoa,
                                 CPFCNPJ = x.CpfCnpj,
                                 Nome = x.Nome,
                                 Ativo = x.Ativo,
                                 RG = x.RG,
                                 IE = x.InscricaoEstadual,
                                 IM = x.InscricaoMunicipal,
                                 CNAE = x.Cnae,
                                 Vendedor = x.Vendedor,
                                 LimiteCredito = (x.LimiteCredito != null ? x.LimiteCredito.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                 Estrangeiro = x.Estrangeiro,
                                 CEP = x.Cep,
                                 Logradouro = x.Endereco,
                                 Numero = x.Numero,
                                 Complemento = x.Complemento,
                                 Bairro = x.Bairro,
                                 Cidade = x.Municipio,
                                 Estado = x.Estado

                             });

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Fornecedores
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Nome
                            };

                return query.ToArray();
            }
        }

        public static List<Select2Model> GetFornecedoresListRel(string search)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                var query = (from d in context.Fornecedores
                             where d.Nome.Contains(search)
                             orderby d.Nome ascending
                             select new
                             {
                                 id = d.ID.ToString(),
                                 text = d.Nome
                             }).AsEnumerable().Select(B => new Select2Model()
                             {
                                 id = string.Format("{0} - {1}", B.id, B.text),
                                 text = string.Format("{0} - {1}", B.id, B.text)
                             }).ToList();

                return query;
            }
        }

        public static Array ConsultarEnderecos(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Fornecedores
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                Nome = (d.Endereco + " " + d.Numero + " " + d.Complemento + " " + d.Bairro + " " + d.Municipio + " " + d.Estado)
                            };

                return query.ToArray();
            }
        }

        public static Array ConsultarEmails(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FornecedoresEmail
                            where d.IDFornecedor == Id
                            where d.Ativo
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Email
                            };

                return query.ToArray();
            }
        }
    }
}