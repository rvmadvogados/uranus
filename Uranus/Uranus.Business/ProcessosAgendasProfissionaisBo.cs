using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class ProcessosAgendasProfissionaisBo : IRequiresSessionState
    {
        public static Int32 Inserir(ProcessosAgendaProfissional agenda)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.ProcessosAgendaProfissional.Add(agenda);
                    context.SaveChanges();

                    return agenda.ID;
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

        public static void Salvar(ProcessosAgendaProfissional agenda)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.Entry(agenda).State = EntityState.Modified;
                    context.Entry(agenda).Property(x => x.DataCadastro).IsModified = false;
                    context.Entry(agenda).Property(x => x.NomeUsuarioCadastro).IsModified = false;
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
                    var agenda = context.ProcessosAgendaProfissional.Find(id);
                    context.ProcessosAgendaProfissional.Attach(agenda);
                    context.ProcessosAgendaProfissional.Remove(agenda);
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

        public static Object Listar(string cliente, string processo, string prazo1, string prazo2, string evento, string profissional, string cadastro, string vara, Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                string[] datas;
                DateTime datainicial;
                DateTime datafinal;

                var nome = PessoasBo.ConverteNome(cliente);

                if (evento.Trim().Length > 0 && evento.IndexOf("(") > 0)
                {
                    evento = PessoasBo.ConverteNome(evento.Substring(0, evento.IndexOf("(")).Trim());
                }
                else
                {
                    evento = PessoasBo.ConverteNome(evento.Trim());
                }

                vara = PessoasBo.ConverteNome(vara.Trim());

                if (!String.IsNullOrEmpty(prazo1))
                {
                    datas = prazo1.Replace(" ", "").Split('-');
                    datainicial = DateTime.Parse(datas[0]);
                    datafinal = DateTime.Parse(datas[1]);

                    var query = (from d in context.View_ProcessosAgendasProfissionais_Listagem
                                 where d.NomeBusca.Contains(nome)
                                 where d.Profissional.Contains(profissional)
                                 where d.EventoNome.Contains(evento)
                                 where d.NumeroProcesso.Contains(processo)
                                 where d.Prazo1 >= datainicial && d.Prazo1 <= datafinal
                                 where d.VaraNome.Contains(vara)
                                 where IdProfissional <= 0 || d.IdProfissional == IdProfissional
                                 orderby d.Prazo1 ascending
                                 select d).Distinct().Take(500).OrderBy(x => x.Prazo1).ToList();


                    return query;
                }
                else if (!String.IsNullOrEmpty(prazo2))
                {
                    datas = prazo2.Replace(" ", "").Split('-');
                    datainicial = DateTime.Parse(datas[0]);
                    datafinal = DateTime.Parse(datas[1]);

                    var query = (from d in context.View_ProcessosAgendasProfissionais_Listagem
                                 where d.NomeBusca.Contains(nome)
                                 where d.Profissional.Contains(profissional)
                                 where d.EventoNome.Contains(evento)
                                 where d.NumeroProcesso.Contains(processo)
                                 where d.Prazo2 >= datainicial && d.Prazo2 <= datafinal
                                 where d.VaraNome.Contains(vara)
                                 where IdProfissional <= 0 || d.IdProfissional == IdProfissional
                                 orderby d.Prazo1 ascending
                                 select d).Distinct().Take(500).OrderBy(x => x.Prazo1).ToList();

                    return query;
                }
                else if (!String.IsNullOrEmpty(cadastro))
                {
                    datas = cadastro.Replace(" ", "").Split('-');
                    datainicial = DateTime.Parse(datas[0]);
                    datafinal = DateTime.Parse(datas[1]);

                    var query = (from d in context.View_ProcessosAgendasProfissionais_Listagem
                                 where d.NomeBusca.Contains(nome)
                                 where d.Profissional.Contains(profissional)
                                 where d.EventoNome.Contains(evento)
                                 where d.NumeroProcesso.Contains(processo)
                                 where d.DataCadastro >= datainicial && d.DataCadastro <= datafinal
                                 where d.VaraNome.Contains(vara)
                                 where IdProfissional <= 0 || d.IdProfissional == IdProfissional
                                 orderby d.Prazo1 ascending
                                 select d).Distinct().Take(500).OrderBy(x => x.Prazo1).ToList();

                    return query;
                }
                else
                {
                    var query = (from d in context.View_ProcessosAgendasProfissionais_Listagem
                                 where d.NomeBusca.Contains(nome)
                                 where d.Profissional.Contains(profissional)
                                 where d.EventoNome.Contains(evento)
                                 where d.NumeroProcesso.Contains(processo)
                                 where d.VaraNome.Contains(vara)
                                 where IdProfissional <= 0 || d.IdProfissional == IdProfissional
                                 orderby d.Prazo1 ascending
                                 select d).Distinct().Take(500).OrderBy(x => x.Prazo1).ToList();

                    return query;
                }
            }
        }

        public static ProcessosAgendaProfissional Consultar(Int32 IdProcessosEvento)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAgendaProfissional
                            where d.IdProcessosEvento == IdProcessosEvento
                            orderby d.DataCadastro descending
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ProcessosAgendaProfissional.Include("Profissionais")
                                                                          .Include("Profissionais.Pessoas")
                                                                          .Include("ProcessosAcoesEventos")
                                                                          .Include("ProcessosAcoesEventos.ProcessosAcoes")
                                                                          .Include("ProcessosAcoesEventos.ProcessosAcoes.Processos")
                                                                          .Include("ProcessosAcoesEventos.ProcessosAcoes.Processos.ProcessosAutores")
                                                                          .Include("ProcessosAcoesEventos.ProcessosAcoes.Processos.ProcessosAutores.Clientes")
                                                                          .Include("ProcessosAcoesEventos.ProcessosAcoes.Processos.ProcessosAutores.Clientes.Pessoas")
                                                                          .Include("ProcessosAcoesEventos.ProcessosEventos")
                                                                          .Include("ProcessosAcoesEventos.ProcessosAcoes.ProcessosVara")

                             where d.ID == Id
                             select new
                             {
                                 Id = d.ID,
                                 IdProfissional = d.IdProfissional,
                                 IdProcessosAcao = d.ProcessosAcoesEventos.IdProcessosAcao,
                                 Prazo1 = (d.ProcessosAcoesEventos.PrazoEvento1 != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.ProcessosAcoesEventos.PrazoEvento1), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.ProcessosAcoesEventos.PrazoEvento1), 2) + "/" + SqlFunctions.DatePart("year", d.ProcessosAcoesEventos.PrazoEvento1).ToString() : string.Empty),
                                 Prazo2 = (d.ProcessosAcoesEventos.PrazoEvento2 != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.ProcessosAcoesEventos.PrazoEvento2), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.ProcessosAcoesEventos.PrazoEvento2), 2) + "/" + SqlFunctions.DatePart("year", d.ProcessosAcoesEventos.PrazoEvento2).ToString() : string.Empty),
                                 Descricao = d.ProcessosAcoesEventos.Descricao ?? String.Empty,
                                 NumeroProcesso = d.ProcessosAcoesEventos.ProcessosAcoes.NumeroProcesso,
                                 IdProcessosEvento = d.IdProcessosEvento,
                                 IdEvento = d.ProcessosAcoesEventos.IdProcessosEvento,
                                 Evento = d.ProcessosAcoesEventos.ProcessosEventos.Descricao ?? string.Empty,
                                 CodigoEvento = d.ProcessosAcoesEventos.ProcessosEventos.Codigo ?? string.Empty,
                                 NomeProfissional = d.Profissionais.Pessoas.Nome ?? String.Empty,
                                 IdProcesso = d.ProcessosAcoesEventos.ProcessosAcoes.IdProcesso,
                                 IdCliente = d.ProcessosAcoesEventos.ProcessosAcoes.Processos.ProcessosAutores.FirstOrDefault().IdCliente,
                                 Cliente = d.ProcessosAcoesEventos.ProcessosAcoes.Processos.ProcessosAutores.FirstOrDefault().Clientes.Pessoas.Nome ?? String.Empty,
                                 Juizo = d.ProcessosAcoesEventos.ProcessosAcoes.ProcessosVara.Sigla ?? String.Empty,
                                 DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                 UsuarioCadastro = d.NomeUsuarioCadastro,
                                 DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                 UsuarioAlteracao = d.NomeUsuarioAlteracao ?? String.Empty,
                                 Excluido = d.Excluido
                             }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                IdProfissional = x.IdProfissional,
                                IdProcessosAcao = x.IdProcessosAcao,
                                Prazo1 = x.Prazo1,
                                Prazo2 = x.Prazo2,
                                Descricao = x.Descricao,
                                NumeroProcesso = x.NumeroProcesso,
                                IdProcessosEvento = x.IdProcessosEvento,
                                IdEvento = x.IdEvento,
                                Evento = x.Evento ?? string.Empty,
                                CodigoEvento = x.CodigoEvento ?? string.Empty,
                                NomeProfissional = x.NomeProfissional,
                                IdProcesso = x.IdProcesso,
                                IdCliente = x.IdCliente,
                                Cliente = x.Cliente,
                                Juizo = x.Juizo,
                                DataCadastro = x.DataCadastro,
                                UsuarioCadastro = x.UsuarioCadastro,
                                DataAlteracao = x.DataAlteracao,
                                UsuarioAlteracao = x.UsuarioAlteracao,
                                Excluido = x.Excluido
                            });

                return query.ToArray();
            }
        }


        public static ProcessosAgendaProfissional Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAgendaProfissional.Include("Profissionais")
                                                                         .Include("Profissionais.Pessoas")
                                                                         .Include("ProcessosAcoesEventos")
                                                                         .Include("ProcessosAcoesEventos.ProcessosAcoes")
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ListarArray()
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ProcessosAgendaProfissional.Include("ProcessosAcoesEventos")
                                                                          .Include("ProcessosAcoesEventos.ProcessosAcoes")
                             orderby d.ProcessosAcoesEventos.ProcessosAcoes.NumeroProcesso ascending
                             select new
                             {
                                 NumeroProcesso = d.ProcessosAcoesEventos.ProcessosAcoes.NumeroProcesso
                             }).Distinct();

                return query.ToArray();
            }
        }

        public static List<ProcessosAgendaProfissional> Visualizar(Int64 IdAcao)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAgendaProfissional.Include("Profissionais")
                                                                         .Include("Profissionais.Pessoas")
                                                                         .Include("ProcessosAcoesEventos")
                                                                         .Include("ProcessosAcoesEventos.ProcessosAcoes")
                                                                         .Include("ProcessosAcoesEventos.ProcessosEventos")
                            where d.ProcessosAcoesEventos.IdProcessosAcao == IdAcao
                            where d.Excluido == false
                            orderby d.ProcessosAcoesEventos.Data descending
                            select d;

                return query.ToList();
            }
        }
        public static ProcessosAgendaProfissional ProfissionalAgenda(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAgendaProfissional.Include("Profissionais")
                                                                         .Include("Profissionais.Pessoas")
                            where d.IdProcessosEvento == Id
                            where d.Excluido == false
                            orderby d.ID descending
                            select d;

                return query.FirstOrDefault();
            }
        }
    }
}