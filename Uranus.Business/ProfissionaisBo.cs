using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class ProfissionaisBo : IRequiresSessionState
    {
        public static Int32 Inserir(Profissionais profissional)
        {
            using (var context = new UranusEntities())
            {
                context.Profissionais.Add(profissional);
                context.SaveChanges();

                return profissional.ID;
            }
        }

        public static void Salvar(Profissionais profissional)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(profissional).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var pessoa = context.Pessoas.Find(id);
                    context.Pessoas.Attach(pessoa);
                    context.Pessoas.Remove(pessoa);
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

        public static List<Profissionais> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.Profissionais.Include("ProfissionaisTipos")
                                                               .Include("Pessoas")
                                                               .Include("Usuarios")
                                orderby d.Usuarios.Bloqueio ascending, d.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Profissionais.Include("ProfissionaisTipos")
                                                               .Include("Pessoas")
                                                               .Include("Usuarios")
                                where d.Pessoas.Nome.Contains(search)
                                orderby d.Usuarios.Bloqueio ascending, d.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static List<Profissionais> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Profissionais
                            where d.IDTipoProfissional.ToString().Length > 0
                            orderby d.Pessoas.Nome ascending
                            select d;

                return query.ToList();
            }
        }

        public static Profissionais Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Profissionais.Include("Pessoas")
                                                           .Include("Pessoas.Email")
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Profissionais.Include("ProfissionaisTipos")
                                                           .Include("Pessoas")
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                IdPessoa = d.IDPessoa,
                                Nome = d.Pessoas.Nome,
                                CPF = d.Pessoas.CpfCnpj,
                                RG = d.Pessoas.RG,
                                IdTipoProfissional = d.IDTipoProfissional,
                                OAB = d.OAB,
                                CEP = d.Pessoas.Cep,
                                Logradouro = d.Pessoas.Endereco,
                                Numero = d.Pessoas.Numero,
                                Complemento = d.Pessoas.Complemento,
                                Bairro = d.Pessoas.Bairro,
                                Estado = d.Pessoas.Estado,
                                Cidade = d.Pessoas.Municipio,
                                IdUsuario = d.IdUsuario
                            };

                return query.ToArray();
            }
        }

        public static Array ListarArray(String Data)
        {
            if (!String.IsNullOrEmpty(Data) && Data.Length == 10)
            {
                var date = DateTime.Parse(Data);

                using (var context = new UranusEntities())
                {
                    var query = from d in context.Profissionais.Include("Pessoas")
                                                               .Include("Pessoas.Agendas")
                                                               .Include("Usuarios")
                                where d.Usuarios.Bloqueio == false
                                orderby d.Pessoas.Nome ascending
                                select new
                                {
                                    Id = d.ID,
                                    Nome = d.Pessoas.Nome,
                                    Total = d.Agendas.Where(x => x.Data == date && x.IdCliente != null && x.Cancelou == false).Count()
                                };

                    return query.ToArray();
                }
            }
            else
            {
                using (var context = new UranusEntities())
                {
                    var query = from d in context.Profissionais.Include("Pessoas")
                                                               .Include("Pessoas.Agendas")
                                                               .Include("Usuarios")
                                where d.Usuarios.Bloqueio == false
                                orderby d.Pessoas.Nome ascending
                                select new
                                {
                                    Id = d.ID,
                                    Nome = d.Pessoas.Nome,
                                    Total = d.Agendas.Where(x => x.Cancelou == false).Count()
                                };

                    return query.ToArray();
                }
            }
        }

        public static Profissionais Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Profissionais.Include("Pessoas")
                                                           .Include("Pessoas.Email")
                            where d.IdUsuario == Id
                            select d;

                return query.FirstOrDefault();
            }
        }
    }
}