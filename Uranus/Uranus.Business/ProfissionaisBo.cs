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
    public class ProfissionaisBo : IRequiresSessionState
    {
        public class Select2Model
        {
            public string id { get; set; }
            public string text { get; set; }
        }

        public static List<Select2Model> GetProfisionaisList(string search, bool filter)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                search = PessoasBo.ConverteNome(search);

                 var query = (from d in context.Profissionais.Include("Pessoas")
                             where d.Pessoas.NomeBusca.Contains(search)
                             orderby d.Pessoas.Nome ascending
                             select new
                             {
                                 id = (filter ? d.Pessoas.Nome.Trim() : d.ID.ToString()),
                                 text = d.Pessoas.Nome
                             }).AsEnumerable().Select(B => new Select2Model()
                             {
                                 id = B.id.ToString(),
                                 text = B.text
                             });

                var xxx = query.ToList();

                return query.ToList();
            }
        }

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
                var query = (from d in context.Profissionais.Include("ProfissionaisTipos")
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
                                IdUsuario = d.IdUsuario,
                                IdCargo = d.IdCargo,
                                IdSede = d.IdSede,
                                TipoContrato = d.TipoContrato,
                                Status = d.Status,
                                Banco = d.Banco,
                                Agencia = d.Agencia,
                                Conta = d.Conta,
                                ChavePix = d.ChavePix,
                                IdArea = d.IdArea,
                                DataNascimento = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataNascimento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataNascimento), 2) + "/" + SqlFunctions.DatePart("year", d.DataNascimento).ToString(),
                                Salario = d.Salario
                            }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                IdPessoa = x.IdPessoa,
                                Nome = x.Nome,
                                CPF = x.CPF,
                                RG = x.RG,
                                IdTipoProfissional = x.IdTipoProfissional,
                                OAB = x.OAB,
                                CEP = x.CEP,
                                Logradouro = x.Logradouro,
                                Numero = x.Numero,
                                Complemento = x.Complemento,
                                Bairro = x.Bairro,
                                Estado = x.Estado,
                                Cidade = x.Cidade,
                                IdUsuario = x.IdUsuario,
                                IdCargo = x.IdCargo,
                                IdSede = x.IdSede,
                                TipoContrato = x.TipoContrato,
                                Status = x.Status,
                                Banco = x.Banco,
                                Agencia = x.Agencia,
                                Conta = x.Conta,
                                ChavePix = x.ChavePix,
                                IdArea = x.IdArea,
                                DataNascimento = x.DataNascimento,
                                Salario = (x.Salario != null ? x.Salario.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                            });

                return query.ToArray();
            }
        }

        public static Array ListarArray(String Data, int IdUsuario)
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
                                    Total = d.Agendas.Where(x => x.Data == date && x.IdCliente != null && x.Cancelou == false).Count(),
                                    IdUsuario = d.Usuarios.ID
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
                                    Total = d.Agendas.Where(x => x.Cancelou == false).Count(),
                                    IdUsuario = d.Usuarios.ID
                                };

                    if (IdUsuario > 0)
                    {
                        query = query.Where(x => x.IdUsuario == IdUsuario);
                        //query.Where(x => x.IdUsuario == IdUsuario);
                    }

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

        public static Profissionais BuscarEmailProfissional(Int32 Id)
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


        public static Profissionais ConsultarNome(string Nome)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Profissionais.Include("Pessoas")
                            where d.Pessoas.Nome.Contains(Nome)
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<Profissionais> ListarAniversariantes(Int32 dia, Int32 Mes)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Profissionais.Include("ProfissionaisContratos")
                            where d.DataNascimento.HasValue && d.DataNascimento.Value.Day >= dia && d.DataNascimento.Value.Month == Mes
                            orderby d.DataNascimento
                            select d;


                return query.ToList();
            }
        }

        public static List<Profissionais> ListarAniversariantesCasa(Int32 dia, Int32 Mes)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Profissionais.Include("Pessoas")
                            where d.DataNascimento.HasValue && d.DataNascimento.Value.Day >= dia && d.DataNascimento.Value.Month == Mes
                            orderby d.DataNascimento
                            select d;


                return query.ToList();
            }
        }

        public static List<Profissionais> ListarColaboradores(Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Profissionais.Include("Pessoas")
                                                           
                            where d.ID != IdProfissional
                            select d;


                return query.ToList();
            }
        }

    }
}