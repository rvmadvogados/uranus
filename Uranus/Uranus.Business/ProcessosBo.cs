using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;
using Uranus.Domain.Entities;

namespace Uranus.Business
{
    public class ProcessosBo : IRequiresSessionState
    {
        public static Int32 Inserir(Processos processo)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.Processos.Add(processo);
                    context.SaveChanges();

                    return processo.ID;
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

        public static void Salvar(Processos processo)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.Entry(processo).State = EntityState.Modified;
                    context.Entry(processo).Property(x => x.DataInclusao).IsModified = false;
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
                    var processo = context.Processos.Find(id);
                    context.Processos.Attach(processo);
                    context.Processos.Remove(processo);
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

        public static Object Listar(string processo, string cliente, string area, string status, string vara)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                var nome = PessoasBo.ConverteNome(cliente);

                var query = (from d in context.View_Processos_Listagem
                             where d.NumeroProcesso.Contains(processo)
                             where d.NomeBusca.Contains(nome)
                             where d.Area.Contains(area)
                             where d.Status.Contains(status)
                             where d.VaraNome.Contains(vara)
                             orderby d.DataInclusao descending
                             select new
                             {
                                 Id = d.ID,
                                 NumeroProcesso = d.NumeroProcesso,
                                 Autor = d.Autor,
                                 Reu = d.Reu,
                                 Area = d.Area,
                                 Status = d.Status,
                                 Vara = d.VaraSigla,
                                 IdAcao = d.IdAcao
                             }).Distinct().Take(500).ToList();

                return query;
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
                                                       .Include("ProcessosPartes")
                                                       .Include("ProcessosPartes.Clientes")
                                                       .Include("ProcessosPartes.Clientes.Sedes")
                            where d.ID == Id
                            select new
                            {
                                IdProcesso = d.ID,
                                DataHora = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataInclusao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataInclusao), 2) + "/" + SqlFunctions.DatePart("year", d.DataInclusao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataInclusao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataInclusao), 2),
                                IdProfissional = d.IdProfissionalResponsavel,
                                Status = d.Status,
                                Objeto = d.Objeto,
                                Observacao = d.Observacao,
                                Profissional = d.Profissionais.Pessoas.Nome,
                                IdSede = d.ProcessosAutores.FirstOrDefault().Clientes.IdSede,
                                Sede = d.ProcessosAutores.FirstOrDefault().Clientes.Sedes.Nome,
                                ClienteSede= d.ProcessosAutores.FirstOrDefault().Clientes.Pessoas.Cliente,
                                IdSedeReu = d.ProcessosPartes.FirstOrDefault().Clientes.IdSede,
                                SedeReu = d.ProcessosPartes.FirstOrDefault().Clientes.Sedes.Nome,
                                ClienteSedeReu = d.ProcessosPartes.FirstOrDefault().Clientes.Pessoas.Cliente,
                            };

                return query.ToArray();
            }
        }

        public static Processos Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Processos.Include("Profissionais")
                                                       .Include("Profissionais.Pessoas")
                                                       .Include("ProcessosAutores")
                                                       .Include("ProcessosAutores.Clientes")
                                                       .Include("ProcessosAutores.Clientes.Pessoas")
                                                       .Include("ProcessosAutores.Clientes.Clientes2.Pessoas")
                                                       .Include("ProcessosAutores.Clientes.Pessoas.Fones")
                                                       .Include("ProcessosAutores.Clientes.Pessoas.Email")
                                                       .Include("ProcessosAutores.Clientes.Sedes")
                                                       .Include("ProcessosAutores.Clientes.Profissionais")
                                                       .Include("ProcessosAutores.Clientes.Profissionais.Pessoas")
                                                       .Include("ProcessosAutores.Clientes.Parceiros")
                                                       .Include("ProcessosAcoes")
                                                       .Include("ProcessosAcoes.ProcessosCadastroAcao")
                                                       .Include("ProcessosAcoes.ProcessosVara")
                                                       .Include("ProcessosAcoes.ProcessosAreas")
                                                       .Include("ProcessosPartes")
                                                       .Include("ProcessosPartes.Clientes")
                                                       .Include("ProcessosPartes.Clientes.Pessoas")
                                                       .Include("ProcessosPartes.Clientes.Clientes2.Pessoas")
                                                       .Include("ProcessosPartes.Clientes.Pessoas.Fones")
                                                       .Include("ProcessosPartes.Clientes.Pessoas.Email")
                                                       .Include("ProcessosPartes.Clientes.Sedes")
                                                       .Include("ProcessosPartes.Clientes.Profissionais")
                                                       .Include("ProcessosPartes.Clientes.Profissionais.Pessoas")
                                                       .Include("ProcessosPartes.Clientes.Parceiros")
                                                       .Include("ProcessosPartes.ProcessosPartesAdvogados")
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static ProcessosAcoes AcaoAnterior(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoes
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static ProcessosAutores ConsultaCliente(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAutores
                            where d.IdProcesso == Id
                            select d;

                return query.FirstOrDefault();
            }
        }


        public static List<ConsultaFinanceiro> ListarFinanceiro(Int32 IdCliente)
        {

            using (var context = new UranusEntities())
            {

                context.Database.CommandTimeout = 360;

                SqlParameter param1 = new SqlParameter("@IdCliente", IdCliente);

                var query = context.Database.SqlQuery<ConsultaFinanceiro>("stpProcessos_Financeiros @IdCliente", param1);

                return query.ToList();
            }

        }


    }
}