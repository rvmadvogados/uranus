using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;
using Uranus.Domain.Entities;

namespace Uranus.Business
{
    public class ProcessosAcoesBo : IRequiresSessionState
    {
        public class Select2Model
        {
            public string id { get; set; }
            public string text { get; set; }
        }

        public static List<Select2Model> GetProcessList(string search, bool filter)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                var query = (from d in context.ProcessosAcoes
                             where d.NumeroProcesso.Contains(search)
                             //   ruy                           where d.NumeroProcesso.Trim().Length > 0
                             orderby d.NumeroProcesso.Trim() ascending
                             select new
                             {
                                 id = (filter ? d.NumeroProcesso.Trim() : d.ID.ToString()),
                                 text = d.NumeroProcesso.Trim()
                             }).AsEnumerable().Select(B => new Select2Model()
                             {
                                 id = B.id.ToString(),
                                 text = B.text
                             }).ToList();

                return query;
            }
        }

        public static Int32 Inserir(ProcessosAcoes acao)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.ProcessosAcoes.Add(acao);
                    context.SaveChanges();

                    return acao.ID;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.InnerException.Message.Contains("Index_Agenda_Data_Hora_Profissional"))
                {
                    return -90;
                }
                else
                {
                    return -80;
                }
            }
        }

        public static void Salvar(ProcessosAcoes acao)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.Entry(acao).State = EntityState.Modified;
                    context.Entry(acao).Property(x => x.DataAcao).IsModified = false;
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
                    var acao = context.ProcessosAcoes.Find(id);

                    if (acao != null)
                    {
                        context.ProcessosAcoes.Attach(acao);
                        context.ProcessosAcoes.Remove(acao);
                        context.SaveChanges();
                    }
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

        public static List<ProcessosAcoes> Listar(Int64 IdProcesso)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoes
                            where d.IdProcesso == IdProcesso
                            select d;

                return query.ToList();
            }
        }

        public static List<ProcessosAcoes> ListarProcessos(Int64 IdProcesso)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoes
                            where d.IdProcesso == IdProcesso
                            select d;


                return query.ToList();
            }
        }


        public static List<ProcessosAcoes> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoes
                            where d.EventoAutomatico == true
                            orderby d.NumeroProcesso ascending
                            select d;

                return query.ToList();
            }
        }

        public static List<View_EventosAutomaticos> ListarEventosAutomaticos()
        {

            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                var query = from d in context.View_EventosAutomaticos
                            orderby d.NumeroProcesso ascending
                            select d;

                return query.ToList();
            }

        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Processos.Include("Profissionais")
                                                       .Include("Profissionais.Pessoas")
                                                       .Include("ProcessosAutores")
                                                       .Include("ProcessosAutores.Clientes")
                                                       .Include("ProcessosAutores.Clientes.Sedes")
                            where d.ID == Id
                            select new
                            {
                                DataHora = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataInclusao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataInclusao), 2) + "/" + SqlFunctions.DatePart("year", d.DataInclusao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataInclusao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataInclusao), 2),
                                IdSede = d.ProcessosAutores.FirstOrDefault().Clientes.IdSede,
                                IdProfissional = d.IdProfissionalResponsavel,
                                Status = d.Status,
                                Objeto = d.Objeto,
                                Observacao = d.Observacao,
                                Sede = d.ProcessosAutores.FirstOrDefault().Clientes.Sedes.Nome,
                                Profissional = d.Profissionais.Pessoas.Nome
                            };

                return query.ToArray();
            }
        }

        public static ProcessosAcoes Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoes.Include("Processos")
                                                            .Include("Processos.Profissionais")
                                                            .Include("Processos.Profissionais.Pessoas")
                                                            .Include("ProcessosCadastroAcao")
                                                            .Include("ProcessosVara")
                                                            .Include("ProcessosAreas")
                                                            .Include("Processos.ProcessosAutores")
                                                            .Include("Processos.ProcessosAutores.Clientes")
                                                            .Include("Processos.ProcessosAutores.Clientes.Sedes")

                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static ProcessosAcoes BuscarNome(String numeroProcesso)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoes.Include("Processos")
                                                            .Include("Processos.ProcessosAutores")
                                                            .Include("Processos.ProcessosAutores.Clientes")
                                                            .Include("Processos.ProcessosAutores.Clientes.Pessoas")
                                                            .Include("Processos.ProcessosPartes")
                                                            .Include("Processos.ProcessosPartes.Clientes")
                                                            .Include("Processos.ProcessosPartes.Clientes.Pessoas")

                            where d.NumeroProcesso == numeroProcesso
                            select d;

                return query.FirstOrDefault();
            }
        }


        public static string BuscarNomeCliente(Int64 IdAcao, string Tipo)
        {
            using (var context = new UranusEntities())
            {
                if (Tipo == "A")
                {
                    var query = context.Database
                        .SqlQuery<string>($"SELECT TOP 1 D.Nome FROM ProcessosAcoes A INNER JOIN ProcessosAutores B ON a.IdProcesso = b.IdProcesso INNER JOIN Clientes C ON B.IdCliente = C.Id INNER JOIN Pessoas D ON C.IDPessoa = D.Id WHERE A.Id = " + IdAcao)
                        .FirstOrDefault();

                    return query ?? string.Empty;
                }
                else
                {
                    var query = context.Database
                        .SqlQuery<string>($"SELECT TOP 1 D.Nome FROM ProcessosAcoes A INNER JOIN ProcessosPartes B ON a.IdProcesso = b.IdProcesso INNER JOIN Clientes C ON B.IdCliente = C.Id INNER JOIN Pessoas D ON C.IDPessoa = D.Id WHERE A.Id = " + IdAcao)
                        .FirstOrDefault();

                    return query ?? string.Empty;
                }
            }
        }

        public static Array Listar(Int32 IdProcesso)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                if (IdProcesso > 0)
                {
                    var query = (from d in context.ProcessosAcoes
                                 where d.IdProcesso == IdProcesso
                                 orderby d.NumeroProcesso.Trim() ascending
                                 select new
                                 {
                                     Id = d.ID,
                                     Nome = d.NumeroProcesso.Trim(),
                                     Status = d.Status
                                 }).Distinct();

                    return query.Take(500).ToArray();
                }
                else
                {
                    var query = (from d in context.ProcessosAcoes
                                 where d.NumeroProcesso.Trim().Length > 0
                                 orderby d.NumeroProcesso.Trim() ascending
                                 select new
                                 {
                                     Id = d.ID,
                                     Nome = d.NumeroProcesso.Trim(),
                                     NumeroProcesso = d.NumeroProcesso.Trim(),
                                     Status = d.Status
                                 }).Distinct();

                    return query.Take(500).ToArray();
                }
            }
        }

        public static Array ListarArray()
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                var query = (from d in context.ProcessosAcoes
                             where d.NumeroProcesso.Trim().Length > 0
                             //where SqlFunctions.IsNumeric(d.NumeroProcesso.Trim()) == 1
                             select new
                             {
                                 NumeroProcesso = d.NumeroProcesso.Trim()
                             }).Distinct();

                return query.OrderBy(d => d.NumeroProcesso.Trim()).ToArray();
            }
        }

        public static Array ListarArrayFinanceiro(Int32 IdCliente)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                var query = (from d in context.ProcessosAcoes.Include("Processos")
                                                             .Include("Processos.ProcessosAutores")
                             where d.NumeroProcesso.Trim().Length > 0
                             where d.Processos.ProcessosAutores.Where(x => x.IdCliente == IdCliente).FirstOrDefault().IdCliente == IdCliente
                             select new
                             {
                                 Id = d.ID,
                                 NumeroProcesso = d.NumeroProcesso.Trim()
                             }).Distinct();

                return query.OrderBy(d => d.NumeroProcesso.Trim()).ToArray();
            }
        }

        public static ProcessosAcoes Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoes
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Int32 BuscarId(Int32 IdProcesso)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoes
                            where d.IdProcesso == IdProcesso
                            select d;

                return query.FirstOrDefault()?.ID ?? 0;
            }
        }

        public static Int32 BuscarIdAcao(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoes
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault()?.ID ?? 0;
            }
        }

        public static List<ProcessosAcoes> ConsultarDublicado(Int32 IdProcesso, String NumeroProcesso)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoes
                            where d.NumeroProcesso == NumeroProcesso
                            where d.IdProcesso != IdProcesso
                            select d;

                return query.ToList();
            }
        }

        public static ProcessosAcoes ConsultarNumeroProcesso(String NumeroProcesso)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoes
                            where d.NumeroProcesso == NumeroProcesso
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ListarProcessosCliente(String Cliente)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                var nome = PessoasBo.ConverteNome(Cliente);

                var query = from d in context.View_Processos_Listagem
                            where d.NomeBusca.Contains(nome)
                            select new
                            {
                                Id = d.IdAcao,
                                Nome = d.NumeroProcesso,
                            };

                return query.ToArray();
            }
        }

        public static ProcessosAcoes ConsultarNumeroProcessoContrato(Int32 IdCliente, String NumeroProcesso)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoes.Include("Processos")
                                                            .Include("Processos.ProcessosAutores")
                            where d.NumeroProcesso == NumeroProcesso
                            where d.Processos.ProcessosAutores.Where(x => x.IdCliente == IdCliente).FirstOrDefault().IdCliente == IdCliente
                            select d;

                return query.FirstOrDefault();
            }
        }

    }
}