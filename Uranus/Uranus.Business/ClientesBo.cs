using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class ClientesBo : IRequiresSessionState
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

                var query = (from d in context.Clientes.Include("Pessoas")
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

        public static Int32 Inserir(Clientes cliente)
        {
            using (var context = new UranusEntities())
            {
                context.Clientes.Add(cliente);
                context.SaveChanges();

                return cliente.ID;
            }
        }

        public static void Salvar(Clientes cliente)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(cliente).State = EntityState.Modified;
                context.Entry(cliente).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(cliente).Property(x => x.NomeUsuarioCadastro).IsModified = false;
                context.Entry(cliente).Property(x => x.Vinculo).IsModified = false;
                context.Entry(cliente).Property(x => x.IdIndicacaoTipo).IsModified = false;
                context.Entry(cliente).Property(x => x.Indicacao).IsModified = false;
                context.Entry(cliente).Property(x => x.IdParceiro).IsModified = false;
                context.Entry(cliente).Property(x => x.IdProfissional).IsModified = false;
                context.Entry(cliente).Property(x => x.IdCliente).IsModified = false;
                context.Entry(cliente).Property(x => x.IdVinculo).IsModified = false;
                context.Entry(cliente).Property(x => x.IdVinculoParceiro).IsModified = false;
                context.SaveChanges();

            }
        }

        public static void SalvarIndicacao(Clientes cliente)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(cliente).State = EntityState.Modified;
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

                if (message.Contains("The DELETE statement conflicted with the REFERENCE constraint") || message.Contains("An error occurred while updating the entries. See the inner exception for details."))
                    error = "98";

                return error;
            }
        }

        public static Object Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                var nome = PessoasBo.ConverteNome(search);

                var query = from d in context.fnBuscarClientes(nome)
                            orderby d.Ordem ascending, d.Nome ascending
                            select d;

                return query.Take(500).ToList();
            }
        }

        public static Object ListarFiltros(string Nome, string cpf, string fone, string email, string datanascimento)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    if (Nome == "" && cpf == "" && fone == "" && email == "" && datanascimento == "")
                    {
                        List<Clientes> clientes = new List<Clientes>();
                        return clientes;
                    }
                    else
                    {
                        context.Database.CommandTimeout = 360;

                        var nome = PessoasBo.ConverteNome(Nome);

                        var query = from d in context.fnBuscarClientesFiltros(nome, cpf, fone, email, datanascimento)
                                    orderby d.Nome ascending
                                    select d;

                        return query.Take(500).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                String error = "99";
                //String message = "Cliente não encontrado";

                return error;
            }
        }

        public static Clientes Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Clientes.Include("Pessoas")
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Clientes ConsultarIndicacao(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Clientes
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static String ConsultarNome(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Clientes.Include("Pessoas")
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault().Pessoas.Nome;
            }
        }

        public static Array ConsultarArray(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Clientes.Include("Pessoas")
                                                      .Include("Clientes")
                                                      .Include("Sedes")
                                                      .Include("ProcessosIndicacoesTipos")
                                                      .Include("Profissionais")
                                                      .Include("Profissionais.Pessoas")
                                                      .Include("Parceiros")
                                                      .Include("Profissionais")
                                                      .Include("Profissionais.Pessoas")
                                                      .Include("Parceiros")
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                IdPessoa = d.IDPessoa,
                                Nome = d.Pessoas.Nome,
                                CPFCNPJ = d.Pessoas.CpfCnpj,
                                RG = d.Pessoas.RG,
                                Profissao = d.Profissao,
                                Nacionalidade = d.Nacionalidade,
                                CEP = d.Pessoas.Cep,
                                Logradouro = d.Pessoas.Endereco,
                                Numero = d.Pessoas.Numero,
                                Complemento = d.Pessoas.Complemento,
                                Bairro = d.Pessoas.Bairro,
                                Estado = d.Pessoas.Estado,
                                Cidade = d.Pessoas.Municipio,
                                IM = d.InscricaoMunicipal,
                                IE = d.InscricaoEstadual,
                                CNAE = d.Cnae,
                                Telefone = d.Pessoas.Fones.Where(x => x.Status == "Ativo").OrderByDescending(x => x.Principal).Take(1).FirstOrDefault().Numero,
                                DataNascimento = d.Pessoas.DataNascimento,
                                EstadoCivil = d.Pessoas.EstadoCivil,
                                NomeMae = d.Pessoas.NomeMae,
                                SenhaMeuINSS = d.SenhaMeuInss,
                                Sede = d.IdSede,
                                Vinculo = d.Vinculo,
                                TipoIndicacao = d.IdIndicacaoTipo,
                                TipoIndicacaoNome = d.ProcessosIndicacoesTipos.Nome,
                                Indicacao = d.Indicacao,
                                Parceiro = d.IdParceiro,
                                ParceiroNome = d.Parceiros.Nome,
                                Profissional = d.IdProfissional,
                                ProfissionalNome = d.Profissionais.Pessoas.Nome,
                                IdCliente = d.IdCliente,
                                ClienteNome = d.Clientes2.Pessoas.Nome,
                                IdVinculo = d.IdVinculo,
                                VinculoProfissionalNome = d.Profissionais1.Pessoas.Nome,
                                IdVinculoParceiro = d.IdVinculoParceiro,
                                VinculoParceiroNome = d.Parceiros1.Nome,
                                Observacao = d.Observacao,
                                DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                UsuarioCadastro = d.NomeUsuarioCadastro,
                                DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                UsuarioAlteracao = d.NomeUsuarioAlteracao,
                                DataCadastroIndicacao = (d.DataCadastroIndicacao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastroIndicacao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastroIndicacao), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastroIndicacao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastroIndicacao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastroIndicacao), 2) : string.Empty),
                                UsuarioCadastroIndicacao = d.UsuarioCadastroIndicacao,
                                DataAlteracaoIndicacao = (d.DataAlteracaoIndicacao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracaoIndicacao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracaoIndicacao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracaoIndicacao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracaoIndicacao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracaoIndicacao), 2) : string.Empty),
                                UsuarioAlteracaoIndicacao = d.UsuarioAlteracaoIndicacao,
                                Tipo = d.ProcessosIndicacoesTipos.Tipo,
                                Cliente = d.Pessoas.Cliente,
                                Etiqueta = d.Pessoas.Etiqueta,
                                LGPDAutorizado = d.Pessoas.LGPDAutorizado
                            };

                return query.Take(500).ToArray();
            }
        }

        public static Array ListarArray()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Clientes.Include("Pessoas")
                            orderby d.Pessoas.Nome ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Pessoas.Nome
                            };

                return query.ToArray();
            }
        }

        public static Clientes Buscar(Int32 IdPessoa)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Clientes.Include("Pessoas")
                                                      .Include("Pessoas.Profissionais")
                                                      .Include("Agendas")
                                                      .Include("ProcessosAutores")
                                                      .Include("ProcessosAutores.Processos")
                                                      .Include("ProcessosAutores.Processos.ProcessosAcoes")
                                                      .Include("ProcessosPartes")
                                                      .Include("ProcessosPartes.Processos")
                                                      .Include("ProcessosPartes.Processos.ProcessosAcoes")
                            where d.IDPessoa == IdPessoa
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<Clientes> BuscarIndicacao(Int32 IdCliente)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Clientes.Include("Pessoas")
                            where d.IdCliente == IdCliente
                            select d;

                return query.ToList();
            }
        }

        public static void Vincular()
        {
            using (var context = new UranusEntities())
            {
                context.Database.ExecuteSqlCommand("stpVincularIndicacoes");
            }
        }

        public static List<Clientes> ConsultarCPFCNPJ(String CPFCNPJ, Int64 IdCliente)
        {
            using (var context = new UranusEntities())
            {
                if (IdCliente != 0)
                {
                    var query = from d in context.Clientes.Include("Pessoas")
                                where !d.ID.ToString().Contains(IdCliente.ToString())
                                where d.Pessoas.CpfCnpj == CPFCNPJ
                                select d;
                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Clientes.Include("Pessoas")
                                where d.Pessoas.CpfCnpj == CPFCNPJ
                                select d;
                    return query.ToList();
                }
            }
        }

        public static Array ConsultarEmails(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Email.Include("Pessoas")
                            where d.IDPessoa == Id
                            where d.Ativo
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Email1
                            };

                return query.ToArray();
            }
        }

        public static Email ConsultarEmailsBoletos(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Email
                            where d.IDPessoa == Id
                            where d.Ativo
                            where d.Principal
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static void AjustarVinculo(Int32 IdCliente)
        {
            using (var context = new UranusEntities())
            {
                SqlParameter param1 = new SqlParameter("@IdCliente", IdCliente);

                context.Database.ExecuteSqlCommand("stpAjustaVinculo @IdCliente", param1);
            }
        }

    }
}