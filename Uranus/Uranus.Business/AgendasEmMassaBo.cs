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
    public class AgendasEmMassaBo : IRequiresSessionState
    {
        public static Int32 Inserir(AgendasMassa agenda)
        {
            using (var context = new UranusEntities())
            {
                context.AgendasMassa.Add(agenda);
                context.SaveChanges();

                return agenda.Id;
            }
        }

        public static void Salvar(AgendasMassa agenda)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.Entry(agenda).State = EntityState.Modified;
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
                    var agenda = context.AgendasMassa.Find(id);
                    context.AgendasMassa.Attach(agenda);
                    context.AgendasMassa.Remove(agenda);
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

        public static List<AgendasMassa> Listar(string sede, string profissional, string periodo)
        {
            using (var context = new UranusEntities())
            {
                if (!String.IsNullOrEmpty(periodo))
                {
                    string[] datas = periodo.Replace(" ", "").Split('-');
                    DateTime datainicial = DateTime.Parse(datas[0]);
                    DateTime datafinal = DateTime.Parse(datas[1]);

                    var query = from d in context.AgendasMassa.Include("AgendasTipos")
                                                              .Include("Sedes")
                                                              .Include("Salas")
                                                              .Include("Profissionais")
                                                              .Include("Profissionais.Pessoas")
                                where d.Sedes.Nome.Contains(sede)
                                where d.Profissionais.Pessoas.Nome.Contains(profissional)
                                where d.DataPeriodo >= datainicial && d.DataPeriodo <= datafinal
                                orderby d.DataPeriodo descending
                                select d;

                    return query.ToList();
                }
                else
                {
                    DateTime data = DateTime.Now.Date.AddDays(-30);
                    var query = from d in context.AgendasMassa.Include("AgendasTipos")
                                                              .Include("Sedes")
                                                              .Include("Salas")
                                                              .Include("Profissionais")
                                                              .Include("Profissionais.Pessoas")
                                where d.Sedes.Nome.Contains(sede)
                                where d.Profissionais.Pessoas.Nome.Contains(profissional)
                                where d.DataPeriodo >= data
                                orderby d.DataPeriodo descending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static Int32 InserirHistorico(AgendasMassaHistorico agenda)
        {
            using (var context = new UranusEntities())
            {
                context.AgendasMassaHistorico.Add(agenda);
                context.SaveChanges();

                return agenda.Id;
            }
        }

        public static Array ListarArray(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.AgendasMassaHistorico
                            where d.IdAgendaMassa == Id
                            select new
                            {
                                Data = d.Data,
                                Hora = d.Hora,
                                Sede = d.Sede,
                                Sala = d.Sala,
                                Profissional = d.Profissional,
                                Status = d.Status,
                                IdAgenda = d.IdAgenda
                            };

                return query.ToArray();
            }
        }

        public static List<AgendasMassaHistorico> Listar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.AgendasMassaHistorico
                            where d.IdAgendaMassa == Id
                            select d;

                return query.ToList();
            }
        }
    }
}