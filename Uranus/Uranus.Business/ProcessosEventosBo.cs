using Newtonsoft.Json;
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
    public class ProcessosEventosBo : IRequiresSessionState
    {
        public class ProcessoEvento
        {
            public int Id { get; set; }
            public string NumeroProcesso { get; set; }
            public int? IdProfissional { get; set; }
            public int? IdProcessoAcao { get; set; }
            public int? IdEvento { get; set; }
            public string PrazoEvento1 { get; set; }
            public string PrazoEvento2 { get; set; }
            public string Texto { get; set; }
            public string Tipo { get; set; }
            public int? IdAgendaProfissional { get; set; }
        }

        public static Int32 Inserir(ProcessosAcoesEventos acaoEvento)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.ProcessosAcoesEventos.Add(acaoEvento);
                    context.SaveChanges();

                    return acaoEvento.ID;
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

        public static void Salvar(ProcessosAcoesEventos acaoEvento)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.Entry(acaoEvento).State = EntityState.Modified;
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
                    var acaoEvento = context.ProcessosAcoesEventos.Find(id);
                    context.ProcessosAcoesEventos.Attach(acaoEvento);
                    context.ProcessosAcoesEventos.Remove(acaoEvento);
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

        public static List<ProcessosAcoesEventos> Listar(Int64 IdProcesso)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoesEventos.Include("ProcessosAcoes")
                                                                   .Include("ProcessosEventos")
                                                                   .Include("ProcessosAgendaProfissional")
                            where d.ProcessosAcoes.IdProcesso == IdProcesso
                            orderby d.Data descending
                            select d;

                return query.ToList();
            }
        }

        public static List<ProcessosAcoesEventos> Visualizar(Int64 IdAcao)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoesEventos.Include("ProcessosAcoes")
                                                                   .Include("ProcessosEventos")
                                                                   .Include("Usuarios")
                                                                   .Include("Usuarios1")
                            where d.ProcessosAcoes.ID == IdAcao
                            orderby d.Data descending
                            select d;

                return query.ToList();
            }
        }

        public static ProcessosAcoesEventos VisualizarEvento(Int64 IdEvento)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoesEventos.Include("ProcessosAcoes")
                                                                   .Include("ProcessosEventos")
                                                                   .Include("Usuarios")
                                                                   .Include("Usuarios1")
                                                                   .Include("ProcessosAgendaProfissional")
                            where d.ID == IdEvento
                            orderby d.Data descending
                            select d;

                return query.FirstOrDefault();
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

        public static ProcessosAcoesEventos Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoesEventos.Include("Processos")
                                                                   .Include("Processos.Sedes")
                                                                   .Include("Processos.Profissionais")
                                                                   .Include("Processos.Profissionais.Pessoas")
                                                                   .Include("ProcessosCadastroAcao")
                                                                   .Include("ProcessosVara")
                                                                   .Include("ProcessosAreas")
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ListarArray()
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ProcessosAcoesEventos.Include("ProcessosAcoes")
                             orderby d.ProcessosAcoes.NumeroProcesso ascending
                             select new
                             {
                                 NumeroProcesso = d.ProcessosAcoes.NumeroProcesso
                             }).Distinct();

                return query.ToArray();
            }
        }

        public static ProcessosEventos Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosEventos
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<ProcessoEvento> ConsultarEventos(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ProcessosAcoesEventos.Include("ProcessosAcoes")
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                NumeroProcesso = d.ProcessosAcoes.NumeroProcesso,
                                IdProcessoAcao = d.IdProcessosAcao,
                                IdEvento = d.IdProcessosEvento,
                                PrazoEvento1 = (d.PrazoEvento1 != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.PrazoEvento1), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.PrazoEvento1), 2) + "/" + SqlFunctions.DatePart("year", d.PrazoEvento1).ToString() : String.Empty),
                                PrazoEvento2 = (d.PrazoEvento2 != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.PrazoEvento2), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.PrazoEvento2), 2) + "/" + SqlFunctions.DatePart("year", d.PrazoEvento2).ToString() : String.Empty),
                                Texto = d.Descricao
                            }).ToList();

                List<ProcessoEvento> evento = new List<ProcessoEvento>();

                if (query.Count > 0)
                {
                    var jsonString = JsonConvert.SerializeObject(query);

                    evento = JsonConvert.DeserializeObject<List<ProcessoEvento>>(jsonString);
                }

                return evento.ToList();
            }
        }
    }
}