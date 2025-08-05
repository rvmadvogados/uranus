
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.SessionState;
using Uranus.Business.DTOs;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class AgendasBo : IRequiresSessionState
    {
        public static Int32 Inserir(Agendas agenda)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.Agendas.Add(agenda);
                    context.SaveChanges();

                    return agenda.Id;
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
                    return -70;
                }
            }
        }

        public static void Salvar(Agendas agenda)
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
                    var agenda = context.Agendas.Find(id);
                    context.Agendas.Attach(agenda);
                    context.Agendas.Remove(agenda);
                    context.SaveChanges();
                }

                return "00";
            }
            catch (Exception ex)
            {
                //String error = "99";
                //String message = ex.InnerException.ToString();

                //if (message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                //    error = "98";

                //return error;
                return "00";
            }
        }

        //public static List<Agendas> Listar(DateTime? DataInicial, DateTime? DataFinal)
        //{
        //    using (var context = new UranusEntities())
        //    {
        //        System.Threading.Thread.Sleep(3000);
        //        if (DataInicial != null && DataFinal == null)
        //        {
        //            var query = from d in context.Agendas.Include("AgendasTipos")
        //                                                 .Include("Sedes")
        //                                                 .Include("Sedes.Salas")
        //                                                 .Include("Salas")
        //                                                 .Include("Profissionais")
        //                                                 .Include("Profissionais.Pessoas")
        //                                                 .Include("Clientes")
        //                                                 .Include("Clientes.Pessoas")
        //                        where d.Data >= DataInicial && d.Data <= DbFunctions.AddDays(DateTime.Now, 120)
        //                        where d.Cancelou == false
        //                        select d;

        //            return query.ToList();
        //        }
        //        else if (DataInicial == null && DataFinal != null)
        //        {
        //            var query = from d in context.Agendas.Include("AgendasTipos")
        //                                                 .Include("Sedes")
        //                                                 .Include("Sedes.Salas")
        //                                                 .Include("Salas")
        //                                                 .Include("Profissionais")
        //                                                 .Include("Profissionais.Pessoas")
        //                                                 .Include("Clientes")
        //                                                 .Include("Clientes.Pessoas")
        //                        where d.Data >= DbFunctions.AddDays(DateTime.Now, -120) && d.Data <= DataFinal
        //                        where d.Cancelou == false
        //                        select d;

        //            return query.ToList();
        //        }
        //        else if (DataInicial != null && DataFinal != null)
        //        {
        //            var query = from d in context.Agendas.Include("AgendasTipos")
        //                                                 .Include("Sedes")
        //                                                 .Include("Sedes.Salas")
        //                                                 .Include("Salas")
        //                                                 .Include("Profissionais")
        //                                                 .Include("Profissionais.Pessoas")
        //                                                 .Include("Clientes")
        //                                                 .Include("Clientes.Pessoas")
        //                        where d.Data >= DataInicial && d.Data <= DataFinal
        //                        where d.Cancelou == false
        //                        select d;

        //            return query.ToList();
        //        }
        //        else
        //        {
        //            var query = from d in context.Agendas.Include("AgendasTipos")
        //                                                 .Include("Sedes")
        //                                                 .Include("Sedes.Salas")
        //                                                 .Include("Salas")
        //                                                 .Include("Profissionais")
        //                                                 .Include("Profissionais.Pessoas")
        //                                                 .Include("Clientes")
        //                                                 .Include("Clientes.Pessoas")
        //                        where d.Data >= DbFunctions.AddDays(DateTime.Now, -120)
        //                        where d.Cancelou == false
        //                        select d;
                  
        //            return query.ToList();
        //        }
        //    }

        //}


        public static List<EventoDTO> Listar(DateTime? dataInicial, DateTime? dataFinal)
        {
            using (var context = new UranusEntities())
            {
                var dataInicio = dataInicial ?? DateTime.Now.AddDays(-120);
                var dataFim = dataFinal ?? DateTime.Now.AddDays(120);

                string sql = @"
                SELECT 
                    a.Id,
                    a.Data,
                    a.Hora,
                    a.Encaixe,
                    a.IdSede,
                    a.IdProfissional,
                    a.IdCliente,
                    a.IdTipo,
                    a.IdSala,
                    a.IdUsuario,
                    pp.Nome AS NomeProfissional,
                    pc.Nome AS NomeCliente,
                    t.Nome AS NomeTipo,
                    s.Nome AS NomeSede,
                    sa.Nome AS NomeSala

                FROM Agendas a WITH (NOLOCK)
                LEFT JOIN AgendasTipos t WITH (NOLOCK) ON a.IdTipo = t.Id
                LEFT JOIN Sedes s WITH (NOLOCK) ON a.IdSede = s.Id
                LEFT JOIN Salas sa WITH (NOLOCK) ON a.IdSala = sa.Id
                LEFT JOIN Profissionais p WITH (NOLOCK) ON a.IdProfissional = p.Id
                LEFT JOIN Pessoas pp WITH (NOLOCK) ON p.IdPessoa = pp.Id
                LEFT JOIN Clientes c WITH (NOLOCK) ON a.IdCliente = c.Id
                LEFT JOIN Pessoas pc WITH (NOLOCK) ON c.IdPessoa = pc.Id

                WHERE a.Cancelou = 0 AND a.Data BETWEEN @p0 AND @p1";

                return context.Database.SqlQuery<EventoDTO>(sql, dataInicio, dataFim).ToList();
            }
        }

        public static List<Agendas> Listar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 9600;

                var query = from d in context.Agendas.Include("AgendasTipos")
                                                     .Include("Sedes")
                                                     .Include("Sedes.Salas")
                                                     .Include("Salas")
                                                     .Include("Profissionais")
                                                     .Include("Profissionais.Pessoas")
                                                     .Include("Profissionais.Pessoas.Email")
                                                     .Include("Clientes")
                                                     .Include("Clientes.Pessoas")
                            where d.Id == Id
                            //where d.Cancelou == false
                            //                            where d.IdCliente != null
                            orderby d.IdProfissional, d.Data, d.Hora ascending
                            select d;

                return query.ToList();
            }
        }

        public static List<Agendas> Listar(DateTime DataInicial, DateTime DataFinal)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 9600;

                var query = from d in context.Agendas.Include("AgendasTipos")
                                                     .Include("Sedes")
                                                     .Include("Sedes.Salas")
                                                     .Include("Salas")
                                                     .Include("Profissionais")
                                                     .Include("Profissionais.Pessoas")
                                                     .Include("Profissionais.Pessoas.Email")
                                                     .Include("Clientes")
                                                     .Include("Clientes.Pessoas")
                            where (d.Data >= DataInicial && d.Data <= DataFinal)
                            where d.Cancelou == false
                            //where d.IdCliente != null
                            orderby d.IdProfissional, d.Data, d.Hora ascending
                            select d;

                return query.ToList();
            }
        }

        public static List<Agendas> ListarPeriodo(DateTime DataInicial, DateTime DataFinal)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 9600;

                var query = from d in context.Agendas.Include("AgendasTipos")
                                                     .Include("Sedes")
                                                     .Include("Sedes.Salas")
                                                     .Include("Salas")
                                                     .Include("Profissionais")
                                                     .Include("Profissionais.Pessoas")
                                                     .Include("Profissionais.Pessoas.Email")
                                                     .Include("Clientes")
                                                     .Include("Clientes.Pessoas")
                            where (d.Data >= DataInicial && d.Data < DataFinal)
                            where d.Cancelou == false
                            //where d.IdCliente != null
                            orderby d.IdProfissional, d.Data, d.Hora ascending
                            select d;

                return query.ToList();
            }
        }

        public static List<Agendas> ListarPeriodoAux(DateTime DataInicial, DateTime DataFinal, Int32? IdProfissional, Int32? IdCliente, Int32? IdSede)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 9600;

                if (IdProfissional != null && IdCliente != null && IdSede != null)
                {
                    var query = from d in context.Agendas.Include("AgendasTipos")
                                                         .Include("Sedes")
                                                         .Include("Sedes.Salas")
                                                         .Include("Salas")
                                                         .Include("Profissionais")
                                                         .Include("Profissionais.Pessoas")
                                                         .Include("Profissionais.Pessoas.Email")
                                                         .Include("Clientes")
                                                         .Include("Clientes.Pessoas")
                                where (d.Data >= DataInicial && d.Data <= DataFinal)
                                where d.IdProfissional == IdProfissional
                                where d.IdCliente == IdCliente
                                where d.IdSede == IdSede
                                where d.Cancelou == false
                                orderby d.Data, d.Hora, d.IdProfissional ascending
                                select d;

                    return query.ToList();
                }
                else if (IdProfissional != null && IdCliente == null)
                {
                    var query = from d in context.Agendas.Include("AgendasTipos")
                                                         .Include("Sedes")
                                                         .Include("Sedes.Salas")
                                                         .Include("Salas")
                                                         .Include("Profissionais")
                                                         .Include("Profissionais.Pessoas")
                                                         .Include("Profissionais.Pessoas.Email")
                                                         .Include("Clientes")
                                                         .Include("Clientes.Pessoas")
                                where (d.Data >= DataInicial && d.Data <= DataFinal)
                                where d.IdProfissional == IdProfissional
                                where d.Cancelou == false
                                orderby d.Data, d.Hora, d.IdProfissional ascending
                                select d;

                    return query.ToList();
                }
                else if (IdProfissional == null && IdCliente != null)
                {
                    var query = from d in context.Agendas.Include("AgendasTipos")
                                                         .Include("Sedes")
                                                         .Include("Sedes.Salas")
                                                         .Include("Salas")
                                                         .Include("Profissionais")
                                                         .Include("Profissionais.Pessoas")
                                                         .Include("Profissionais.Pessoas.Email")
                                                         .Include("Clientes")
                                                         .Include("Clientes.Pessoas")
                                where (d.Data >= DataInicial && d.Data <= DataFinal)
                                where d.IdCliente == IdCliente
                                where d.Cancelou == false
                                orderby d.Data, d.Hora, d.IdProfissional ascending
                                select d;

                    return query.ToList();
                }
                else if (IdProfissional != null && IdCliente == null && IdSede != null)
                {
                    var query = from d in context.Agendas.Include("AgendasTipos")
                                                         .Include("Sedes")
                                                         .Include("Sedes.Salas")
                                                         .Include("Salas")
                                                         .Include("Profissionais")
                                                         .Include("Profissionais.Pessoas")
                                                         .Include("Profissionais.Pessoas.Email")
                                                         .Include("Clientes")
                                                         .Include("Clientes.Pessoas")
                                where (d.Data >= DataInicial && d.Data <= DataFinal)
                                where d.IdCliente == IdCliente
                                where d.IdSede == IdSede
                                where d.Cancelou == false
                                orderby d.Data, d.Hora, d.IdProfissional ascending
                                select d;

                    return query.ToList();
                }
                else if (IdProfissional == null && IdCliente == null && IdSede != null)
                {
                    var query = from d in context.Agendas.Include("AgendasTipos")
                                                         .Include("Sedes")
                                                         .Include("Sedes.Salas")
                                                         .Include("Salas")
                                                         .Include("Profissionais")
                                                         .Include("Profissionais.Pessoas")
                                                         .Include("Profissionais.Pessoas.Email")
                                                         .Include("Clientes")
                                                         .Include("Clientes.Pessoas")
                                where (d.Data >= DataInicial && d.Data <= DataFinal)
                                where d.IdSede == IdSede
                                where d.Cancelou == false
                                orderby d.Data, d.Hora, d.IdProfissional ascending
                                select d;

                    return query.ToList();
                }
                else if (IdProfissional == null && IdCliente != null && IdSede != null)
                {
                    var query = from d in context.Agendas.Include("AgendasTipos")
                                                         .Include("Sedes")
                                                         .Include("Sedes.Salas")
                                                         .Include("Salas")
                                                         .Include("Profissionais")
                                                         .Include("Profissionais.Pessoas")
                                                         .Include("Profissionais.Pessoas.Email")
                                                         .Include("Clientes")
                                                         .Include("Clientes.Pessoas")
                                where (d.Data >= DataInicial && d.Data <= DataFinal)
                                where d.IdSede == IdSede
                                where d.IdCliente == IdCliente
                                where d.Cancelou == false
                                orderby d.Data, d.Hora, d.IdProfissional ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Agendas.Include("AgendasTipos")
                                                         .Include("Sedes")
                                                         .Include("Sedes.Salas")
                                                         .Include("Salas")
                                                         .Include("Profissionais")
                                                         .Include("Profissionais.Pessoas")
                                                         .Include("Profissionais.Pessoas.Email")
                                                         .Include("Clientes")
                                                         .Include("Clientes.Pessoas")
                                where (d.Data >= DataInicial && d.Data <= DataFinal)
                                where d.Cancelou == false
                                orderby d.Data, d.Hora, d.IdProfissional ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static Agendas Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 9600;

                var query = from d in context.Agendas.Include("AgendasTipos")
                                                     .Include("Sedes")
                                                     .Include("Sedes.Salas")
                                                     .Include("Salas")
                                                     .Include("Profissionais")
                                                     .Include("Profissionais.Pessoas")
                                                     .Include("Profissionais.Pessoas.Email")
                                                     .Include("Clientes")
                                                     .Include("Clientes.Pessoas")

                            where d.Id == Id
                            where d.Cancelou == false
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Agendas Consultar(DateTime Data, String Hora, Int32 IdTipo, Int32 IdSede, Int32 IdProfissional, Int32 IdUsuario)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 9600;

                var query = from d in context.Agendas
                            where d.Data == Data
                            where d.Hora == Hora
                            where d.IdTipo == IdTipo
                            where d.IdSede == IdSede
                            where d.IdProfissional == IdProfissional
                            where d.IdUsuario == IdUsuario
                            where d.Cancelou == false
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Agendas ConsultarSala(Int32 Id, DateTime Data, String Hora, Int32 IdTipo, Int32 IdSede, Int32? Sala)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Agendas
                            where d.Id != Id
                            where d.Data == Data
                            where d.Hora == Hora
                            where d.IdTipo == IdTipo
                            where d.IdSede == IdSede
                            where d.IdSala == Sala
                            where d.Cancelou == false
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 9600;

                var query = from d in context.Agendas.Include("Clientes")
                                                     .Include("Pessoas")
                                                     .Include("Pessoas.Fones")
                                                     .Include("Clientes.ProcessosIndicacoesTipos")
                            where d.Id == Id
                            where d.Cancelou == false
                            select new
                            {
                                Id = d.Id,
                                Data = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Data), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Data), 2) + "/" + SqlFunctions.DatePart("year", d.Data).ToString(),
                                Hora = d.Hora.Substring(0, 5),
                                IdTipo = d.IdTipo,
                                IdSede = d.IdSede,
                                IdSala = d.IdSala,
                                Valor = d.ValorConsulta,
                                IdProfissional = d.IdProfissional,
                                IdCliente = d.IdCliente,
                                Cliente = d.Clientes.Pessoas.Nome,
                                Descricao = d.MotivoConsulta,
                                Telefone = d.Clientes.Pessoas.Fones.Where(x => x.Status == "Ativo").OrderByDescending(x => x.Principal).Take(1).FirstOrDefault().Numero,
                                Encaixe = d.Encaixe,
                                Compareceu = (d.Compareceu != null ? (d.Compareceu.Value ? "S" : "N") : "N"),
                                TipoIndicacao = d.Clientes.IdIndicacaoTipo,
                                Indicacao = d.Clientes.Indicacao,
                                Parceiro = d.Clientes.IdParceiro,
                                Profissional = d.Clientes.IdProfissional,
                                Tipo = d.Clientes.ProcessosIndicacoesTipos.Tipo,
                                DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                UsuarioCadastro = d.NomeUsuarioCadastro,
                                DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                UsuarioAlteracao = d.NomeUsuarioAlteracao,
                                IdFormaPagamento = d.IdFormaPagamento,
                                IdBanco = d.IdBanco,
                                IdArea = d.IdArea,
                                IdReceita = d.IdReceita
                            };

                return query.Take(1).ToArray();
            }
        }

        public static string ConsultarAgendasDisponiveis()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Agendas
                            where d.IdCliente == null
                            where d.Data.Month == DateTime.Now.Month
                            where d.Cancelou == false
                            group d by d.Data into g
                            select new
                            {
                                Dia = g.Key.Day,
                                Total = g.Count()
                            };

                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                string disponiveis = string.Empty;

                var dias = query.ToList();

                int total = 0;
                for (int day = 1; day < (days + 1); day++)
                {
                    if (dias.Where(x => x.Dia == day).Any())
                    {
                        total = dias.Where(x => x.Dia == day).FirstOrDefault().Total;
                    }
                    else
                    {
                        total = 0;
                    }

                    disponiveis += string.Format("{0}[gd({1}, {2}, {3}), {4}]", (disponiveis.Length > 0 ? ", " : string.Empty), DateTime.Now.Year, DateTime.Now.Month, day, total);
                }

                disponiveis = string.Format("[{0}]", disponiveis);
                return disponiveis;
            }
        }

        public static string ConsultarAgendasConfirmadas()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Agendas
                                //                            where d.IdCliente != null
                            where d.Data.Month == DateTime.Now.Month
                            where d.Cancelou == false
                            group d by d.Data into g
                            select new
                            {
                                Dia = g.Key.Day,
                                Total = g.Count()
                            };

                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                string confirmandas = string.Empty;

                var dias = query.ToList();

                int total = 0;
                for (int day = 1; day < (days + 1); day++)
                {
                    if (dias.Where(x => x.Dia == day).Any())
                    {
                        total = dias.Where(x => x.Dia == day).FirstOrDefault().Total;
                    }
                    else
                    {
                        total = 0;
                    }

                    confirmandas += string.Format("{0}[gd({1}, {2}, {3}), {4}]", (confirmandas.Length > 0 ? ", " : string.Empty), DateTime.Now.Year, DateTime.Now.Month, day, total);
                }

                confirmandas = string.Format("[{0}]", confirmandas);

                return confirmandas;
            }
        }

        public static List<Agendas> BuscarAgendaProfissionais(DateTime Data, String HoraInicial, String HoraFinal, Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 9600;

                var query = (from d in context.Agendas.Include("Profissionais")
                                                      .Include("Profissionais.Pessoas")
                             where d.Data == Data
                             where d.IdProfissional == IdProfissional
                             where d.Cancelou == false
                             orderby d.Profissionais.Pessoas.Nome ascending
                             select d)
                            .ToList().Where(p => Convert.ToInt32(p.Hora.Replace(":", "")) >= int.Parse(HoraInicial.Replace(":", ""))
                            && Convert.ToInt32(p.Hora.Replace(":", "")) <= int.Parse(HoraFinal.Replace(":", "")));

                return query.ToList();
            }
        }

        public static List<Agendas> ConsultarAgendaPeriodo(DateTime DataInicial, DateTime DataFinal, Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 9600;

                var query = from d in context.Agendas
                            where (d.Data >= DataInicial && d.Data <= DataFinal)
                            where d.Cancelou == false
                            where d.IdProfissional == IdProfissional
                            select d;

                return query.ToList();
            }
        }
    }
}
